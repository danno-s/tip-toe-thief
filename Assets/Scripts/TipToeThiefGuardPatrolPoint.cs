using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TipToeThiefGuardPatrolPoint : MonoBehaviour {
  public float allotedTime;
  public Color handleColor;

  private void OnDrawGizmos() {
#if UNITY_EDITOR
    Handles.color = handleColor;
    Handles.DrawSolidArc(
      transform.position,
      transform.forward,
      Vector3.RotateTowards(transform.up, transform.right, Mathf.PI / 8, 1),
      45,
      5
    );
#endif
  }
}
