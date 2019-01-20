using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, TipToeThiefResettableObject
{
    private Animator anm;
    public bool startLocked,
                playAnimationOnStart,
                playFlipped;

    private void Start()
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
            anm.Play("Lock");
        else
            anm.Play("Lock Flipped");
    }

    public void Reset()
    {
        if (playAnimationOnStart)
        {
            if (startLocked)
                Lock();
            else
                Unlock();
        }
    }
}
