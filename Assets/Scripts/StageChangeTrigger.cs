using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class StageChangeTrigger : MonoBehaviour
{
    public TipToeThiefCameraManager cameraManager;
    public Transform newCameraPosition;
    public float newCameraSize;
    public Door previousDoorLock;

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Trigger activated.");
        enabled = false;
        previousDoorLock.enabled = true;
        cameraManager.SetNewCameraPosition(newCameraPosition, newCameraSize);
    }
}
