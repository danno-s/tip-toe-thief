using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuardPatrolType {
  None,
  BackAndForth,
  Cyclical
}

public enum GuardState {
  Waiting,
  RotatingTowards,
  Moving,
  LookingTowards,
  PlayerSpotted,
  Distracted
}

public enum Direction {
    Up,
    Down,
    Left, 
    Right
}