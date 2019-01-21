using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AlarmLogic : MonoBehaviour {

    public float triggerTime;
    public float noiseRadius;
    public AudioClip placed,
                     ticking,
                     activated;
    private AudioSource audioSrc;
    private Animator anm;

	// Use this for initialization
	void Start () {
        anm = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
        audioSrc.loop = false;
        Invoke("Trigger", triggerTime);
        Destroy(this.gameObject, triggerTime + 11.5f);
    }

    private void Trigger() {
        StartCoroutine("DistractGuards");

        audioSrc.clip = activated;
        audioSrc.Play();
        anm.Play("Triggered");
    }

    public IEnumerator DistractGuards()
    {
        for(; ; )
        {
            foreach (TipToeThiefGuardLogic Guard in Object.FindObjectsOfType<TipToeThiefGuardLogic>())
                if (Vector3.Distance(transform.position, Guard.transform.position) <= noiseRadius)
                    Guard.Distract(transform);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void StartAnimations()
    {
        StartCoroutine("AudioPattern");
    }

    public IEnumerator AudioPattern()
    {
        audioSrc.clip = placed;
        audioSrc.Play();
        yield return new WaitForSeconds(0.5f);
        audioSrc.clip = ticking;
        audioSrc.Play();
    }

    private void OnDrawGizmos() {
        Color c = Color.yellow;
        c.a = 0.4f;
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, noiseRadius);
    }
}
