using System;
using System.Collections;
using UnityEngine;
using System.Linq;

public class AudioGhostManager : MonoBehaviour {

  [SerializeField] private AudioSource normalMovement;
  [SerializeField] private AudioSource frightenedMovement;
  [SerializeField] private AudioSource enragedMovement;
  [SerializeField] private AudioSource returnHomeMovement;

  private enum Mode {
    Silent,
    Normal,
    Frightened,
    Eaten,
    Enraged
  }

  private Mode currentMode = Mode.Silent;
  private Mode wanted;
  private bool isEnraged = false;
  private bool isPacManDead = false;
  private bool isAudioMute = false;

  private void Start() {
    PacMan.Instance.OnDeath += PacMan_OnDeath;
    PacMan.Instance.OnRespawn += PacMan_OnRespawn;
    GameManager.Instance.OnGameOver += GameManager_OnGameOver;

    ToggleVolumeUI.Instance.OnAudioMute += ToggleVolumeUI_OnMute;
    ToggleVolumeUI.Instance.OnAudioUnmute += ToggleVolumeUI_OnUnmute;
  }

  private void GameManager_OnGameOver(object sender, GameManager.OnGameOverEventArgs e) {
    isEnraged = false;
  }

  private void ToggleVolumeUI_OnUnmute(object sender, EventArgs e) {
    isAudioMute = false;

    normalMovement.mute = false;
    frightenedMovement.mute = false;
    enragedMovement.mute = false;
    returnHomeMovement.mute = false;
  }

  private void ToggleVolumeUI_OnMute(object sender, EventArgs e) {
    isAudioMute = true;

    normalMovement.mute = true;
    frightenedMovement.mute = true;
    enragedMovement.mute = true;
    returnHomeMovement.mute = true;
  }

  private void PacMan_OnRespawn(object sender, EventArgs e) {
    isPacManDead = false;
  }

  private void PacMan_OnDeath(object sender, EventArgs e) {
    isPacManDead = true;
  }

  private void Update() {
    if (isAudioMute) return;

    bool anyEaten = GhostManager.Instance.GetGhosts().Any(
        ghost => ghost.GetState() == Ghost.State.Eaten) || 
      SpecialGhostManager.Instance.GetSpecialGhosts().Any(
        specialGhost => specialGhost.GetState() == Wasp.State.Eaten || 
        (specialGhost.GetState() == Wasp.State.Home && !specialGhost.GetIsHomeForever()));

    bool anyFrightened = GhostManager.Instance.GetGhosts().Any(ghost => ghost.GetState() == Ghost.State.Frightened);

    if (GhostManager.Instance.GetIsBlinkyEnraged() && !isEnraged) {
      isEnraged = true;
    }

    wanted = Mode.Silent;
    if (GameManager.Instance.GetIsGameRunning() && !isPacManDead) {
      if (anyEaten)            wanted = Mode.Eaten;
      else if (anyFrightened)  wanted = Mode.Frightened;
      else if (isEnraged)      wanted = Mode.Enraged;
      else                     wanted = Mode.Normal;
    }

    if (wanted == currentMode) return;

    normalMovement.Stop();
    frightenedMovement.Stop();
    enragedMovement.Stop();
    returnHomeMovement.Stop();

    switch (wanted) {
      case Mode.Normal:     normalMovement.Play(); break;
      case Mode.Frightened: frightenedMovement.Play(); break;
      case Mode.Enraged:    enragedMovement.Play(); break;
      case Mode.Eaten:      returnHomeMovement.Play(); break;
    }

    currentMode = wanted;
  }
}
