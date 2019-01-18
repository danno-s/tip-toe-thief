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
  private TipToeThiefPostProcessing cameraPostProcessing;
  private Coroutine currentCoroutine;
	private bool lightsOn;

	// Use this for initialization
	void Start () {
		lightsOn = true;
    cameraPostProcessing = gameCamera.GetComponent<TipToeThiefPostProcessing>();
	}

  /**
   *  Toggles the state of the lights. If the lights turned off or on,
   *  then a black filter is applied on the game, to hide its entities.
   **/
  public void ToggleLights() {
    if (currentCoroutine != null)
		  StopCoroutine(currentCoroutine);

    if(lightsOn)
      currentCoroutine = StartCoroutine("BlockCamera");
    else
      currentCoroutine = StartCoroutine("UnBlockCamera");
  }

  public void RestartLevel() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  private IEnumerator BlockCamera() {
    while(cameraPostProcessing.Contrast < contrastMaximum) {
      cameraPostProcessing.Contrast =
        Mathf.Min(
          cameraPostProcessing.Contrast + cameraPostProcessing.Contrast * contrastFactor * Time.deltaTime,
          contrastMaximum
        );

      yield return null;
    }

    lightsOn = false;
  }

  private IEnumerator UnBlockCamera() {
    while(cameraPostProcessing.Contrast > contrastMinimum) {
      cameraPostProcessing.Contrast = 
        Mathf.Max(
          cameraPostProcessing.Contrast - cameraPostProcessing.Contrast * contrastFactor * Time.deltaTime,
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
