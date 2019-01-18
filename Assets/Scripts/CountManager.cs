using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CountManager : MonoBehaviour
{
    public int cur_objects = 0;
    public int max_objects = 0;

    public int cur_llaves = 0;
    public int max_llaves = 0;

    public GameObject Door;
    public GameObject SecondDoor;

    // Use this for initialization
    void Start()
    {
        Door.SetActive(false);
        SecondDoor.SetActive(true);
        max_objects = cur_objects;
        max_llaves = cur_llaves;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        //Door = GameObject.Find("Door");
    }
    public void UpdateUI()
    {
        if (cur_objects <= 0)
        {
            Door.SetActive(true);
        }


        if (cur_llaves <= 0)
        {
            SecondDoor.SetActive(false);
        }

    }
}
