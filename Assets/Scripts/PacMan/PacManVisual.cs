using System;
using System.Collections;
using UnityEngine;

public class PacManVisual : MonoBehaviour {

  public static PacManVisual Instance { get; private set; }
  
  private SpriteRenderer pacManSprite;
  private Animator animator;

  private void Awake() {
    Instance = this;

    pacManSprite = GetComponentInChildren<SpriteRenderer>();
    animator = GetComponentInChildren<Animator>();
  }

  private void Start() {
    PacMan.Instance.OnDeath += PacMan_OnDeath;
    PacMan.Instance.OnRespawn += PacMan_OnRespawn;

    GameManager.Instance.OnGameOver += GameManager_OnGameOver;
  }

  private void GameManager_OnGameOver(object sender, EventArgs e) {
    pacManSprite.enabled = false;
  }

  private void PacMan_OnRespawn(object sender, EventArgs e) {
    transform.rotation = Quaternion.Euler(0, 0, 0);
    animator.SetTrigger("OnRespawn");
  }

  private void PacMan_OnDeath(object sender, EventArgs e) {
    animator.ResetTrigger("OnRespawn");
    animator.SetTrigger("OnDeath");
  }

  private void Update() {
    if (GameManager.Instance.GetGameState() != GameManager.GameState.GameRunning || PacMan.Instance.GetState() == PacMan.State.Dead) return;

    RotateSpriteBasedOnDirection();
    
    ToggleAnimationWhenMoving(); 
  }

  private void RotateSpriteBasedOnDirection() {
    if (PacMan.Instance.GetCurrentDirection() == Vector2.up) {
      transform.rotation = Quaternion.Euler(0, 0, 90);
    }
    else if (PacMan.Instance.GetCurrentDirection() == Vector2.left) {
      transform.rotation = Quaternion.Euler(0, 0, 180);
    }
    else if (PacMan.Instance.GetCurrentDirection() == Vector2.down) {
      transform.rotation = Quaternion.Euler(0, 0, -90);
    }
    else if (PacMan.Instance.GetCurrentDirection() == Vector2.right) {
      transform.rotation = Quaternion.Euler(0, 0, 0);
    }
  }

  private void ToggleAnimationWhenMoving() {
    if (PacMan.Instance.GetIsMoving()) {
      animator.SetBool("IsMoving", true);
    }
    else {
      animator.SetBool("IsMoving", false);
    }
  }

  public void HideSpriteAfterAnimation() {
    StartCoroutine(WaitBeforeHide());
  }
  public void HideSprite() {
    pacManSprite.enabled = false;
  }
  public void ShowSprite() {
    pacManSprite.enabled = true;
  }

  private IEnumerator WaitBeforeHide() {
    float waitBeforeHideValue = 2f;
    yield return new WaitForSeconds(waitBeforeHideValue);
    pacManSprite.enabled = false;
  }
}
