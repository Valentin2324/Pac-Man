using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToggleVolumeUI : MonoBehaviour {

  public static ToggleVolumeUI Instance { get; private set; }

  public event EventHandler OnAudioMute;
  public event EventHandler OnAudioUnmute;
  
  [SerializeField] private Image volumeOnImage;
  [SerializeField] private Image volumeOffImage;

  private Button toggleVolumeButton;
  private static bool isVolumeOff = false;

  private void Awake() {
    Instance = this;
    toggleVolumeButton = GetComponent<Button>();

    if (!isVolumeOff) {
      volumeOnImage.enabled = true;
      volumeOffImage.enabled = false;
    }
    else {
      volumeOnImage.enabled = false;
      volumeOffImage.enabled = true;
    }

    toggleVolumeButton.onClick.AddListener(() => {
      if (!isVolumeOff) {
        MuteAudio();
      }
      else {
        UnmuteAudio();
      }
    });
  }

  private void Update() {
    if (Keyboard.current.mKey.wasPressedThisFrame && !isVolumeOff) {
      MuteAudio();
    }
    else if (Keyboard.current.mKey.wasPressedThisFrame && isVolumeOff) {
      UnmuteAudio();
    }
  }

  private void MuteAudio() {
    OnAudioMute?.Invoke(this, EventArgs.Empty);

    volumeOnImage.enabled = false;
    volumeOffImage.enabled = true;
    isVolumeOff = true;
  }
  private void UnmuteAudio() {
    OnAudioUnmute?.Invoke(this, EventArgs.Empty);

    volumeOnImage.enabled = true;
    volumeOffImage.enabled = false;
    isVolumeOff = false;
  }
}
