using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    public Animator animator;
    public string sceneName;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(sceneName);

        }
    }

    /* private void Update()
     {
         if (Input.GetKeyDown(KeyCode.Space))
         {
             SceneManager.LoadScene(sceneName);
         }
     }*/

    IEnumerator LoadScene()
    {
        animator.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
    }

     




}
