using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour, TipToeThiefResettableObject
{

    public Door door;
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
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated)
            return;

        if (other.gameObject.tag == "Player")
        {
            door.Unlock();
            rndr.enabled = false;
            activated = true;
        }
    }

}
