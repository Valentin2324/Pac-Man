using UnityEngine;

public class Inky : MonoBehaviour, IChaseTarget {
  
  private const float TWO_TILES_AHEAD_OF_PACMAN = 2f;

  [SerializeField] private Transform chaseTarget;
  [SerializeField] private Blinky blinky;

  private void Start() {
    chaseTarget = LevelReferences.Instance.GetInkyChaseTarget();
  }

  private void Update() {
    Vector3 targetOffset = PacMan.Instance.GetCurrentDirection() * TWO_TILES_AHEAD_OF_PACMAN;
    Vector3 pacManTileTarget = PacMan.Instance.GetPosition() + targetOffset;

    Vector3 blinkyToTileTarget = pacManTileTarget - blinky.GetPosition();
    Vector3 inkyTarget = blinky.GetPosition() + blinkyToTileTarget * 2f;

    chaseTarget.position = inkyTarget;
  }

  public Transform GetChaseTarget() {
    return chaseTarget;
  }
}
