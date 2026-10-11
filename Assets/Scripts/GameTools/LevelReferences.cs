using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelReferences : MonoBehaviour {

  // for prefabs that we cant assing in inspector
  
  public static LevelReferences Instance { get; private set; }

  [SerializeField] private Tilemap wallsTile;
  [SerializeField] private Tilemap doorTile;

  [SerializeField] private Transform blinkyHouseTargetTransform;
  [SerializeField] private Transform pinkyHouseTargetTransform;
  [SerializeField] private Transform inkyHouseTargetTransform;
  [SerializeField] private Transform clydeHouseTargetTransform;
  [SerializeField] private Transform isidaHouseTargetTransform;
  [SerializeField] private Transform waspHouseTargetTransform;

  [SerializeField] private Transform blinkyScatterTarget;
  [SerializeField] private Transform pinkyScatterTarget;
  [SerializeField] private Transform inkyScatterTarget;
  [SerializeField] private Transform clydeScatterTarget;
  [SerializeField] private Transform isidaScatterTarget;

  [SerializeField] private Transform pinkyChaseTarget;
  [SerializeField] private Transform inkyChaseTarget;

  [SerializeField] private Transform blinkyEatenTarget;
  [SerializeField] private Transform pinkyEatenTarget;
  [SerializeField] private Transform inkyEatenTarget;
  [SerializeField] private Transform clydeEatenTarget;
  [SerializeField] private Transform isidaEatenTarget;
  [SerializeField] private Transform waspEatenTarget;


  [SerializeField] private Transform pacManStartingPosition;

  private void Awake() {
    Instance = this;
  }

  public Tilemap GetWallsTile() {
    return wallsTile;
  }
  public Tilemap GetDoorTile() {
    return doorTile;
  }

  public Transform GetPacManStartingPosition() {
    return pacManStartingPosition;
  }

  public Transform GetBlinkyHouseTargetTransform() {
    return blinkyHouseTargetTransform;
  }
  public Transform GetPinkyHouseTargetTransform() {
    return pinkyHouseTargetTransform;
  }
  public Transform GetInkyHouseTargetTransform() {
    return inkyHouseTargetTransform;
  }
  public Transform GetClydeHouseTargetTransform() {
    return clydeHouseTargetTransform;
  }
  public Transform GetIsidaHouseTargetTransform() {
    return isidaHouseTargetTransform;
  }
  public Transform GetWaspHouseTargetTransform() {
    return waspHouseTargetTransform;
  }


  public Transform GetBlinkyEatenTarget() {
    return blinkyEatenTarget;
  }
  public Transform GetPinkyEatenTarget() {
    return pinkyEatenTarget;
  }
  public Transform GetInkyEatenTarget() {
    return inkyEatenTarget;
  }
  public Transform GetClydeEatenTarget() {
    return clydeEatenTarget;
  }
  public Transform GetIsidaEatenTarget() {
    return isidaEatenTarget;
  }
  public Transform GetWaspEatenTarget() {
    return waspEatenTarget;
  }

  public Transform GetBlinkyScatterTarget() {
    return blinkyScatterTarget;
  }
  public Transform GetPinkyScatterTarget() {
    return pinkyScatterTarget;
  }
  public Transform GetInkyScatterTarget() {
    return inkyScatterTarget;
  }
  public Transform GetClydeScatterTarget() {
    return clydeScatterTarget;
  }
  public Transform GetIsidaScatterTarget() {
    return isidaScatterTarget;
  }

  public Transform GetPinkyChaseTarget() {
    return pinkyChaseTarget;
  }
  public Transform GetInkyChaseTarget() {
    return inkyChaseTarget;
  }
}
