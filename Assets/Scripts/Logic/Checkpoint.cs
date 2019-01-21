using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour, TipToeThiefResettableObject
{
    public ChinchillaLogic player;

    public void Reset()
    {
        player.transform.position = transform.position;
    }
}
