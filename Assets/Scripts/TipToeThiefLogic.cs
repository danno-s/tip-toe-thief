using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipToeThiefLogic : MonoBehaviour {

  public Camera GameCamera;
  public float normalClipPlane, contrastClipPlane;
	private bool lightsOn;

	// Use this for initialization
	void Start () {
		lightsOn = true;
	}

  /**
   *  Toggles the state of the lights. If the lights turned off or on,
   *  then a black filter is applied on the game, to hide its entities.
   **/
  public void ToggleLights() {
		lightsOn = !lightsOn;

    if(lightsOn)
      UnBlockCamera();
    else
      BlockCamera();
	}

  private void BlockCamera() {
    GameCamera.farClipPlane = contrastClipPlane;
  }

  private void UnBlockCamera() {
    GameCamera.farClipPlane = normalClipPlane;
  }

  public bool IsLightOn() {
		return lightsOn;
	}
}
