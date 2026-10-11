using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour {

  public static GhostManager Instance { get; private set; }

  public event EventHandler<OnEatenGhostEventArgs> OnEatenGhost;
  public class OnEatenGhostEventArgs : EventArgs {
    public Ghost eatenGhost;
  }

  public event EventHandler<OnChaseModeEventArgs> OnChaseMode;
  public class OnChaseModeEventArgs : EventArgs {
    public bool isBlinkyEnraged;
  }
  public event EventHandler<OnFrightenedModeEventArgs> OnFrightenedMode;
  public class OnFrightenedModeEventArgs : EventArgs {
    public float frightenedModeDuration;
    public bool isBlinkyEnraged;
  }
  
  [SerializeField] private Transform blinkyTransform;
  [SerializeField] private Transform pinkyTransform;
  [SerializeField] private Transform inkyTransform;
  [SerializeField] private Transform clydeTransform;
  [SerializeField] private Transform isidaTransform;
  [SerializeField] private GameObject blinkyPrefab;
  [SerializeField] private GameObject pinkyPrefab;
  [SerializeField] private GameObject inkyPrefab;
  [SerializeField] private GameObject clydePrefab;
  [SerializeField] private GameObject isidaPrefab;
  [SerializeField] private float scatterModeDuration;
  [SerializeField] private float chaseModeDuration;
  [SerializeField] private float frightenedModeDuration;

  private Ghost.State baseState = Ghost.State.Scatter;
  private float modeTimer = 0f;
  private float frightenedTimer = 0f;
  private bool isFrightened = false;
  private bool isBlinkyEnraged = false;
  private bool didGhostLeaveHouseRoutineStart = false;
  private Coroutine ghostLeaveHouseRoutine;

  private List<Ghost> ghosts;

  private void Awake() {
    Instance = this;
    ghosts = new List<Ghost>();
  }

  private void Start() {
    SpawnGhost(blinkyPrefab, blinkyTransform);
    SpawnGhost(pinkyPrefab, pinkyTransform);
    SpawnGhost(inkyPrefab, inkyTransform);
    SpawnGhost(clydePrefab, clydeTransform);
    if (GameManager.Instance.GetCurrentGameLevel() == 3 || GameManager.Instance.GetCurrentGameLevel() == 4 ||
        GameManager.Instance.GetCurrentGameLevel() == 7 || GameManager.Instance.GetCurrentGameLevel() == 8) {
      SpawnGhost(isidaPrefab, isidaTransform);
    }

    PacMan.Instance.OnPowerUp += PacMan_OnPowerUp;
    PacMan.Instance.OnGhostKill += PacMan_OnGhostKill;
  }

  private void PacMan_OnGhostKill(object sender, EventArgs e) {
    foreach (Ghost ghost in ghosts) {
      if (ghost.GetState() == Ghost.State.Eaten) {
        OnEatenGhost?.Invoke(this, new OnEatenGhostEventArgs {
          eatenGhost = ghost
        });
        ghost.SetOriginalSpeed();
      }
    }
  }

  private void SpawnGhost(GameObject ghost, Transform spawnPoint) {
    GameObject instance = Instantiate(ghost, spawnPoint.position, Quaternion.identity);
    ghosts.Add(instance.GetComponent<Ghost>());
  }

  private void PacMan_OnPowerUp(object sender, EventArgs e) {
    isFrightened = true;
    frightenedTimer = frightenedModeDuration;

    foreach (Ghost ghost in ghosts) {
      if (ghost.GetComponent<Blinky>() && isBlinkyEnraged) continue;

      if (ghost.GetState() != Ghost.State.Eaten) ghost.SetState(Ghost.State.Frightened);
    }

    OnFrightenedMode?.Invoke(this, new OnFrightenedModeEventArgs {
      frightenedModeDuration = frightenedModeDuration,
      isBlinkyEnraged = isBlinkyEnraged
    });
  }

  private void Update() {
    if (GameManager.Instance.GetGameState() != GameManager.GameState.GameRunning || PacMan.Instance.GetState() == PacMan.State.Dead) return;

    if (!didGhostLeaveHouseRoutineStart) {
      ghostLeaveHouseRoutine = StartCoroutine(StartGhostLeaveHouse());
      didGhostLeaveHouseRoutineStart = true;
    }

    foreach (Ghost ghost in ghosts) {
      if (ghost.GetPosition().x > GameManager.Instance.GetMazeMapBound() || ghost.GetPosition().x < -GameManager.Instance.GetMazeMapBound()) {
        Teleport(ghost);
      }
    }

    if (isFrightened) {                
      frightenedTimer -= Time.deltaTime;
      if (frightenedTimer <= 0f) {
        isFrightened = false;
        SetAllGhostsState(baseState);
      }
      return;
    }

    if (PaletteManager.Instance.GetPaletteMarkers().Count <= 50 && !isBlinkyEnraged) {
      isBlinkyEnraged = true;

      foreach (Ghost ghost in ghosts) {
        if (ghost.GetComponent<Blinky>()) {
          ghost.SetState(Ghost.State.Chase);
        }
      }
    }
    
    modeTimer += Time.deltaTime;
    float duration = baseState == Ghost.State.Scatter ? scatterModeDuration : chaseModeDuration;

    if (modeTimer >= duration) {
      modeTimer = 0f;
      baseState = baseState == Ghost.State.Scatter ? Ghost.State.Chase : Ghost.State.Scatter;
      SetAllGhostsState(baseState);
      OnChaseMode?.Invoke(this, new OnChaseModeEventArgs {
        isBlinkyEnraged = isBlinkyEnraged
      });
    }
  }

  private void SetAllGhostsState(Ghost.State state) {

    foreach (Ghost ghost in ghosts) {
      if (ghost.GetState() == Ghost.State.Eaten || (ghost.GetComponent<Blinky>() && isBlinkyEnraged)) continue;
      ghost.SetState(state);
    }
  }

  private IEnumerator StartGhostLeaveHouse() {
    float waitBeforeNextGhost = 8f;
    foreach (Ghost ghost in ghosts) {
      ghost.SetCanLeaveHouse(true);

      yield return new WaitForSeconds(waitBeforeNextGhost);
    }
  }

  private void Teleport(Ghost ghost) {
    bool wasTeleported = false;
    Vector3 currentPosition = ghost.transform.position;

    if (currentPosition.x < -GameManager.Instance.GetMazeMapBound()) {
      currentPosition.x = GameManager.Instance.GetMazeMapBound();
      wasTeleported = true;
    }
    else if (currentPosition.x > GameManager.Instance.GetMazeMapBound()) {
      currentPosition.x = -GameManager.Instance.GetMazeMapBound();
      wasTeleported = true;
    }
    
    ghost.SetPosition(currentPosition);
    if (wasTeleported) {
      ghost.SetTargetPosition(ghost.GetPosition()); 
    }
  }

  public List<Ghost> GetGhosts() {
    return ghosts;
  }
  public Ghost.State GetBaseState() {
    return baseState;
  }
  public bool GetIsBlinkyEnraged() {
   return isBlinkyEnraged; 
  }

  public void ResetStates() {
    modeTimer = 0f;
    baseState = Ghost.State.Scatter;
    isFrightened = false;
    frightenedTimer = 0f;

    foreach (Ghost ghost in ghosts) {
      ghost.SetState(baseState);

      if (ghost.GetComponent<Blinky>() && isBlinkyEnraged) {
        ghost.SetState(Ghost.State.Chase);
      }

      ghost.SetCanLeaveHouse(false);
      ghost.SetIsInsideHouse(true);
      ghost.SetPassedDoorWhileEaten(false);
    }

    if (ghostLeaveHouseRoutine != null) {
      StopCoroutine(ghostLeaveHouseRoutine);
    }
    ghostLeaveHouseRoutine = StartCoroutine(StartGhostLeaveHouse());
  }
}
