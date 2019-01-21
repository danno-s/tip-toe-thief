using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class StageChangeTrigger : MonoBehaviour
{
    public Door previousDoorLock;
    public TipToeThiefLogic gameLogic;

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Trigger activated.");
        enabled = false;
        previousDoorLock.Lock();
        gameLogic.NextLevel();
    }
}
