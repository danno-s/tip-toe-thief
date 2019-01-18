using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipToeThiefPostProcessing : MonoBehaviour {
  public TipToeThiefLogic gameLogic;
  public Material material;
  private float contrast = 1f;

  public float Contrast {
    get {
      return contrast;
    }

    set {
      contrast = value;
    }
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination) {
    material.SetFloat("_Contrast", Contrast);
    Graphics.Blit(source, destination, material);
  }
}
