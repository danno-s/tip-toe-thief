using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, TipToeThiefResettableObject
{
    private Animator anm;
    public bool startLocked,
                playAnimationOnStart,
                playFlipped;

    private void Awake()
    {
        anm = GetComponent<Animator>();

        Reset();
    }

    public void Unlock(float normalizedTime = 0f) 
    {
        if (!playFlipped)
            anm.Play("Unlock", -1, normalizedTime);
        else
            anm.Play("Unlock Flipped", -1, normalizedTime);
    }

    public void Lock(float normalizedTime = 0f)
    {
        if (!playFlipped)
            anm.Play("Lock", -1, normalizedTime);
        else
            anm.Play("Lock Flipped", -1, normalizedTime);
    }

    public void Reset()
    {
        float normalizedTime = playAnimationOnStart ? 0f : 1.0f;

        if (startLocked)
            Lock(normalizedTime);
        else
            Unlock(normalizedTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.up);
    }
}
