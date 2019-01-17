using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TipToeThiefGuardLogic : MonoBehaviour {

  public float walkSpeed, 
               rotationSpeed,
               sightRange,
               fieldOfView,
               investigationDelay,
               investigationTime;
  public bool canBeDistracted;
  public ChinchillaLogic playerReference;
  public TipToeThiefLogic gameLogic;
  public List<TipToeThiefGuardPatrolPoint> patrolPoints;
  public GuardPatrolType patrolType;
  public Color waitColor, rotateColor, moveColor, lookColor;

  private TipToeThiefGuardPatrolPoint currentPatrolPoint;
  private SpriteRenderer spriteR;
  private Coroutine currentCoroutine;
  private bool invertedPatrol;
  private float waitTime;

  private Transform distractionTransform;
  private GuardState guardState;
  private Vector3 lastPointOnPatrol, lastDirectionOnPatrol;

	// Use this for initialization
	void Start () {
    // Initial position
    if(patrolPoints.Count > 0) {
      transform.position = patrolPoints[0].transform.position;
      transform.rotation = patrolPoints[0].transform.rotation;
    }

    spriteR = GetComponent<SpriteRenderer>();

    currentPatrolPoint = patrolPoints[0];

    guardState = GuardState.Waiting;
    waitTime = 0;
    invertedPatrol = false;

    currentCoroutine = StartCoroutine("Waiting");
  }
	
	// Update is called once per frame
	void Update () {
    var vectorToPlayer = playerReference.transform.position - transform.position;
    if(gameLogic.IsLightOn() && vectorToPlayer.magnitude < sightRange) {
      if(Vector3.Angle(transform.up, vectorToPlayer) < fieldOfView) {
        // Cast a ray to check there is no wall between the guard and the player.
        if(!Physics2D.Raycast(transform.position, vectorToPlayer, sightRange, 1 << 8))
          StartCoroutine("PlayerFound");
      }
    }
	}

  public void Distract(Transform distractionTransform) {
    if(!canBeDistracted)
      return;

    this.distractionTransform = distractionTransform;
    waitTime = 0;
    lastPointOnPatrol = transform.position;
    lastDirectionOnPatrol = transform.up;

    if (guardState != GuardState.Distracted && currentCoroutine != null)
      StopCoroutine(currentCoroutine);

    if (guardState != GuardState.PlayerSpotted)
      currentCoroutine = StartCoroutine("Distracted");
  }

  private void RotateTowards(Vector3 vectorToTarget, out float angle) {
    RotateTowards(vectorToTarget, 1f, out angle);
  }

  private void RotateTowards(Vector3 vectorToTarget, float speedMultiplier, out float angle) {
    angle = Vector3.SignedAngle(transform.up, vectorToTarget, Vector3.forward);
    float rotateAngle;

    if(angle < 0)
      rotateAngle = Mathf.Max(-rotationSpeed * Time.deltaTime, angle);
    else
      rotateAngle = Mathf.Min(rotationSpeed * Time.deltaTime, angle);

    transform.Rotate(Vector3.forward, rotateAngle);
  }

  private TipToeThiefGuardPatrolPoint GetNextPatrolPoint() {
    int indexOfPatrol = patrolPoints.IndexOf(currentPatrolPoint);

    switch(patrolType) {
      case GuardPatrolType.BackAndForth:
        if(!invertedPatrol && indexOfPatrol == patrolPoints.Count - 1)
          invertedPatrol = true;
        else if(invertedPatrol && indexOfPatrol == 0)
          invertedPatrol = false;

        return patrolPoints[indexOfPatrol + (invertedPatrol ? -1 : 1)];
      case GuardPatrolType.Cyclical:
        return patrolPoints[(indexOfPatrol + 1) % patrolPoints.Count];
    }

    throw new Exception("No next patrol point found, maybe wrong patrol type?");
  }

  /*******************************************************
   *                                                     *
   * COROUTINES BEGIN                                    *
   *                                                     *
   *******************************************************/

  private IEnumerator Waiting() {
    guardState = GuardState.Waiting;

    while (waitTime < currentPatrolPoint.allotedTime) {
      waitTime += .1f;

      yield return new WaitForSecondsRealtime(.1f);
    }
    waitTime = 0;

    currentCoroutine = StartCoroutine("RotatingTowards");
  }

  private IEnumerator RotatingTowards() {
    float angle = 180;
    guardState = GuardState.RotatingTowards;

    while(angle != 0) {
      RotateTowards(currentPatrolPoint.transform.position - transform.position, out angle);

      yield return null;
    }

    currentCoroutine = StartCoroutine("MovingTowards");
  }

  private IEnumerator MovingTowards() {
    guardState = GuardState.Moving;

    while(transform.position != currentPatrolPoint.transform.position) {
      transform.position = 
        Vector3.MoveTowards(transform.position, currentPatrolPoint.transform.position, walkSpeed * Time.deltaTime);

      yield return null;
    }

    currentCoroutine = StartCoroutine("LookingTowards");
  }

  private IEnumerator LookingTowards() {
    float angle = 180;
    guardState = GuardState.LookingTowards;

    while(angle != 0) {
      RotateTowards(currentPatrolPoint.transform.up, out angle);

      yield return null;
    }

    currentPatrolPoint = GetNextPatrolPoint();
    currentCoroutine = StartCoroutine("Waiting");
  }

  private IEnumerator PlayerSpotted() {
    float angle;
    for (;;) {
      RotateTowards(playerReference.transform.position - transform.position, 4f, out angle);
      yield return null;
    }
  }

  private IEnumerator Distracted() {
    while(waitTime < investigationDelay) {
      waitTime += .1f;

      yield return new WaitForSecondsRealtime(.1f);
    }
    waitTime = 0;

    currentCoroutine = StartCoroutine("LookingAtDistraction");
  }

  private IEnumerator LookingAtDistraction() {
    float angle = 180;

    while(angle != 0) {
      RotateTowards(distractionTransform.position - transform.position, out angle);
      yield return null;
    }

    currentCoroutine = StartCoroutine("MovingToDistraction");
  }

  private IEnumerator MovingToDistraction() {
    while(transform.position != distractionTransform.position) {
      transform.position =
        Vector3.MoveTowards(transform.position, distractionTransform.position, walkSpeed * Time.deltaTime);

      yield return null;
    }

    currentCoroutine = StartCoroutine("InvestigatingDistraction");
  }

  private IEnumerator InvestigatingDistraction() {
    Quaternion startRotation = transform.rotation;

    while(waitTime < investigationTime) {
      waitTime += Time.deltaTime;
      transform.rotation = startRotation;
      transform.Rotate(Vector3.forward, Mathf.Sin(waitTime * Mathf.PI / 2) * 90);
      yield return null;
    }

    waitTime = 0;

    currentCoroutine = StartCoroutine("LookingBack");
  }

  private IEnumerator LookingBack() {
    float angle = 180;

    while(angle != 0) {
      RotateTowards(lastPointOnPatrol - transform.position, out angle);
      yield return null;
    }

    currentCoroutine = StartCoroutine("MovingBack");
  }

  private IEnumerator MovingBack() {
    while(transform.position != lastPointOnPatrol) {
      transform.position =
        Vector3.MoveTowards(transform.position, lastPointOnPatrol, walkSpeed * Time.deltaTime);

      yield return null;
    }

    currentCoroutine = StartCoroutine("RotatingBack");
  }

  private IEnumerator RotatingBack() {
    float angle = 180;

    while(angle != 0) {
      RotateTowards(lastDirectionOnPatrol, out angle);
      yield return null;
    }

    switch(guardState) {
      case GuardState.Waiting:
        currentCoroutine = StartCoroutine("Waiting");
        break;
      case GuardState.RotatingTowards:
        currentCoroutine = StartCoroutine("RotatingTowards");
        break;
      case GuardState.Moving:
        currentCoroutine = StartCoroutine("MovingTowards");
        break;
      case GuardState.LookingTowards:
        currentCoroutine = StartCoroutine("LookingTowards");
        break;
    }
  }

  private IEnumerator PlayerFound() {
    if (currentCoroutine != null)
      StopCoroutine(currentCoroutine);

    currentCoroutine = StartCoroutine("PlayerSpotted");
    guardState = GuardState.PlayerSpotted;
    spriteR.color = Color.black;
    yield return new WaitForSeconds(1.5f);
    gameLogic.RestartLevel();
  }

  /*******************************************************
   *                                                     *
   * COROUTINES END                                      *
   *                                                     *
   *******************************************************/

  private void OnDrawGizmos() {
#if UNITY_EDITOR
    switch(guardState) {
      case GuardState.Waiting:
        Handles.color = waitColor;
        break;
      case GuardState.RotatingTowards:
        Handles.color = rotateColor;
        break;
      case GuardState.Moving:
        Handles.color = moveColor;
        break;
      case GuardState.LookingTowards:
        Handles.color = lookColor;
        break;
    }
    Handles.DrawSolidArc(
      transform.position,
      transform.forward,
      Vector3.RotateTowards(transform.up, transform.right, Mathf.Deg2Rad * fieldOfView / 2, 1),
      fieldOfView,
      sightRange
    );
#endif
  }
}
