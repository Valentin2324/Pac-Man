using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PacManScoreAmountCombo : MonoBehaviour {
  
  [SerializeField] private TextMeshPro scoreAmountComboTextMesh;

  private void Awake() {
    scoreAmountComboTextMesh.enabled = false;
  }

  public TextMeshPro GetScoreAmountComboTextMesh() {
    scoreAmountComboTextMesh.text = PacMan.Instance.GetScoreAmountCombo().ToString();
    scoreAmountComboTextMesh.transform.rotation = Quaternion.Euler(0, 0, 0);
    return scoreAmountComboTextMesh;
  }
}
