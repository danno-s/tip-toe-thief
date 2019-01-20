using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ParticleSystem))]
public class PebbleLogic : MonoBehaviour, TipToeThiefResettableObject {

    public float noiseRadius;
    [HideInInspector]
    public Rigidbody2D rgbd;
    private ParticleSystem particles;
    private bool activated;

    private void Start() {
        activated = false;
        rgbd = GetComponent<Rigidbody2D>();
        particles = GetComponent<ParticleSystem>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(activated)
        return;

        Debug.Log("Pebble collision");
    
        foreach (TipToeThiefGuardLogic Guard in Object.FindObjectsOfType<TipToeThiefGuardLogic>())
            if(Vector3.Distance(transform.position, Guard.transform.position) <= noiseRadius)   
                Guard.Distract(transform);

        activated = true;
        particles.Play();
    }

    public void Reset()
    {
        Destroy(this);
    }
}
