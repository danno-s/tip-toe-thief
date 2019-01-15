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

  // Settings Begin
  public float speed;
  // Settings End

  // Debug Settings Begin
  public bool useIntegerMovement;
  // Debug Settings End

  private Rigidbody2D rgbd;
  private Vector3 centerOffset;
  private Vector3Int cellPos;

	// Use this for initialization
	void Start () {
    centerOffset = new Vector3(0.5f, 0.5f, 0);
    cellPos = GameGrid.WorldToCell(transform.position);

    rgbd = GetComponent<Rigidbody2D>();
  }
	
	// Update is called once per frame
	void Update () {
    if(Input.GetKeyDown(KeyCode.Space))
      GameLogic.ToggleLights();


    if (useIntegerMovement) {
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

  private void OnDrawGizmos () {
    Gizmos.color = Color.magenta;
    Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    Debug.Log(collision);
  }
}
