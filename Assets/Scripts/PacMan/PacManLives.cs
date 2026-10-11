using System;
using System.Collections;
using UnityEngine;

public class PacManLives : MonoBehaviour {

  private const int STARTING_LIVES = 5;

  public static PacManLives Instance { get; private set; }
  
  private static int lives = STARTING_LIVES;

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    GameManager.Instance.OnGameOver += GameManager_OnGameOver;
  }

  private void GameManager_OnGameOver(object sender, GameManager.OnGameOverEventArgs e) {
    if (!e.isGameWon) {
      StartCoroutine(WaitBeforeResetLifes(e.waitTimeBeforeReset));
    }
  }

  private IEnumerator WaitBeforeResetLifes(float waitTime) {
    yield return new WaitForSeconds(waitTime);
    lives = STARTING_LIVES;
  }

  public void TakeLife() {
    lives--;
  }
  public void GiveLife() {
    if (lives < STARTING_LIVES) {
      lives++;
    }
  }

  public int GetLives() {
    return lives;
  }

}
