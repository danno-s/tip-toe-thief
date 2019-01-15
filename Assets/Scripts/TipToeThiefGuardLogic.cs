using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TipToeThiefGuardLogic : MonoBehaviour {

  public float walkSpeed, 
               rotationSpeed,
               sightRange,
               fieldOfView;
  public ChinchillaLogic playerReference;
  public TipToeThiefLogic gameLogic;
  public List<TipToeThiefGuardPatrolPoint> patrolPoints;
  public GuardPatrolType patrolType;
  public Color waitColor, rotateColor, moveColor, lookColor;
  private TipToeThiefGuardPatrolPoint currentPatrolPoint;
  private GuardState guardState;
  private bool invertedPatrol;
  private float waitTime;

	// Use this for initialization
	void Start () {
    // Initial position
    if(patrolPoints.Count > 0) {
      transform.position = patrolPoints[0].transform.position;
      transform.rotation = patrolPoints[0].transform.rotation;
    }

    currentPatrolPoint = patrolPoints[0];

    guardState = GuardState.Waiting;
    waitTime = 0;
    invertedPatrol = false;
  }
	
	// Update is called once per frame
	void Update () {
    float angle, rotateAngle;
    switch(guardState) {
      case GuardState.Waiting:
        waitTime += Time.deltaTime;

        if(waitTime >= currentPatrolPoint.allotedTime) {
          waitTime = 0;
          currentPatrolPoint = GetNextPatrolPoint();
          guardState = GuardState.RotatingTowards;
        }
        break;

      case GuardState.RotatingTowards:
        Vector3 vectorToTarget = currentPatrolPoint.transform.position - transform.position;
        angle = Vector3.SignedAngle(transform.up, vectorToTarget, Vector3.forward);

        if (angle < 0)
          rotateAngle = Mathf.Max(-rotationSpeed * Time.deltaTime, angle);
        else
          rotateAngle = Mathf.Min(rotationSpeed * Time.deltaTime, angle);

        transform.Rotate(Vector3.forward, rotateAngle);

        if(angle == 0)
          guardState = GuardState.Moving;
        break;

      case GuardState.Moving:
        transform.position = 
          Vector3.MoveTowards(transform.position, currentPatrolPoint.transform.position, walkSpeed * Time.deltaTime);

        if(transform.position == currentPatrolPoint.transform.position)
          guardState = GuardState.LookingTowards;
        break;

      case GuardState.LookingTowards:
        angle = Vector3.SignedAngle(transform.up, currentPatrolPoint.transform.up, Vector3.forward);

        if(angle < 0)
          rotateAngle = Mathf.Max(-rotationSpeed * Time.deltaTime, angle);
        else
          rotateAngle = Mathf.Min(rotationSpeed * Time.deltaTime, angle);

        transform.Rotate(Vector3.forward, rotateAngle);

        if(angle == 0)
          guardState = GuardState.Waiting;
        break;
    }


    var vectorToPlayer = playerReference.transform.position - transform.position;
    if(gameLogic.IsLightOn() && vectorToPlayer.magnitude < sightRange) {
      angle = Vector3.Angle(transform.up, vectorToPlayer);
      if(angle < fieldOfView) {
        PlayerFound();
      }
    }
	}

  private void PlayerFound() {
    Debug.Log("Player Found!!");
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

  private void OnDrawGizmos() {
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
  }
}
