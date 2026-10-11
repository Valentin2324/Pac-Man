using UnityEngine;

public class Clyde : MonoBehaviour, IChaseTarget {

  private const float CLYDE_DANGER_ZONE_RADIUS = 8f;

  [SerializeField] private Transform scatterTarget;

  private bool isInPacManZone = false;

  private void Start() {
    scatterTarget = LevelReferences.Instance.GetClydeScatterTarget();
  }

  private void Update() {
    Vector3 toPacMan = PacMan.Instance.GetPosition() - transform.position;
    float distanceToPacMan = toPacMan.x * toPacMan.x + toPacMan.y * toPacMan.y;
    float radius = CLYDE_DANGER_ZONE_RADIUS * CLYDE_DANGER_ZONE_RADIUS;
    
    if (distanceToPacMan < radius) {
      isInPacManZone = true;
    }
    else {
      isInPacManZone = false;
    }
  }

  public Transform GetChaseTarget() {
    return isInPacManZone ? scatterTarget : PacMan.Instance.GetTransform();
  }
}
