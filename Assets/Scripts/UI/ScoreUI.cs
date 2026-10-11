using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour {
  
  public static ScoreUI Instance { get; private set; }

  [SerializeField] private TextMeshProUGUI scoreTextMesh;
  [SerializeField] private TextMeshProUGUI highScoreTextMesh;
  [SerializeField] private TextMeshProUGUI currentLevelTextMesh;

  private static int score = 0;
  private static int highScore = 0;

  private void Awake() {
    Instance = this;

    UpdateHighScore();
    UpdateLevelUI();
  }

  private void Start() {
    GameManager.Instance.OnGameOver += GameManager_OnGameOver;
  }

  private void GameManager_OnGameOver(object sender, GameManager.OnGameOverEventArgs e) {
    UpdateHighScore();
    
    if (!e.isGameWon) {
      score = 0;
    }
  }

  private void Update() {
    scoreTextMesh.text = "SCORE: " + score.ToString();
  }

  public void AddScore(int amount) {
    score += amount;
  }

  private void UpdateHighScore() {
    highScore = PlayerPrefs.GetInt("HighScore", 0);
    
    if (score > highScore) {
      highScore = score;
      PlayerPrefs.SetInt("HighScore", highScore);
      PlayerPrefs.Save();
    }

    highScoreTextMesh.text = "HIGHSCORE: " + highScore.ToString();
  }

  public void UpdateLevelUI() {
    currentLevelTextMesh.text = "LEVEL: " + GameManager.Instance.GetCurrentGameLevel().ToString();
  }
}
