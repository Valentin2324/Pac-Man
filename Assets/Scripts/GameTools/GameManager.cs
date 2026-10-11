using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
  
  private const float MAZE_MAX_WIDTH = 18;

  public static GameManager Instance { get; private set; }

  public event EventHandler OnGameStart;
  public event EventHandler<OnGameOverEventArgs> OnGameOver;
  public class OnGameOverEventArgs : EventArgs {
    public bool isGameWon;
    public float waitTimeBeforeReset;
  }

  private static int currentLevelNumber = 1;

  [SerializeField] private List<GameLevel> gameLevels;
  [SerializeField] private float waitTimeBeforeGameStarts;

  public enum GameState {
    WaitingToStart,
    GameRunning,
    GameLost,
    GameWon
  }

  private GameState gameState;
  private bool isGameWon = false;
  private bool isWaitingToStart = true;
  private bool isGameRunning = false;

  private void Awake() {
    Instance = this;
    gameState = GameState.WaitingToStart;

    StartCoroutine(WaitingBeforeGameStarts());
  }

  private IEnumerator WaitingBeforeGameStarts() {
    yield return new WaitForSeconds(waitTimeBeforeGameStarts);

    gameState = GameState.GameRunning;
  }

  private void Start() {
    PacMan.Instance.OnGhostKill += PacMan_OnGhostKill;
    PacMan.Instance.OnDeath += PacMan_OnDeath;

    TogglePauseUI.Instance.OnPause += TogglePauseUI_OnPause;
    TogglePauseUI.Instance.OnUnpause += TogglePauseUI_OnUnpause;

    LoadCurrentLevel();
  }

  private void TogglePauseUI_OnUnpause(object sender, EventArgs e) {
    Time.timeScale = 1f;
  }

  private void TogglePauseUI_OnPause(object sender, EventArgs e) {
    Time.timeScale = 0f;
  }

  private void PacMan_OnGhostKill(object sender, PacMan.OnGhostKillEventArgs e) {
    StartCoroutine(FreezeGameOnKill(e));
  }

  private void LoadCurrentLevel() {
    GameLevel gameLevel = GetGameLevel();
    GameLevel spawnedGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);

    LevelReferences refs = spawnedGameLevel.GetComponentInChildren<LevelReferences>();
    PacMan.Instance.SetupLevel(refs); 
    PacMan.Instance.SetPosition(refs.GetPacManStartingPosition().position);
    PacMan.Instance.SetTargetPosition(PacMan.Instance.GetPosition());
  }

  private GameLevel GetGameLevel() {
    foreach (GameLevel level in gameLevels) {
      if (level.GetLevelNumber() == currentLevelNumber) {
        return level;
      }
    }

    return null;
  }

  private IEnumerator WaitBeforeNextLevel() {
    float waitTime = 5f;
    yield return new WaitForSeconds(waitTime);

    UpdateLevel();
  }

  public void UpdateLevel() {
    if (currentLevelNumber + 1 <= gameLevels.Count && gameState == GameState.GameWon) {
      currentLevelNumber++;
    }

    ScoreUI.Instance.UpdateLevelUI();
    SceneManager.LoadScene("GameScene");
  }

  private void PacMan_OnDeath(object sender, EventArgs e) {
    StartCoroutine(WaitBeforeStageReset());
  }

  private IEnumerator FreezeGameOnKill(PacMan.OnGhostKillEventArgs e) {
    float freezeTime = 1f;
    Time.timeScale = 0f;
    PacManVisual.Instance.HideSprite();
    if (e.isNormalGhost) {
      PacMan.Instance.ScoreAmountComboWorldUI().enabled = true;
    }

    yield return new WaitForSecondsRealtime(freezeTime);

    PacMan.Instance.ScoreAmountComboWorldUI().enabled = false;
    PacManVisual.Instance.ShowSprite();
    Time.timeScale = 1f;
  }

  private void ResetCharactersAfterPacManLostLife() {
    PacMan.Instance.SetPosition(PacMan.Instance.GetStartPosition().position);
    PacMan.Instance.SetTargetPosition(PacMan.Instance.GetStartPosition().position);

    PacMan.Instance.SetState(PacMan.State.Normal);

    foreach (Ghost ghost in GhostManager.Instance.GetGhosts()) {
      ghost.SetPosition(ghost.GetStartPosition());
      ghost.SetTargetPosition(ghost.GetStartPosition());
    }

    GhostManager.Instance.ResetStates();
    
    if (currentLevelNumber == 1) {
      foreach(Wasp specialGhost in SpecialGhostManager.Instance.GetSpecialGhosts()) {
        specialGhost.SetPosition(specialGhost.GetStartPosition());
        specialGhost.SetTargetPosition(specialGhost.GetStartPosition());
      }

      SpecialGhostManager.Instance.ResetStates();
    }
  }

  private IEnumerator WaitBeforeStageReset() {
    float waitBeforeStageReset = 3f;
    yield return new WaitForSeconds(waitBeforeStageReset);

    if (PacManLives.Instance.GetLives() <= 0) yield break;

    ResetCharactersAfterPacManLostLife();
    gameState = GameState.GameRunning;
  }

  private void Update() {

    switch (gameState) {
      case GameState.WaitingToStart:
        if (isWaitingToStart) {
          OnGameStart?.Invoke(this, EventArgs.Empty);
          GameOverUI.Instance.HideGameOver();
          isWaitingToStart = false;
        }
        break;  
      case GameState.GameRunning:
        if (!isGameRunning) {
          isGameRunning = true;
        }

      if (PaletteManager.Instance.GetPaletteMarkers().Count <= 0) {
        SetGameState(GameState.GameWon);
      }
        break;
      case GameState.GameLost:
      SetGameState(GameState.GameLost);
        break;
      case GameState.GameWon:
        break;
    }
  }

  public void SetGameState(GameState newState) {
    if (gameState == newState) return;

    gameState = newState;

    switch (newState) {
      case GameState.GameLost:
        GameOverUI.Instance.ShowLoseGameOver();

        isGameWon = false;
        currentLevelNumber = 1;
        OnGameOver?.Invoke(this, new OnGameOverEventArgs{
          isGameWon = isGameWon,
          waitTimeBeforeReset = 3f
        });
        isGameRunning = false;
        break;

      case GameState.GameWon:
        GameOverUI.Instance.ShowWinGameOver();

        isGameWon = true;
        OnGameOver?.Invoke(this, new OnGameOverEventArgs{
          isGameWon = isGameWon
        });
        isGameRunning = false;
        break;
    }

    StartCoroutine(WaitBeforeNextLevel());
  }

  public GameState GetGameState() {
    return gameState;
  }

  public int GetCurrentGameLevel() {
    return currentLevelNumber;
  }
  public float GetMazeMapBound() {
    return MAZE_MAX_WIDTH;
  }
  public bool GetIsGameRunning() {
    return isGameRunning;
  }
}
