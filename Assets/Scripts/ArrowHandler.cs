using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArrowHandler : MonoBehaviour
{
    public float continueHeight;
    public float endHeight;

    private bool continueSelected = true;
    private float deltaY;
    private Animator anm;

    private void Start()
    {
        anm = GetComponentInParent<Animator>();
        deltaY = endHeight - continueHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if (continueSelected &&
            (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)))
        {
            transform.Translate(0, deltaY, 0);
            continueSelected = false;
        }


        if (!continueSelected &&
            (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            transform.Translate(0, -deltaY, 0);
            continueSelected = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine("ExecuteSelection");
    }

    internal IEnumerator ExecuteSelection()
    {
        anm.Play("Selected");
        yield return new WaitForSeconds(1f);
        if (continueSelected)
            SceneManager.LoadScene(1);
        else
            Application.Quit();
    }
}
