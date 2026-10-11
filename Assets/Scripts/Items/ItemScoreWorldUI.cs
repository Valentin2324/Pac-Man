using TMPro;
using UnityEngine;

public class ItemScoreWorldUI : MonoBehaviour {
  
  [SerializeField] private TextMeshPro itemScoreAmountTextMesh;

  private void Awake() {
    itemScoreAmountTextMesh.enabled = false;
  }

  public void ShowItemScoreAmount(int scoreAmount) {
    itemScoreAmountTextMesh.enabled = true;
    itemScoreAmountTextMesh.text = scoreAmount.ToString();
  }
}
