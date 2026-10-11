using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Wasp : MonoBehaviour {
  
  [SerializeField] private Transform eatenTileTarget;

  [SerializeField] private Transform houseExitTarget;
  [SerializeField] private float movementSpeed;
  [SerializeField] private Tilemap wallsTile;
  [SerializeField] private Tilemap doorTile;

  public enum State {
    TargetPowerPalette,
    Eaten,
    Home,
  }

  private State state;
  private Vector2 requestedDirection;
  private Vector2 currentDirection;
  private Vector2 targetPosition;
  private Vector2 startPosition;
  private bool canLeaveHouse = false;
  private bool isInsideHouse = true;
  private bool isHomeForever = false;
  private BoxCollider2D ghostCollider;

  private void Awake() {
    state = State.TargetPowerPalette;
    startPosition = transform.position;
    targetPosition = transform.position;

    ghostCollider = GetComponent<BoxCollider2D>();
  }

  private void Start() {
    eatenTileTarget = LevelReferences.Instance.GetWaspEatenTarget();
    wallsTile = LevelReferences.Instance.GetWallsTile();
    doorTile = LevelReferences.Instance.GetDoorTile();
    houseExitTarget = LevelReferences.Instance.GetWaspHouseTargetTransform();

    PacMan.Instance.OnPowerUp += PacMan_OnPowerUp;
  }

  private void PacMan_OnPowerUp(object sender, EventArgs e) {
    canLeaveHouse = true;
  }

  private void Update() {
    if (GameManager.Instance.GetGameState() != GameManager.GameState.GameRunning || PacMan.Instance.GetState() == PacMan.State.Dead) return;
    if (state != State.Eaten) ghostCollider.enabled = true;

    DirectionManager();

    switch (state) {
      case State.TargetPowerPalette: 
        break;
      case State.Eaten:
        ghostCollider.enabled = false;
        break;
      case State.Home:
        ghostCollider.enabled = false;
        break;
    }
  }

  private void DirectionManager() {
    Vector2 newPosition;

    if (state == State.Eaten || (state == State.Home && !isHomeForever)) {
      float multiplierSpeed = 3f;
      newPosition = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * multiplierSpeed * Time.deltaTime);
    }
    else {
      newPosition = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);  
    }

    if (newPosition == targetPosition) {
      Vector3Int currentCell = wallsTile.WorldToCell(newPosition);

      UpdateInsideHouse(currentCell);
      ChooseState(currentCell);

      if (requestedDirection != Vector2.zero) {
        currentDirection = requestedDirection;
      }

      Vector3Int targetPositionTile = currentCell + new Vector3Int((int)currentDirection.x, (int)currentDirection.y, 0);
      bool doesTargetPositionHasWall = wallsTile.HasTile(targetPositionTile);

      if (!doesTargetPositionHasWall) {
        targetPosition = wallsTile.CellToWorld(targetPositionTile) + wallsTile.cellSize / 2f;
      }
      else {
        targetPosition = transform.position;
      }

    }

      transform.position = newPosition;
  }

  private void ChooseState(Vector3Int currentCell) {
    if (isInsideHouse && state != State.Eaten && canLeaveHouse) {
      FindBestPathToTarget(currentCell, houseExitTarget);
      return;
    }

    if (state == State.TargetPowerPalette) {
      if (PaletteManager.Instance.GetPowerPaletteMarkers().Count <= 0) {
        state = State.Home;
        return;
      }

      Transform chosenTarget = ChoosePowerPaletteToEat();

      FindBestPathToTarget(currentCell, chosenTarget);
    }
    else if (state == State.Eaten) {
      FindBestPathToTarget(currentCell, eatenTileTarget);
    }
    else if (state == State.Home) {
      FindBestPathToTarget(currentCell, eatenTileTarget);
    }
  }

  private Transform ChoosePowerPaletteToEat() {
    List<Transform> powerPalettesList = PaletteManager.Instance.GetPowerPaletteMarkers();

    float minimumDistance = float.MaxValue;
    Transform chosenTarget = null;

    foreach (Transform powerPalette in powerPalettesList) {
      float distance = Mathf.Pow(transform.position.x - powerPalette.position.x, 2) + Mathf.Pow(transform.position.y - powerPalette.position.y, 2);

      if (minimumDistance > distance) {
        minimumDistance = distance;
        chosenTarget = powerPalette;
      }
    }

    return chosenTarget;
  }

  private void FindBestPathToTarget(Vector3Int currentCell, Transform target) {
    List<(Vector3Int direction, Vector2 position)> candidatePath = new List<(Vector3Int direction, Vector2 position)>();
    Vector3Int reverseDirection = new Vector3Int(-(int)currentDirection.x, -(int)currentDirection.y);

    List<Vector3Int> validDirections = GetValidDirections(currentCell);

    foreach (Vector3Int direction in validDirections) {
      Vector3Int cell = currentCell + direction;
      Vector2 position = wallsTile.CellToWorld(cell) + wallsTile.cellSize / 2f;
      candidatePath.Add((direction, position));
    }

    float minimumDistance = 0f;
    Vector3Int chosenDirection = Vector3Int.zero;

    for (int i = 0; i < candidatePath.Count; i++) {
      float distance = Mathf.Pow(candidatePath[i].position.x - target.position.x, 2) + Mathf.Pow(candidatePath[i].position.y - target.position.y, 2);

      if (minimumDistance == 0f) {
        minimumDistance = distance;
        chosenDirection = candidatePath[i].direction;
      }
      else if (minimumDistance == distance) {
        if (candidatePath[i].direction == Vector3Int.up) {
          chosenDirection = candidatePath[i].direction;
        }
        else if (candidatePath[i].direction == Vector3Int.left && chosenDirection != Vector3Int.up) {
          chosenDirection = candidatePath[i].direction;
        }
        else if (candidatePath[i].direction == Vector3Int.down && chosenDirection != Vector3Int.up && chosenDirection != Vector3Int.left) {
          chosenDirection = candidatePath[i].direction;
        }
        else if (candidatePath[i].direction == Vector3Int.right) {
          continue;
        }
      }
      else if (minimumDistance > distance) {
        minimumDistance = distance;
        chosenDirection = candidatePath[i].direction;
      }
    }

    if (candidatePath.Count == 0) {
      requestedDirection = new Vector2(reverseDirection.x, reverseDirection.y);
    }
    else {
      requestedDirection = new Vector2(chosenDirection.x, chosenDirection.y);
    }
  }

  private bool IsBlocked(Vector3Int cell) {
    if (wallsTile.HasTile(cell)) return true;
    if (doorTile.HasTile(cell)) return !(isInsideHouse || state == State.Eaten || state == State.Home);
    return false;
  }

  private void UpdateInsideHouse(Vector3Int currentCell) {
    if (state == State.Home && doorTile.HasTile(currentCell + Vector3Int.up)) {
      isHomeForever = true;
      return;
    }

    if (isInsideHouse && doorTile.HasTile(currentCell)) {
      isInsideHouse = false;
    }
    else if (!isInsideHouse && state == State.Eaten && doorTile.HasTile(currentCell + Vector3Int.up)) {
      isInsideHouse = true;
      state = State.TargetPowerPalette;
    }
  }

  private List<Vector3Int> GetValidDirections(Vector3Int currentCell) {
    List<Vector3Int> path = new List<Vector3Int>(); 
    Vector3Int reverseDirection = new Vector3Int(-(int)currentDirection.x, -(int)currentDirection.y);

    Vector3Int topCell = currentCell + Vector3Int.up;
    Vector3Int leftCell = currentCell + Vector3Int.left;
    Vector3Int bottomCell = currentCell + Vector3Int.down;
    Vector3Int rightCell = currentCell + Vector3Int.right;

    if (!IsBlocked(topCell) && Vector3Int.up != reverseDirection) path.Add(Vector3Int.up);
    if (!IsBlocked(leftCell) && Vector3Int.left != reverseDirection) path.Add(Vector3Int.left);
    if (!IsBlocked(bottomCell) && Vector3Int.down != reverseDirection) path.Add(Vector3Int.down);
    if (!IsBlocked(rightCell) && Vector3Int.right != reverseDirection) path.Add(Vector3Int.right);

    return path;
  }

  public Vector2 GetStartPosition() {
    return startPosition;
  }
  public Vector2 GetPosition() {
    return transform.position;
  }
  public void SetPosition(Vector2 newPosition) {
    transform.position = newPosition;
  }
  public void SetTargetPosition(Vector2 newPosition) {
    targetPosition = newPosition;
  }
  public Vector2 GetCurrentDirection() {
    return currentDirection;
  }
  public State GetState() {
    return state;
  }
  public void SetState(State newState) {
    state = newState;
  }

  public void SetCanLeaveHouse(bool newState) {
    canLeaveHouse = newState;
  }
  public bool GetCanLeaveHouse() {
    return canLeaveHouse;
  }
  public void SetIsInsideHouse(bool newState) {
    isInsideHouse = newState;
  }

  public bool GetIsHomeForever() {
    return isHomeForever;
  }
}
