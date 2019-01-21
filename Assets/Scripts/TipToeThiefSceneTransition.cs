using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipToeThiefSceneTransition : MonoBehaviour
{
    public GameOverCause gameOverCause;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
