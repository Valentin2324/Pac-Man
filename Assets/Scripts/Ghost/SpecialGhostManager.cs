using System.Collections.Generic;
using UnityEngine;

public class SpecialGhostManager : MonoBehaviour {

  public static SpecialGhostManager Instance { get; private set; }
  
  [SerializeField] private Transform specialGhostSpawnTransform;
  [SerializeField] private GameObject specialGhostPrefab;

  private List<Wasp> specialGhosts;

  private void Awake() {
    Instance = this;
    specialGhosts = new List<Wasp>();
  }

  private void Start() {
    GameObject instanceOfspecialGhost = Instantiate(specialGhostPrefab, specialGhostSpawnTransform.position, Quaternion.identity);

    specialGhosts.Add(instanceOfspecialGhost.GetComponent<Wasp>());
  }

  private void Update() {
    foreach (Wasp specialGhost in specialGhosts) {
      if (specialGhost.GetPosition().x > GameManager.Instance.GetMazeMapBound() || specialGhost.GetPosition().x < -GameManager.Instance.GetMazeMapBound()) {
        Teleport(specialGhost);
      }
    }
  }

  private void Teleport(Wasp specialGhost) {
    bool wasTeleported = false;
    Vector3 currentPosition = specialGhost.transform.position;

    if (currentPosition.x < -GameManager.Instance.GetMazeMapBound()) {
      currentPosition.x = GameManager.Instance.GetMazeMapBound();
      wasTeleported = true;
    }
    else if (currentPosition.x > GameManager.Instance.GetMazeMapBound()) {
      currentPosition.x = -GameManager.Instance.GetMazeMapBound();
      wasTeleported = true;
    }
    
    specialGhost.SetPosition(currentPosition);
    if (wasTeleported) {
      specialGhost.SetTargetPosition(specialGhost.GetPosition()); 
    }
  }

  public void ResetStates() {
    foreach (Wasp wasp in specialGhosts) {
      wasp.SetState(Wasp.State.TargetPowerPalette);

      if (PaletteManager.Instance.GetPowerPaletteMarkers().Count <= 0) wasp.SetCanLeaveHouse(false);
      else if (wasp.GetCanLeaveHouse()) wasp.SetCanLeaveHouse(true);
      
      wasp.SetIsInsideHouse(true);
    }
  }

  public List<Wasp> GetSpecialGhosts() {
    return specialGhosts;
  }
}
