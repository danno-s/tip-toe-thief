using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class GameOverController : MonoBehaviour
{
    private void Awake()
    {
        var animator = GetComponent<Animator>();

        var transition = FindObjectOfType<TipToeThiefSceneTransition>();

        if (transition == null)
            return;

        switch (transition.gameOverCause)
        {
            case GameOverCause.CaughtByCat:
                animator.Play("Cat");
                break;
            case GameOverCause.CaughtByOwl:
                animator.Play("Owl");
                break;
        }

        Destroy(transition.gameObject);
    }
}
