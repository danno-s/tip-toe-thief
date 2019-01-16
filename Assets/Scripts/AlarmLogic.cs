using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLogic : MonoBehaviour {

  public float triggerTime;
  public float noiseRadius;

	// Use this for initialization
	void Start () {
    Invoke("Trigger", triggerTime);
    Destroy(this.gameObject, triggerTime + 15f);
	}

  private void Trigger() {
    foreach(TipToeThiefGuardLogic Guard in Object.FindObjectsOfType<TipToeThiefGuardLogic>())
      if(Vector3.Distance(transform.position, Guard.transform.position) <= noiseRadius)
        Guard.Distract(transform);
  }

  private void OnDrawGizmos() {
    Color c = Color.yellow;
    c.a = 0.4f;
    Gizmos.color = c;
    Gizmos.DrawSphere(transform.position, noiseRadius);
  }
}
