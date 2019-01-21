using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TipToeThiefGuardLogic : MonoBehaviour, TipToeThiefResettableObject, TipToeThiefEntity {

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
    internal Coroutine currentCoroutine;
    private bool invertedPatrol,
                 playerFound,
                 distracted;
    private float waitTime;

    internal Transform distractionTransform;
    internal GuardState guardState;
    private Vector3 lastPointOnPatrol, lastDirectionOnPatrol;

    internal Vector3 lookingDirection;

    private Vector3 initialPosition;
    private Vector3 initialLookDirection;

    internal Animator anm;
    private bool movedThisFrame;
    private Direction lookDirection;

    // Use this for initialization
    internal virtual void Start() {
        // Initial position
        if(patrolPoints.Count > 0) {
            transform.position = patrolPoints[0].transform.position;
            lookingDirection = patrolPoints[0].transform.up;
        }

        initialPosition = transform.position;
        initialLookDirection = lookingDirection;

        anm = GetComponent<Animator>();
        movedThisFrame = false;
        distracted = false;

        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vectorToPlayer = playerReference.transform.position - transform.position;
        var col = Color.black;
        Debug.DrawRay(transform.position, GetSnappedDirection(lookingDirection), Color.magenta);
        if (gameLogic.IsLightOn() && vectorToPlayer.magnitude < sightRange)
        {
            col = Color.blue;
            if (Mathf.Abs(Vector2.SignedAngle(GetSnappedDirection(lookingDirection), vectorToPlayer)) < fieldOfView / 2f)
            {
                col = Color.cyan;
                // Cast a ray to check there is no wall between the guard and the player.
                if (!Physics2D.Raycast(transform.position, vectorToPlayer, sightRange, 1 << 8))
                {
                    col = Color.white;
                    StartCoroutine("PlayerFound");
                }
            }
        }

        Debug.DrawRay(transform.position, vectorToPlayer, col);
        UpdateAnimation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine("PlayerFound");
    }

    internal virtual Vector3 GetSnappedDirection(Vector3 direction)
    {
        float angle = Vector2.SignedAngle(Vector2.right, lookingDirection);

        if (angle > -135 && angle < -45)
            return Vector3.down;
        else if (angle > -45 && angle < 45)
            return Vector3.right;
        else if (angle > 45 && angle < 135)
            return Vector3.up;
        else
            return Vector3.left;
    }


    public virtual bool IsCat()
    {
        return true;
    }

    public virtual bool IsOwl()
    {
        return false;
    }

    internal virtual void UpdateAnimation()
    {
        float angle = Vector2.SignedAngle(Vector2.right, lookingDirection);

        if (angle > -135 && angle < -45)
            lookDirection = Direction.Down;
        else if (angle > -45 && angle < 45)
            lookDirection = Direction.Right;
        else if (angle > 45 && angle < 135)
            lookDirection = Direction.Up;
        else
            lookDirection = Direction.Left;

        string clipName = anm.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        if (movedThisFrame)
        {
            switch (lookDirection)
            {
                case Direction.Down:
                    if (clipName != "MoveDown")
                        anm.Play("MoveDown");
                    break;
                case Direction.Right:
                    if (clipName != "MoveRight")
                        anm.Play("MoveRight");
                    break;
                case Direction.Up:
                    if (clipName != "MoveUp")
                        anm.Play("MoveUp");
                    break;
                case Direction.Left:
                    if (clipName != "MoveLeft")
                        anm.Play("MoveLeft");
                    break;
            }
        }
        else
        {
            switch (lookDirection)
            {
                case Direction.Down:
                    if (clipName != "IdleDown")
                        anm.Play("IdleDown");
                    break;
                case Direction.Right:
                    if (clipName != "IdleRight")
                        anm.Play("IdleRight");
                    break;
                case Direction.Up:
                    if (clipName != "IdleUp")
                        anm.Play("IdleUp");
                    break;
                case Direction.Left:
                    if (clipName != "IdleLeft")
                        anm.Play("IdleLeft");
                    break;
            }
        }
    }

    public void Distract(Transform distractionTransform) {
        if(!canBeDistracted || distracted)
            return;

        distracted = true;
        this.distractionTransform = distractionTransform;
        waitTime = 0;
        lastPointOnPatrol = transform.position;
        lastDirectionOnPatrol = transform.up;

        if(guardState != GuardState.Distracted && currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if(guardState != GuardState.PlayerSpotted)
            currentCoroutine = StartCoroutine("Distracted");
    }

    internal void RotateTowards(Vector3 vectorToTarget, out float angle) {
        RotateTowards(vectorToTarget, 1f, out angle);
    }

    internal void RotateTowards(Vector3 vectorToTarget, float speedMultiplier, out float angle) {
        angle = Vector3.SignedAngle(lookingDirection, vectorToTarget, Vector3.forward);
        float rotateAngle;

        if(angle < 0)
            rotateAngle = Mathf.Max(-rotationSpeed * Time.deltaTime * speedMultiplier, angle);
        else
            rotateAngle = Mathf.Min(rotationSpeed * Time.deltaTime * speedMultiplier, angle);

        lookingDirection = Quaternion.Euler(0, 0, rotateAngle) * lookingDirection;

        Debug.DrawRay(transform.position, lookingDirection, Color.red);

        Debug.DrawRay(transform.position, vectorToTarget, Color.blue);
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
    
    public void Reset()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentPatrolPoint = patrolPoints[0];

        guardState = GuardState.Waiting;
        waitTime = 0;
        invertedPatrol = false;
        playerFound = false;
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        lookingDirection = initialLookDirection;

        UpdateAnimation();

        currentCoroutine = StartCoroutine("Waiting");
    }

    /*******************************************************
     *                                                     *
     * COROUTINES BEGIN                                    *
     *                                                     *
     *******************************************************/

    private IEnumerator Waiting() {
        guardState = GuardState.Waiting;

        movedThisFrame = false;
        distracted = false;

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

        movedThisFrame = false;
        distracted = false;

        while (angle != 0) {
            RotateTowards(currentPatrolPoint.transform.position - transform.position, out angle);

            yield return null;
        }

        currentCoroutine = StartCoroutine("MovingTowards");
    }

    private IEnumerator MovingTowards() {
        guardState = GuardState.Moving;

        movedThisFrame = true;
        distracted = false;

        while (transform.position != currentPatrolPoint.transform.position) {
            transform.position = 
              Vector3.MoveTowards(transform.position, currentPatrolPoint.transform.position, walkSpeed * Time.deltaTime);

            yield return null;
        }

        currentCoroutine = StartCoroutine("LookingTowards");
    }

    private IEnumerator LookingTowards() {
        float angle = 180;
        guardState = GuardState.LookingTowards;

        movedThisFrame = false;
        distracted = false;

        while (angle != 0) {
            RotateTowards(currentPatrolPoint.transform.up, out angle);

            yield return null;
        }

        currentPatrolPoint = GetNextPatrolPoint();
        currentCoroutine = StartCoroutine("Waiting");
    }

    private IEnumerator PlayerSpotted() {
        float angle;
        for(; ; ) {
            RotateTowards(playerReference.transform.position - transform.position, 4f, out angle);
            yield return null;
        }
    }

    internal virtual IEnumerator Distracted() {
        movedThisFrame = false;

        while(waitTime < investigationDelay) {
            waitTime += .1f;

            yield return new WaitForSecondsRealtime(.1f);
        }
        waitTime = 0;

        currentCoroutine = StartCoroutine("LookingAtDistraction");
    }

    internal virtual IEnumerator LookingAtDistraction() {
        float angle = 180;

        movedThisFrame = false;

        while(angle != 0) {
            if (distractionTransform == null)
                break;

            RotateTowards(distractionTransform.position - transform.position, 4f, out angle);
            yield return null;
        }

        currentCoroutine = StartCoroutine("MovingToDistraction");
    }

    internal virtual IEnumerator MovingToDistraction() {
        movedThisFrame = true;

        while(distractionTransform != null && transform.position != distractionTransform.position) {
            transform.position =
              Vector3.MoveTowards(transform.position, distractionTransform.position, walkSpeed * Time.deltaTime);

            yield return null;
        }

        currentCoroutine = StartCoroutine("InvestigatingDistraction");
    }

    internal virtual IEnumerator InvestigatingDistraction() {
        Quaternion startRotation = transform.rotation;

        movedThisFrame = false;

        while(waitTime < investigationTime) {
            waitTime += Time.deltaTime;
            transform.rotation = startRotation;
            transform.Rotate(Vector3.forward, Mathf.Sin(waitTime * Mathf.PI / 2) * 90);
            yield return null;
        }

        waitTime = 0;
        distracted = false;

        currentCoroutine = StartCoroutine("LookingBack");
    }

    internal virtual IEnumerator LookingBack() {
        float angle = 180;

        movedThisFrame = false;
        distracted = false;

        while (angle != 0) {
            RotateTowards(lastPointOnPatrol - transform.position, out angle);
            yield return null;
        }

        currentCoroutine = StartCoroutine("MovingBack");
    }

    internal virtual IEnumerator MovingBack() {
        movedThisFrame = true;
        distracted = false;

        while (transform.position != lastPointOnPatrol) {
            transform.position =
              Vector3.MoveTowards(transform.position, lastPointOnPatrol, walkSpeed * Time.deltaTime);

            yield return null;
        }

        currentCoroutine = StartCoroutine("RotatingBack");
    }

    internal virtual IEnumerator RotatingBack() {
        float angle = 180;
        movedThisFrame = false;
        distracted = false;

        while (angle != 0) {
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
        if (playerFound)
            yield break;


        playerFound = true;

        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine("PlayerSpotted");
        guardState = GuardState.PlayerSpotted;
        yield return new WaitForSeconds(1.5f);
        gameLogic.RestartLevel(this);
    }

    /*******************************************************
     *                                                     *
     * COROUTINES END                                      *
     *                                                     *
     *******************************************************/

    internal virtual void OnDrawGizmos() {
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
          Quaternion.Euler(0, 0, -fieldOfView / 2) * GetSnappedDirection(lookingDirection),
          fieldOfView,
          sightRange
        );
#endif
    }
}
