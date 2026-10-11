using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Ghost : MonoBehaviour  {

  [SerializeField] private Transform scatterTileTarget;
  [SerializeField] private Transform eatenTileTarget;

  [SerializeField] private Transform houseExitTarget;
  [SerializeField] private float movementSpeed;
  [SerializeField] private Tilemap wallsTile;
  [SerializeField] private Tilemap doorTile;

  public enum State {
    Chase,
    Scatter,
    Frightened,
    Eaten
  }

  private State state;
  private BoxCollider2D boxCollider2D;
  private Vector2 requestedDirection;
  private Vector2 currentDirection;
  private Vector2 targetPosition;
  private Vector2 startPosition;
  private Coroutine frightenedRoutine;
  private bool canLeaveHouse = false;
  private bool isInsideHouse = true;
  private bool passedDoorWhileEaten = false;
  private float originalMovementSpeed;

  private IChaseTarget chaseTarget;

  private void Awake() {
    state = State.Scatter;
    startPosition = transform.position;
    
    targetPosition = transform.position;
    originalMovementSpeed = movementSpeed;

    chaseTarget = GetComponent<IChaseTarget>();
    boxCollider2D = GetComponent<BoxCollider2D>();
  }

  private void Start() {
    wallsTile = LevelReferences.Instance.GetWallsTile();
    doorTile = LevelReferences.Instance.GetDoorTile();

    if (chaseTarget is Blinky) {
      houseExitTarget = LevelReferences.Instance.GetBlinkyHouseTargetTransform();
      scatterTileTarget = LevelReferences.Instance.GetBlinkyScatterTarget();
      eatenTileTarget = LevelReferences.Instance.GetBlinkyEatenTarget();
    }
    if (chaseTarget is Pinky) {
      houseExitTarget = LevelReferences.Instance.GetPinkyHouseTargetTransform();
      scatterTileTarget = LevelReferences.Instance.GetPinkyScatterTarget();
      eatenTileTarget = LevelReferences.Instance.GetPinkyEatenTarget();
    } 
    if (chaseTarget is Inky) {
      houseExitTarget = LevelReferences.Instance.GetInkyHouseTargetTransform();
      scatterTileTarget = LevelReferences.Instance.GetInkyScatterTarget();
      eatenTileTarget = LevelReferences.Instance.GetInkyEatenTarget();
    }
    if (chaseTarget is Clyde) {
      houseExitTarget = LevelReferences.Instance.GetClydeHouseTargetTransform();
      scatterTileTarget = LevelReferences.Instance.GetClydeScatterTarget();
      eatenTileTarget = LevelReferences.Instance.GetClydeEatenTarget();
    }
    if (chaseTarget is Isida) {
      houseExitTarget = LevelReferences.Instance.GetIsidaHouseTargetTransform();
      scatterTileTarget = LevelReferences.Instance.GetIsidaScatterTarget();
      eatenTileTarget = LevelReferences.Instance.GetIsidaEatenTarget();
    }

    GhostManager.Instance.OnChaseMode += GhostManager_OnChaseMode;
    GhostManager.Instance.OnFrightenedMode += GhostManager_OnFrightenedMode;
  }

  private void GhostManager_OnChaseMode(object sender, GhostManager.OnChaseModeEventArgs e) {
    if (chaseTarget is Blinky && e.isBlinkyEnraged) return;
    ReverseCurrentDirection();
  }

  private void GhostManager_OnFrightenedMode(object sender, GhostManager.OnFrightenedModeEventArgs e) {
    if (chaseTarget is Blinky && e.isBlinkyEnraged) return;

    ReverseCurrentDirection();

    if (frightenedRoutine != null) {
      StopCoroutine(frightenedRoutine);
      movementSpeed = originalMovementSpeed;
    }
    frightenedRoutine = StartCoroutine(FrightenedModeDuration(e));
  }

  private IEnumerator FrightenedModeDuration(GhostManager.OnFrightenedModeEventArgs e) {
    float slowMovementDebuff = 1.5f;
    movementSpeed /= slowMovementDebuff;

    yield return new WaitForSeconds(e.frightenedModeDuration);

    movementSpeed = originalMovementSpeed;
  }

  

  private void Update() {
    if (GameManager.Instance.GetGameState() != GameManager.GameState.GameRunning || PacMan.Instance.GetState() == PacMan.State.Dead) return;
    if (state != State.Eaten) boxCollider2D.enabled = true;

    DirectionManager();

    switch (state) {
      case State.Chase: 
        break;
      case State.Scatter:
        break;
      case State.Frightened: 
        break;
      case State.Eaten:
        boxCollider2D.enabled = false;
        break;
    }
  }

  private void DirectionManager() {
    Vector2 newPosition;

    if (state == State.Eaten) {
      float multiplierSpeed = 3f;
      newPosition = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * multiplierSpeed * Time.deltaTime);
    }
    else {
      newPosition = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);  
    }

    if (newPosition == targetPosition) {
      //world to cell din vector3 -> vector3int
      Vector3Int currentCell = wallsTile.WorldToCell(newPosition);

      UpdateInsideHouse(currentCell);
      ChooseState(currentCell);

      if (requestedDirection != Vector2.zero) {
        currentDirection = requestedDirection;
      }

      Vector3Int targetPositionTile = currentCell + new Vector3Int((int)currentDirection.x, (int)currentDirection.y, 0);
      bool doesTargetPositionHasWall = wallsTile.HasTile(targetPositionTile);

      if (!doesTargetPositionHasWall) {
        //cell to world din vector3int -> vector3 (returns corners, but with the division we can get the center)
        targetPosition = wallsTile.CellToWorld(targetPositionTile) + wallsTile.cellSize / 2f;
      }
      else {
        // wall
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

    if (state == State.Scatter) {
      FindBestPathToTarget(currentCell, scatterTileTarget);
    }
    else if (state == State.Chase) {
      FindBestPathToTarget(currentCell, chaseTarget.GetChaseTarget());
    }
    else if (state == State.Frightened) {
      if (isInsideHouse) FindBestPathToTarget(currentCell, scatterTileTarget);
      else FindRandomDirectionToEscape(currentCell);
    }
    else if (state == State.Eaten) {
      FindBestPathToTarget(currentCell, eatenTileTarget);
    }
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

  private void FindRandomDirectionToEscape(Vector3Int currentCell) {
    List<Vector3Int> randomDirection;
    Vector3Int reverseDirection = new Vector3Int(-(int)currentDirection.x, -(int)currentDirection.y);

    randomDirection = GetValidDirections(currentCell);

    if (randomDirection.Count == 0) {
      requestedDirection = new Vector2(reverseDirection.x, reverseDirection.y);
    }
    else {
      int roll = Random.Range(0, randomDirection.Count);
      Vector3Int chosenDirection = randomDirection[roll];

      requestedDirection = new Vector2(chosenDirection.x, chosenDirection.y);
    }
  }

  private bool IsBlocked(Vector3Int cell) {
    if (wallsTile.HasTile(cell)) return true;
    if (doorTile.HasTile(cell)) {
      bool mayPass = (isInsideHouse && canLeaveHouse) || state == State.Eaten;
      return !mayPass;
    }
    return false;
  }

  private void UpdateInsideHouse(Vector3Int currentCell) {

    if (isInsideHouse && doorTile.HasTile(currentCell)) {
      isInsideHouse = false;
      return;
    }

    if (state != State.Eaten || isInsideHouse) return;

    if (doorTile.HasTile(currentCell)) {
      passedDoorWhileEaten = true;
    }
    else if (passedDoorWhileEaten) {
      passedDoorWhileEaten = false;
      isInsideHouse = true;
      state = GhostManager.Instance.GetBaseState();

      if (frightenedRoutine != null) {
        SetOriginalSpeed();
      }
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

  private void ReverseCurrentDirection() {
    if (isInsideHouse) return;
    
    Vector3Int reverseDirection = new Vector3Int(-(int)currentDirection.x, -(int)currentDirection.y);
    currentDirection = new Vector2(reverseDirection.x, reverseDirection.y);
  }

  public Transform GetTransform() {
    return transform;
  }
  public Vector3 GetPosition() {
    return transform.position;
  }
  public Vector3 GetStartPosition() {
    return startPosition;
  }
  public void SetPosition(Vector3 newPosition) {
    transform.position = newPosition;
  }
  public void SetTargetPosition(Vector2 newPosition) {
    targetPosition = newPosition;
  }
  public Vector2 GetCurrentDirection() {
    return currentDirection;
  }
  public void SetOriginalSpeed() {
    StopCoroutine(frightenedRoutine);
    movementSpeed = originalMovementSpeed;
  }

  public void SetIsInsideHouse(bool newState) {
    isInsideHouse = newState;
  }
  public void SetCanLeaveHouse(bool newState) {
    canLeaveHouse = newState;
  }
  public void SetPassedDoorWhileEaten(bool newState) {
    passedDoorWhileEaten = newState;
  }

  public State GetState() {
    return state;
  }
  public void SetState(State newState) {
    state = newState;
  }
}
