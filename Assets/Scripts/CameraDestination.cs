using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDestination : MonoBehaviour
{
    public float cameraSize;

    private void OnDrawGizmos() {
        Gizmos.color = Color.black;

        Gizmos.DrawWireCube(transform.position, new Vector3(16f/9f * cameraSize * 2, cameraSize * 2, 0));
    }
}
