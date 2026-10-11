using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TogglePauseUI : MonoBehaviour  {
  
  public static TogglePauseUI Instance { get; private set; }

  public event EventHandler OnPause;
  public event EventHandler OnUnpause;
  
  [SerializeField] private Image pauseImage;
  [SerializeField] private Image unPauseImage;
  [SerializeField] private GameObject pauseTextGameObject;

  private Button togglePauseButton;
  private static bool isPaused = false;
  private Coroutine pauseTextCoroutine;

  private void Awake() {
    Instance = this;
    togglePauseButton = GetComponent<Button>();
    pauseTextGameObject.SetActive(false);

    if (!isPaused) {
      pauseImage.enabled = true;
      unPauseImage.enabled = false;
    }
    else {
      pauseImage.enabled = false;
      unPauseImage.enabled = true;
    }

    togglePauseButton.onClick.AddListener(() => {
      if (!isPaused) {
        PauseGame();
      }
      else {
        UnPauseGame();
      }
    });
  }

  private void Update() {
    if (Keyboard.current.pKey.wasPressedThisFrame && !isPaused) {
      PauseGame();
    }
    else if (Keyboard.current.pKey.wasPressedThisFrame && isPaused) {
      UnPauseGame();
    }
  }

  private void PauseGame() {
    AudioListener.pause = true;
    OnPause?.Invoke(this, EventArgs.Empty);

    pauseImage.enabled = false;
    unPauseImage.enabled = true;
    isPaused = true;

    pauseTextCoroutine = StartCoroutine(TogglePauseTextFlash());
  }

  private void UnPauseGame() {
    AudioListener.pause = false;
    OnUnpause?.Invoke(this, EventArgs.Empty);

    pauseImage.enabled = true;
    unPauseImage.enabled = false;
    isPaused = false;

    if (pauseTextCoroutine != null) {
      StopCoroutine(pauseTextCoroutine);
    }
    pauseTextGameObject.SetActive(false);
  }

  private IEnumerator TogglePauseTextFlash() {
    float flashEffectDuration = 0.5f;

    while (true) {
      pauseTextGameObject.SetActive(true);
      yield return new WaitForSecondsRealtime(flashEffectDuration);

      pauseTextGameObject.SetActive(false);
      yield return new WaitForSecondsRealtime(flashEffectDuration);
    }
  }
}
