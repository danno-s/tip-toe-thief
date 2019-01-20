using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Key : MonoBehaviour
{

    public Door door;
    private float rotateSpeed = 5f;

    public void Update() {
        transform.Rotate(Vector3.left * rotateSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("c");
            door.Unlock();
            Destroy(gameObject);
        }
    }

}
