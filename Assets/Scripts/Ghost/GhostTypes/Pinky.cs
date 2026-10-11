using UnityEngine;

public class Pinky : MonoBehaviour, IChaseTarget {

  private const float FOUR_TILES_AHEAD_OF_PACMAN = 4f;

  [SerializeField] private Transform chaseTarget;

  private void Start() {
    chaseTarget = LevelReferences.Instance.GetPinkyChaseTarget();
  }

  private void Update() {
    CalculateTarget();
  }

  private void CalculateTarget() {
    Vector3 currentOffset = PacMan.Instance.GetCurrentDirection() * FOUR_TILES_AHEAD_OF_PACMAN;
    chaseTarget.position = PacMan.Instance.GetPosition() + currentOffset;
  }

  public Transform GetChaseTarget() {
    return chaseTarget;
  }
}
