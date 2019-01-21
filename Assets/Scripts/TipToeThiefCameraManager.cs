using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(ChinchillaLogic))]
public class TipToeThiefCameraManager : MonoBehaviour {
    public TipToeThiefLogic gameLogic;
    public Material material;
    public ChinchillaLogic player;
    public float moveSpeed;
    private float contrast = 1f;

    public float Contrast {
        get {
            return contrast;
        }

        set {
            contrast = value;
        }
    }

    private void Start()
    {
        transform.position = player.transform.position + Vector3.back * 10;

    }

    private void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position, 
            player.transform.position + Vector3.back * 10, 
            Time.deltaTime * moveSpeed
        );
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        material.SetFloat("_Contrast", Contrast);
        Graphics.Blit(source, destination, material);
    }
}
