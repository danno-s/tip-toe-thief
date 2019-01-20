using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TipToeThiefLogic : MonoBehaviour {

    public Camera gameCamera;
    public float normalClipPlane,
                 contrastClipPlane,
                 contrastMaximum,
                 contrastMinimum,
                 contrastFactor;
    public List<GameObject> stages;
    private TipToeThiefCameraManager cameraManager;
    private Coroutine currentCoroutine;
    private bool lightsOn;
    private int currentStage;

    // Use this for initialization
    void Start() {
        lightsOn = true;
        cameraManager = gameCamera.GetComponent<TipToeThiefCameraManager>();
        currentStage = 0;
    }

    /**
     *  Toggles the state of the lights. If the lights turned off or on,
     *  then a black filter is applied on the game, to hide its entities.
     **/
    public void ToggleLights() {
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if(lightsOn)
            currentCoroutine = StartCoroutine("BlockCamera");
        else
            currentCoroutine = StartCoroutine("UnBlockCamera");
    }

    public void RestartLevel() {
        foreach(var tipToeThiefObject in
            stages[currentStage].GetComponentsInChildren<TipToeThiefResettableObject>())
        {
            tipToeThiefObject.Reset();
        }
    }

    private IEnumerator BlockCamera() {
        while(cameraManager.Contrast < contrastMaximum) {
            cameraManager.Contrast =
                Mathf.Min(
                    cameraManager.Contrast + cameraManager.Contrast * contrastFactor * Time.deltaTime,
                    contrastMaximum
                );

            yield return null;
        }

        lightsOn = false;
    }

    private IEnumerator UnBlockCamera() {
        while(cameraManager.Contrast > contrastMinimum) {
            cameraManager.Contrast =
              Mathf.Max(
                cameraManager.Contrast - cameraManager.Contrast * contrastFactor * Time.deltaTime,
                contrastMinimum
              );

            yield return null;
        }

        lightsOn = true;
    }

    public bool IsLightOn() {
        return lightsOn;
    }
}
