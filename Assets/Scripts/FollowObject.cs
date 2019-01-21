using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject following;
    public Vector3 offset;
    public float moveSpeed;

    private void Start()
    {
        transform.position = following.transform.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            following.transform.position + offset,
            Time.deltaTime * moveSpeed
        );
    }
}
