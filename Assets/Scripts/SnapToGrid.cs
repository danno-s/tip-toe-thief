using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    public float pixelsPerUnit = 40;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 position = transform.localPosition;

        position.x = (Mathf.Round(transform.parent.position.x * pixelsPerUnit) / pixelsPerUnit) - transform.parent.position.x; 
        position.y = (Mathf.Round(transform.parent.position.y * pixelsPerUnit) / pixelsPerUnit) - transform.parent.position.y;

        transform.localPosition = position;

    }
}
