using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour, TipToeThiefResettableObject
{

    public Door door;
    private AudioSource audioSrc;
    private SpriteRenderer rndr;
    private bool activated = false;

    public void Reset()
    {
        activated = false;
        rndr.enabled = true;
    }

    private void Awake()
    {
        rndr = GetComponentInChildren<SpriteRenderer>();
        audioSrc = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated)
            return;

        if (other.gameObject.tag == "Player")
        {
            audioSrc.Play();
            door.Unlock();
            rndr.enabled = false;
            activated = true;
        }
    }

}
