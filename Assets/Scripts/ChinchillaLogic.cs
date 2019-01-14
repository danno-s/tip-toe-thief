using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChinchillaLogic : MonoBehaviour {

	public TipTopThiefLogic GameLogic;
	public Grid GameGrid;
  private Vector3 centerOffset;
  private Vector3Int cellPos;

	// Use this for initialization
	void Start () {
    centerOffset = new Vector3(0.5f, 0.5f, 0);
    cellPos = GameGrid.WorldToCell(transform.position);
  }
	
	// Update is called once per frame
	void Update () {
    if(Input.GetKeyDown(KeyCode.Space))
      GameLogic.ToggleLights();

    if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
      cellPos += new Vector3Int(-1, 0, 0);

    if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
      cellPos += new Vector3Int(1, 0, 0);

    if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
      cellPos += new Vector3Int(0, 1, 0);

    if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
      cellPos += new Vector3Int(0, -1, 0);


    transform.position = GameGrid.CellToWorld(cellPos) + centerOffset;
  }

  private void OnDrawGizmos () {
    Gizmos.color = Color.magenta;
    Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
  }
}
