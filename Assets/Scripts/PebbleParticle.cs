using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PebbleParticle : MonoBehaviour
{
    public float duration;
    [HideInInspector]
    public Rigidbody2D rgbd;

    // Start is called before the first frame update
    void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, duration);
    }
}
