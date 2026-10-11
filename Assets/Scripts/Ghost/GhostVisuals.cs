using System;
using System.Collections;
using UnityEngine;

public class GhostVisuals : MonoBehaviour {
  
  [SerializeField] private Transform ghostFaceTransform;

  [SerializeField] private SpriteRenderer ghostFaceSprite;
  [SerializeField] private SpriteRenderer ghostSprite;
  [SerializeField] private SpriteRenderer ghostSpiritSprite;

  private Ghost ghost;
  private Animator animator;
  private Vector3 newFacePosition;
  private Coroutine frightenedRoutine;
  private Coroutine secondfrightenedRoutinePart;

  private void Awake() {
    ghost = GetComponent<Ghost>();
    animator = GetComponentInChildren<Animator>();

    ghostSpiritSprite.enabled = false;
  }

  private void Start() {
    GhostManager.Instance.OnFrightenedMode += GhostManager_OnFrightenedMode;
    GameManager.Instance.OnGameOver += GameManager_OnGameOver;
  }

  private void GameManager_OnGameOver(object sender, EventArgs e) {
    ghostFaceSprite.enabled = false;
    ghostSprite.enabled = false;
    ghostSpiritSprite.enabled = false;
  }

  private void GhostManager_OnFrightenedMode(object sender, GhostManager.OnFrightenedModeEventArgs e) {
    if (frightenedRoutine != null) {
      StopCoroutine(frightenedRoutine);
    }
    if (secondfrightenedRoutinePart != null) {
      StopCoroutine(secondfrightenedRoutinePart);
      animator.SetBool("IsWhite", false);
    }
    frightenedRoutine = StartCoroutine(FrightenedVisual(e));
  }

  private IEnumerator FrightenedVisual(GhostManager.OnFrightenedModeEventArgs e) {
    ghostFaceSprite.enabled = false;
    ghostSprite.enabled = true;
    ghostSpiritSprite.enabled = false;

    animator.SetBool("IsFrightened", true);

    float secondFrightenedPartDuration = 5f;
    float firstFrightenedPartDuration = e.frightenedModeDuration - secondFrightenedPartDuration;

    yield return new WaitForSeconds(firstFrightenedPartDuration);

    secondfrightenedRoutinePart = StartCoroutine(ToggleFrightenedFinalPart(secondFrightenedPartDuration));
  }

  private IEnumerator ToggleFrightenedFinalPart(float duration) {
    float elapsed = 0f;
    float toggleDuration = 0.5f;

    while (elapsed < duration) {
      elapsed += toggleDuration;
      animator.SetBool("IsWhite", true);

      yield return new WaitForSeconds(toggleDuration);

      animator.SetBool("IsWhite", false);

      yield return new WaitForSeconds(toggleDuration);
    }
  }

  private void Update() {
    if (GameManager.Instance.GetGameState() != GameManager.GameState.GameRunning) return;

    if (ghost.GetState() == Ghost.State.Eaten) {
      ghostFaceSprite.enabled = false;
      ghostSprite.enabled = false;
      ghostSpiritSprite.enabled = true;
    }
    else if (ghost.GetState() != Ghost.State.Frightened) {

      ghostFaceSprite.enabled = true;
      ghostSprite.enabled = true;
      ghostSpiritSprite.enabled = false;
      animator.SetBool("IsFrightened", false);
      animator.SetBool("IsWhite", false);

      if (ghost.GetCurrentDirection() == Vector2.up) {
        UpdateFaceDirection(0f, 0.2f);
      }
      else if (ghost.GetCurrentDirection() == Vector2.left) {
        UpdateFaceDirection(-0.2f, 0f);
      }
      else if (ghost.GetCurrentDirection() == Vector2.down) {
        UpdateFaceDirection(0f, -0.2f);
      }
      else if (ghost.GetCurrentDirection() == Vector2.right) {
        UpdateFaceDirection(0.2f, 0f);
      }
    }
  }

  private void UpdateFaceDirection(float x, float y) {
    newFacePosition = ghostFaceTransform.localPosition;
    newFacePosition.x = x;
    newFacePosition.y = y;
    ghostFaceTransform.localPosition = newFacePosition;
  }
}
