using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLevel : MonoBehaviour {
  
  [SerializeField] private int levelNumber;
  [SerializeField] private Transform pacManStartPosition;

  public int GetLevelNumber() {
    return levelNumber;
  }

  public Vector3 GetPacManStartPosition() {
    return pacManStartPosition.position;
  }
}
