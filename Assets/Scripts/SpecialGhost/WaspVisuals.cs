using System;
using UnityEngine;

public class WaspVisuals : MonoBehaviour {
  
  [SerializeField] private Transform ghostFaceTransform;

  [SerializeField] private SpriteRenderer ghostFaceSprite;
  [SerializeField] private SpriteRenderer ghostSprite;
  [SerializeField] private SpriteRenderer ghostSpiritSprite;

  private Wasp wasp;
  private Vector3 newFacePosition;

  private void Awake() {
    wasp = GetComponent<Wasp>();

    ghostSpiritSprite.enabled = false;
  }

  private void Start() {
    GameManager.Instance.OnGameOver += GameManager_OnGameOver;
  }

  private void GameManager_OnGameOver(object sender, EventArgs e) {
    ghostFaceSprite.enabled = false;
    ghostSprite.enabled = false;
    ghostSpiritSprite.enabled = false;
  }

  private void Update() {
    if (GameManager.Instance.GetGameState() != GameManager.GameState.GameRunning) return;

    if (wasp.GetState() == Wasp.State.Eaten || (wasp.GetState() == Wasp.State.Home && !wasp.GetIsHomeForever())) {
      ghostFaceSprite.enabled = false;
      ghostSprite.enabled = false;
      ghostSpiritSprite.enabled = true;
    }
    else {

      ghostFaceSprite.enabled = true;
      ghostSprite.enabled = true;
      ghostSpiritSprite.enabled = false;

      if (wasp.GetCurrentDirection() == Vector2.up) {
        UpdateFaceDirection(0f, 0.2f);
      }
      else if (wasp.GetCurrentDirection() == Vector2.left) {
        UpdateFaceDirection(-0.2f, 0f);
      }
      else if (wasp.GetCurrentDirection() == Vector2.down) {
        UpdateFaceDirection(0f, -0.2f);
      }
      else if (wasp.GetCurrentDirection() == Vector2.right) {
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
