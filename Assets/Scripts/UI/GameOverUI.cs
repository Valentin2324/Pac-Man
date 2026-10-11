using System.Collections;
using UnityEngine;

public class GameOverUI : MonoBehaviour {

  public static GameOverUI Instance { get; private set; }
  
  [SerializeField] private GameObject winGameOverText;
  [SerializeField] private GameObject loseGameOverText;

  private void Awake() {
    Instance = this;
  }

  public void ShowWinGameOver() {
    StartCoroutine(FlashGameOver(winGameOverText));
  }
  public void ShowLoseGameOver() {
    StartCoroutine(FlashGameOver(loseGameOverText));
  }
  public void HideGameOver() {
    winGameOverText.SetActive(false);
    loseGameOverText.SetActive(false);
  }

  private IEnumerator FlashGameOver(GameObject gameOverText) {
    float flashEffectDuration = 0.5f;

    while (true) {
      gameOverText.SetActive(true);
      yield return new WaitForSeconds(flashEffectDuration);

      gameOverText.SetActive(false);
      yield return new WaitForSeconds(flashEffectDuration);
    }
  }
}
