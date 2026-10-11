using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PacManLivesUI : MonoBehaviour {
  
  [SerializeField] private List<Image> pacManLivesList;

  private void Awake() {
    foreach (Image image in pacManLivesList) {
      image.enabled = false;
    }
  }

  private void Start() {
    EnablePacManLivesImages();

    PacMan.Instance.OnDeath += PacMan_OnDeath;
    PacMan.Instance.OnItemPickup += PacMan_OnItemPickup;
    GameManager.Instance.OnGameOver += GameManager_OnGameOver;
  }

  private void PacMan_OnItemPickup(object sender, EventArgs e) {
    EnablePacManLivesImages();
  }

  private void GameManager_OnGameOver(object sender, GameManager.OnGameOverEventArgs e) {
    if (!e.isGameWon) {
      EnablePacManLivesImages();
    }
  }

  private void PacMan_OnDeath(object sender, EventArgs e) {
    EnablePacManLivesImages();
  }

  private void EnablePacManLivesImages() {
    int lives = PacManLives.Instance.GetLives();

    for (int i = 0; i < pacManLivesList.Count; i++) {
      pacManLivesList[i].enabled = i < lives;
    }
  }
}
