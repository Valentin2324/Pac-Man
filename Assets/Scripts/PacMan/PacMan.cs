using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PacMan : MonoBehaviour {

  public static PacMan Instance { get; private set; }

  public event EventHandler OnEatPalette;
  public event EventHandler OnEatItem;
  public event EventHandler OnPowerUp;
  public event EventHandler OnDeath;
  public event EventHandler<OnGhostKillEventArgs> OnGhostKill;
  public class OnGhostKillEventArgs : EventArgs {
    public bool isNormalGhost;
  }

  public event EventHandler OnRespawn;
  public event EventHandler OnItemPickup;

  [SerializeField] private float movementSpeed;
  [SerializeField] private Tilemap wallsTile;
  [SerializeField] private Tilemap ghostDoor;
  [SerializeField] private Transform startPosition;

  public enum State {
    Normal,
    PoweredUp,
    Dead
  }

  private State state;
  private State previousState;
  private Vector2 requestedDirection;
  private Vector2 currentDirection;
  private Vector2 targetPosition;
  private bool isRespawning = true;

  private PacManVisual pacManVisual;
  private PacManScoreAmountCombo pacManScoreAmountCombo;
  private bool isMoving = false;
  private bool isItemTooked = false;
  private int scoreAmountCombo = 100;

  private void Awake() {
    Instance = this;
    pacManVisual = GetComponent<PacManVisual>();
    pacManScoreAmountCombo = GetComponentInChildren<PacManScoreAmountCombo>();

    state = State.Normal;
  }

  private void Update() {
    if (GameManager.Instance.GetGameState() != GameManager.GameState.GameRunning) return;

    if (transform.position.x > GameManager.Instance.GetMazeMapBound() || transform.position.x < -GameManager.Instance.GetMazeMapBound()) {
      Teleport();
    }

    switch (state) {
      case State.Normal:
      if (isRespawning) {
        OnRespawn?.Invoke(this, EventArgs.Empty);
        pacManVisual.ShowSprite();
        isRespawning = false;
      }
      
      RequestDirection();
      UpdateDirection();
        break;
      case State.Dead:
      if (previousState != State.Dead) {
        OnDeath?.Invoke(this, EventArgs.Empty);
        pacManVisual.HideSpriteAfterAnimation();
      }
        break;
    }

    previousState = state;
  }

  private void RequestDirection() {
    if (GameInput.Instance.IsUpActionPressed()) {
      requestedDirection = Vector2.up;
    }
    else if (GameInput.Instance.IsLeftActionPressed()) {
      requestedDirection = Vector2.left;
    }
    else if (GameInput.Instance.IsDownActionPressed()) {
      requestedDirection = Vector2.down;
    }
    else if (GameInput.Instance.IsRightActionPressed()) {
      requestedDirection = Vector2.right;
    }
  }

  private void UpdateDirection() {
    Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

    if (newPosition == targetPosition) {
      Vector3Int currentCell = wallsTile.WorldToCell(transform.position);

      Vector3Int requestedCell = new Vector3Int((int)requestedDirection.x, (int)requestedDirection.y, 0);
      Vector3Int candidateCell = currentCell + requestedCell;

      bool isWallInTheWay = wallsTile.HasTile(candidateCell);
      bool isGhostDoorInTheWay = ghostDoor.HasTile(candidateCell);
      if (requestedDirection != Vector2.zero && !isWallInTheWay && !isGhostDoorInTheWay) {
        currentDirection = requestedDirection;
        isMoving = true;
      }

      Vector3Int nextCell = currentCell + new Vector3Int((int)currentDirection.x, (int)currentDirection.y);
      bool isNextCellWall = wallsTile.HasTile(nextCell);
      bool isNextCellDoor = ghostDoor.HasTile(nextCell);
      if (!isNextCellWall && !isNextCellDoor) {
        targetPosition = wallsTile.CellToWorld(nextCell) + wallsTile.cellSize / 2f;
      }
      else {
        isMoving = false;
        targetPosition = transform.position;
      }
    }

    transform.position = newPosition;
  }

  private void Teleport() {
    bool wasTeleported = false;
    Vector3 currentPosition = transform.position;

    if (currentPosition.x < -GameManager.Instance.GetMazeMapBound()) {
      currentPosition.x = GameManager.Instance.GetMazeMapBound();
      wasTeleported = true;
    }
    else if (currentPosition.x > GameManager.Instance.GetMazeMapBound()) {
      currentPosition.x = -GameManager.Instance.GetMazeMapBound();
      wasTeleported = true;
    }
    
    transform.position = currentPosition;

    if (wasTeleported) {
      targetPosition = transform.position;
    }
  }

  private void OnTriggerEnter2D(Collider2D collider2D) {
    if (collider2D.gameObject.TryGetComponent(out Ghost ghost)) {
      if (ghost.GetState() != Ghost.State.Frightened && ghost.GetState() != Ghost.State.Eaten) {
        state = State.Dead;
        PacManLives.Instance.TakeLife();
        
        if (PacManLives.Instance.GetLives() > 0) {
          isRespawning = true;

          isMoving = false;
          currentDirection = Vector2.zero;
          requestedDirection = Vector2.zero;
        }
        else {
          StartCoroutine(WaitBeforeGameOver());
        }
      }
      else {
        int currentScore = scoreAmountCombo * 2;
        scoreAmountCombo = currentScore;

        OnGhostKill?.Invoke(this, new OnGhostKillEventArgs {
          isNormalGhost = true
        });
        ghost.SetState(Ghost.State.Eaten);

        ScoreUI.Instance.AddScore(scoreAmountCombo);
      }
    }
    else if (collider2D.gameObject.TryGetComponent(out Palette palette)) {
      OnEatPalette?.Invoke(this, EventArgs.Empty);

      PaletteManager.Instance.RemovePaletteFromList(palette.transform);
      palette.DestroySelf();

      ScoreUI.Instance.AddScore(50);
    }
    else if (collider2D.gameObject.TryGetComponent(out PowerPalette powerPalette)) {
      PaletteManager.Instance.RemovePowerPaletteFromList(powerPalette.transform);
      powerPalette.DestroySelf();

      scoreAmountCombo = 100;

      OnPowerUp?.Invoke(this, EventArgs.Empty);
    }
    else if (collider2D.gameObject.TryGetComponent(out Wasp wasp)) {

      OnGhostKill?.Invoke(this, new OnGhostKillEventArgs {
        isNormalGhost = false
      });
      wasp.SetState(Wasp.State.Eaten);
    }
    else if (collider2D.gameObject.TryGetComponent(out Item item)) {
      OnEatItem?.Invoke(this, EventArgs.Empty);

      item.DestroySelf();

      isItemTooked = true;
      PacManLives.Instance.GiveLife();

      OnItemPickup?.Invoke(this, EventArgs.Empty);

      ScoreUI.Instance.AddScore(item.GetAmountOfScore());
    }
  }

  private IEnumerator WaitBeforeGameOver() {
    float waitTime = 3f;
    yield return new WaitForSeconds(waitTime);
    isRespawning = false;

    GameManager.Instance.SetGameState(GameManager.GameState.GameLost);
  }

  public void SetupLevel(LevelReferences level) {
    wallsTile = level.GetWallsTile();
    ghostDoor = level.GetDoorTile();
    startPosition = level.GetPacManStartingPosition();
  }

  public void SetState(State newState) {
    state = newState;
  }
  public State GetState() {
    return state;
  }
  public Transform GetTransform() {
    return transform;
  }
  public Vector3 GetPosition() {
    return transform.position;
  }
  public void SetPosition(Vector3 position) {
    transform.position = position;
  }
  public void SetTargetPosition(Vector2 position) {
    targetPosition = position;
  }
  public Vector2 GetCurrentDirection() {
    return currentDirection;
  }

  public bool GetIsMoving() {
    return isMoving;
  }
  public Transform GetStartPosition() {
    return startPosition;
  }

  public bool GetIsItemTooked() {
    return isItemTooked;
  }
  public int GetScoreAmountCombo() {
    return scoreAmountCombo;
  }
  public TextMeshPro ScoreAmountComboWorldUI() {
    return pacManScoreAmountCombo.GetScoreAmountComboTextMesh();
  }
}