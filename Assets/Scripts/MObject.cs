﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MObject : MonoBehaviour {

    CountManager  GMS;
    private float rotateSpeed = 5f;

    void Awake()
    {
        GMS = GameObject.Find("GameManager").GetComponent<CountManager>();
        GMS.cur_objects++;
    }

    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.left * rotateSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("c");
            Destroy(gameObject);
            GMS.cur_objects--;
            GMS.UpdateUI();
        }
    }
    
}