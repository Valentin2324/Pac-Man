using System;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  
  [SerializeField] private AudioClip startGameAudio;
  [SerializeField] private AudioClip pacManEatPaletteAudio;
  [SerializeField] private AudioClip pacManEatFruitAudio;
  [SerializeField] private AudioClip pacManDeathAudio;
  [SerializeField] private AudioClip ghostEatenAudio;

  private void Start() {
    GameManager.Instance.OnGameStart += GameManager_OnGameStart;
    PacMan.Instance.OnEatPalette += PacMan_OnEatPalette;
    PacMan.Instance.OnEatItem += PacMan_OnEatItem;
    PacMan.Instance.OnDeath += PacMan_OnDeath;
    PacMan.Instance.OnGhostKill += PacMan_OnGhostKill;

    ToggleVolumeUI.Instance.OnAudioMute += ToggleVolumeUI_OnMute;
    ToggleVolumeUI.Instance.OnAudioUnmute += ToggleVolumeUI_OnUnmute;
  }

  private void ToggleVolumeUI_OnUnmute(object sender, EventArgs e) {
    AudioListener.volume = 1f;
  }

  private void ToggleVolumeUI_OnMute(object sender, EventArgs e) {
    AudioListener.volume = 0f;
  }

  private void PacMan_OnGhostKill(object sender, PacMan.OnGhostKillEventArgs e) {
    AudioSource.PlayClipAtPoint(ghostEatenAudio, Camera.main.transform.position, 0.5f);
  }

  private void PacMan_OnDeath(object sender, EventArgs e) {
    AudioSource.PlayClipAtPoint(pacManDeathAudio, Camera.main.transform.position, 0.4f);
  }

  private void PacMan_OnEatItem(object sender, EventArgs e) {
    AudioSource.PlayClipAtPoint(pacManEatFruitAudio, Camera.main.transform.position, 1f);
  }

  private void PacMan_OnEatPalette(object sender, EventArgs e) {
    AudioSource.PlayClipAtPoint(pacManEatPaletteAudio, Camera.main.transform.position, 0.2f);
  }

  private void GameManager_OnGameStart(object sender, EventArgs e) {
    AudioSource.PlayClipAtPoint(startGameAudio, Camera.main.transform.position, 0.3f);
  }
}
