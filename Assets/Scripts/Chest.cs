using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chest : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<Animator>().Play("Open");
        GetComponent<AudioSource>().Play();

        Invoke("EndLevel", 15f);
    }

    public void EndLevel()
    {
        SceneManager.LoadScene(0);
    }
}
