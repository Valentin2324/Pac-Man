using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventoryUI : MonoBehaviour {
  
  [SerializeField] private List<Image> itemsInventory;

  private static int currentItemCount = 0;

  private void Awake() {

    RefreshImages();
  }

  private void Start() {
    PacMan.Instance.OnItemPickup += PacMan_OnItemPickup;
    GameManager.Instance.OnGameOver += GameManager_OnGameOver;
  }

  private void GameManager_OnGameOver(object sender, GameManager.OnGameOverEventArgs e) {
    if (!e.isGameWon) {
      currentItemCount = 0;
    }
  }

  private void PacMan_OnItemPickup(object sender, EventArgs e) {
    if (currentItemCount >= itemsInventory.Count) return;

    currentItemCount++;
    RefreshImages();
  }

  private void RefreshImages() {
    for (int i = 0; i < itemsInventory.Count; i++) {
      itemsInventory[i].enabled = i < currentItemCount;
    }
  }
}
