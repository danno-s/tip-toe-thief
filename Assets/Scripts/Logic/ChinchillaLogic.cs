﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class ChinchillaLogic : MonoBehaviour, TipToeThiefResettableObject {

    // Game Objects Begin
    public TipToeThiefLogic GameLogic;
    public Grid GameGrid;
    public Tilemap PlayerLayer;
    // Game Objects End

    // Prefabs Begin
    public GameObject pebblePrefab,
                      alarmPrefab;
    // Prefabs End

    // Assets Begin
    public AudioClip leftStepOn,
                     rightStepOn,
                     leftStepOff,
                     rightStepOff;
    // Assets End

    // Settings Begin
    public float speed,
                 throwingSpeed;
    public int postThrowPausedFrames,
               lives;
    // Settings End

    // Debug Settings Begin
    public int debugStage;
    // Debug Settings End

    private Rigidbody2D rgbd;
    private AudioSource audiosrc;
    private bool aimingPebble,
                 postThrowPause,
                 movedLastFrame;
    private int frameCount;
    private Direction lastMovedDirection;
    private Animator anim;
    private AlarmLogic activeAlarm;
    private PebbleLogic activePebble;

    public void Reset()
    {
        if (activeAlarm != null)
            Destroy(activeAlarm.gameObject);
        if (activePebble != null)
            Destroy(activePebble.gameObject);
    }

    public void PlayLeftStep()
    {
        if (GameLogic.IsLightOn())
            audiosrc.clip = leftStepOn;
        else
            audiosrc.clip = leftStepOff;
        audiosrc.Play();
    }

    public void PlayRightStep()
    {
        if (GameLogic.IsLightOn())
            audiosrc.clip = rightStepOn;
        else
            audiosrc.clip = rightStepOff;
        audiosrc.Play();
    }

    // Use this for initialization
    void Start() {
        rgbd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audiosrc = GetComponent<AudioSource>();
        aimingPebble = false;
        frameCount = 0;
        lastMovedDirection = Direction.Down;
        GameLogic.SetLives(lives);
#if UNITY_EDITOR
        GameLogic.SetStage(debugStage);
#endif
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
            GameLogic.ToggleLights();

        Direction movedDirection = lastMovedDirection;
        bool movedThisFrame = false;

        if(!aimingPebble && !postThrowPause) {
            Vector2 vel = new Vector2(Input.GetAxis("Horizontal") * speed,
                                        Input.GetAxis("Vertical") * speed);
            rgbd.velocity = vel;


            if (vel.magnitude != 0) {
                // Ángulo entre la horizontal y la velocidad (-180 a 180)
                float angle = Vector2.SignedAngle(Vector2.right, vel);

                if(angle > -135 && angle < -45)
                    movedDirection = Direction.Down;
                else if(angle > -45 && angle < 45)
                    movedDirection = Direction.Right;
                else if(angle > 45 && angle < 135)
                    movedDirection = Direction.Up;
                else
                    movedDirection = Direction.Left;

                movedThisFrame = true;
            }
        }

        if(rgbd.velocity.magnitude != 0)
            movedThisFrame = true;

        if(postThrowPause && !Input.anyKey) {
            postThrowPause = false;
            frameCount = 0;
        } else if(postThrowPause) {
            frameCount++;
            if(frameCount >= postThrowPausedFrames) {
                postThrowPause = false;
                frameCount = 0;
            }
        }

        if(Input.GetKeyDown(KeyCode.X)) {
            aimingPebble = true;
            rgbd.velocity = Vector2.zero;
        }

        // Lanzar piedra
        if(Input.GetKeyUp(KeyCode.X) && aimingPebble) {
            if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
                GameObject pebbleInstance = Instantiate(pebblePrefab, transform);
                Rigidbody2D pebbleRgbd = pebbleInstance.GetComponent<Rigidbody2D>();
                pebbleRgbd.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * throwingSpeed,
                                                  Input.GetAxisRaw("Vertical") * throwingSpeed);

                activePebble = pebbleInstance.GetComponent<PebbleLogic>();
                Destroy(pebbleInstance, 15f);
                postThrowPause = true;
            }

            aimingPebble = false;
        }

        // Activar alarma
        if(Input.GetKeyDown(KeyCode.Z)) {
            GameObject alarmInstance = Instantiate(alarmPrefab);
            alarmInstance.transform.position = transform.position;
            activeAlarm = alarmInstance.GetComponent<AlarmLogic>();
        }

        string clipName= anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        if(movedThisFrame != movedLastFrame || movedDirection != lastMovedDirection) {
            if (movedThisFrame) {
                switch(movedDirection) {
                    case Direction.Down:
                        if(clipName != "ChinchillaMoveDown")
                            anim.Play("ChinchillaMoveDown");
                        break;
                    case Direction.Right:
                        if(clipName != "ChinchillaMoveRight")
                            anim.Play("ChinchillaMoveRight");
                        break;
                    case Direction.Up:
                        if(clipName != "ChinchillaMoveUp")
                            anim.Play("ChinchillaMoveUp");
                        break;
                    case Direction.Left:
                        if(clipName != "ChinchillaMoveLeft")
                            anim.Play("ChinchillaMoveLeft");
                        break;
                }
            } else {
                switch(movedDirection) {
                    case Direction.Down:
                        if(clipName != "ChinchillaIdleDown")
                            anim.Play("ChinchillaIdleDown");
                        break;
                    case Direction.Right:
                        if(clipName != "ChinchillaIdleRight")
                            anim.Play("ChinchillaIdleRight");
                        break;
                    case Direction.Up:
                        if(clipName != "ChinchillaIdleUp")
                            anim.Play("ChinchillaIdleUp");
                        break;
                    case Direction.Left:
                        if(clipName != "ChinchillaIdleLeft")
                            anim.Play("ChinchillaIdleLeft");
                        break;
                }
            }

            lastMovedDirection = movedDirection;
            movedLastFrame = movedThisFrame;
        }

    }
}