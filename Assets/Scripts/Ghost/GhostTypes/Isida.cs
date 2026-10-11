using UnityEngine;

public class Isida : MonoBehaviour, IChaseTarget {
  
  public Transform GetChaseTarget() {
    foreach (Ghost ghost in GhostManager.Instance.GetGhosts()) {
      if (ghost.GetComponent<Blinky>()) {
        return ghost.GetTransform();
      }
    }
    return null;
  }

  public Vector3 GetPosition() {
    return transform.position;
  }
}
