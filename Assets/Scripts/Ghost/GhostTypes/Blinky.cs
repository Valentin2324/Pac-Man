using System;
using UnityEngine;

public class Blinky : MonoBehaviour, IChaseTarget {

  public Transform GetChaseTarget() {
    return PacMan.Instance.GetTransform();
  }

  public Vector3 GetPosition() {
    return transform.position;
  }
}
