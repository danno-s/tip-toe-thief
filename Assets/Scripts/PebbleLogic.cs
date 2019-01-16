using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PebbleLogic : MonoBehaviour {

  public float noiseRadius;
  [HideInInspector]
  public Rigidbody2D rgbd;
  private bool activated;

  private void Start() {
    activated = false;
    rgbd = GetComponent<Rigidbody2D>();
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if(activated)
      return;

    Debug.Log("Pebble collision");
    
    foreach (TipToeThiefGuardLogic Guard in Object.FindObjectsOfType<TipToeThiefGuardLogic>())
      if(Vector3.Distance(transform.position, Guard.transform.position) <= noiseRadius)
        Guard.Distract(transform);

    activated = true;
  }

  private void OnDrawGizmos() {
    Color c = Color.yellow;
    c.a = 0.4f;
    Gizmos.color = c;
    Gizmos.DrawSphere(transform.position, noiseRadius);
  }
}
