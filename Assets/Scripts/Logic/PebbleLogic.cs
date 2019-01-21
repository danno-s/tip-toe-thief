using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PebbleLogic : MonoBehaviour {

    public float noiseRadius,
                 particleAngles,
                 particleSpeed;
    public GameObject pebbleParticle;
    public AudioClip throwLong,
                     throwShort,
                     crash;
    [HideInInspector]
    public Rigidbody2D rgbd;
    private Animator anm;
    private AudioSource audiosrc;
    private bool activated;

    private void Start() {
        activated = false;
        rgbd = GetComponent<Rigidbody2D>();
        audiosrc = GetComponent<AudioSource>();
        anm = GetComponent<Animator>();
        anm.Play("Flying");
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(activated)
            return;
    
        foreach (TipToeThiefGuardLogic Guard in Object.FindObjectsOfType<TipToeThiefGuardLogic>())
            if(Vector2.Distance(transform.position, Guard.transform.position) <= noiseRadius)   
                Guard.Distract(transform);

        activated = true;
        anm.Play("Crash");


        for (float i = 0f; i * particleAngles < 360; i++)
        {
            PebbleParticle instancedParticle = Instantiate(pebbleParticle, transform).GetComponent<PebbleParticle>();
            instancedParticle.rgbd.velocity = new Vector2(
                particleSpeed * Mathf.Cos(i * particleAngles),
                particleSpeed * Mathf.Sin(i * particleAngles)
            );
        }
    }

    public void PlayLongThrow()
    {
        StopAndPlay(throwLong);
    }

    public void PlayShortThrow()
    {
        StopAndPlay(throwShort);
    }

    public void PlayCrash()
    {
        StopAndPlay(crash);
    }

    private void StopAndPlay(AudioClip audioClip)
    { 
        audiosrc.Stop();
        audiosrc.clip = audioClip;
        audiosrc.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, noiseRadius);
    }
}
