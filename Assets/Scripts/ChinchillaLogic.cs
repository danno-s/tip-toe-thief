using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class ChinchillaLogic : MonoBehaviour {

  // Game Objects Begin
	public TipToeThiefLogic GameLogic;
	public Grid GameGrid;
  public Tilemap PlayerLayer;
  // Game Objects End

  // Prefabs Begin
  public GameObject pebblePrefab,
                    alarmPrefab;
  // Prefabs End

  // Settings Begin
  public float speed,
               throwingSpeed;
  public int postThrowPausedFrames;
  // Settings End

  // Debug Settings Begin
  public bool useIntegerMovement;
  // Debug Settings End

  private Rigidbody2D rgbd;
  private Vector3 centerOffset;
  private Vector3Int cellPos;
  private bool aimingPebble,
               postThrowPause;
  private int frameCount;

	// Use this for initialization
	void Start () {
    centerOffset = new Vector3(0.5f, 0.5f, 0);
    cellPos = GameGrid.WorldToCell(transform.position);

    rgbd = GetComponent<Rigidbody2D>();
    aimingPebble = false;
    frameCount = 0;
  }
	
	// Update is called once per frame
	void Update () {
    if(Input.GetKeyDown(KeyCode.Space))
      GameLogic.ToggleLights();

    if(!aimingPebble && !postThrowPause) {
      if(useIntegerMovement) {
        var oldCellPos = cellPos;

        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
          cellPos += Vector3Int.left;

        if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
          cellPos += Vector3Int.right;

        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
          cellPos += Vector3Int.up;

        if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
          cellPos += Vector3Int.down;

        // Check if new position is valid.
        if(!PlayerLayer.GetTile(cellPos)) {
          transform.position = GameGrid.CellToWorld(cellPos) + centerOffset;
        } else {
          cellPos = oldCellPos;
        }
      } else {
        rgbd.velocity = new Vector2(Input.GetAxis("Horizontal") * speed,
                                    Input.GetAxis("Vertical") * speed);
      }
    }

    if(postThrowPause && !Input.anyKey) {
      postThrowPause = false;
      frameCount = 0;
    } else if (postThrowPause) {
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

    if(Input.GetKeyUp(KeyCode.X) && aimingPebble) {
      if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
        Debug.Log("Pebble thrown");
        GameObject pebbleInstance = Instantiate(pebblePrefab, transform);
        Rigidbody2D pebbleRgbd = pebbleInstance.GetComponent<Rigidbody2D>();
        pebbleRgbd.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * throwingSpeed,
                                          Input.GetAxisRaw("Vertical") * throwingSpeed);
        Destroy(pebbleInstance, 15f);
        postThrowPause = true;
      }

      aimingPebble = false;
    }

    if(Input.GetKeyDown(KeyCode.Z)) {
      GameObject alarmInstance = Instantiate(alarmPrefab);
      alarmInstance.transform.position = transform.position;
    }
  }

  private void OnDrawGizmos () {
    Gizmos.color = Color.magenta;
    Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
  }
}
