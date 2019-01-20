using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TipToeThiefCameraManager : MonoBehaviour {
    public TipToeThiefLogic gameLogic;
    public Material material;
    public float changeSpeed;
    private float contrast = 1f,
                  nextSize;
    private Transform nextTransform;
    private Camera handledCamera;

    public float Contrast {
        get {
            return contrast;
        }

        set {
            contrast = value;
        }
    }

    private void Start() {
        handledCamera = GetComponent<Camera>();
    }

    public void SetNewCameraPosition(Transform newTransform, float newSize) {
        nextTransform = newTransform;
        nextSize = newSize;
        StartCoroutine("MoveToNewPosition");
    }

    IEnumerator MoveToNewPosition() {
        while(transform.position != nextTransform.position) {
            float distanceToNext = (nextTransform.position - transform.position).magnitude;
            transform.position = Vector3.Lerp(transform.position, nextTransform.position, Time.deltaTime * changeSpeed);
            handledCamera.orthographicSize = Mathf.Lerp(handledCamera.orthographicSize, nextSize, Time.deltaTime * changeSpeed);
            yield return null;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        material.SetFloat("_Contrast", Contrast);
        Graphics.Blit(source, destination, material);
    }
}
