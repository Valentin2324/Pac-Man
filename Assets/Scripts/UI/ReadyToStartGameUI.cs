using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyToStartGameUI : MonoBehaviour {
  
  [SerializeField] private TextMeshProUGUI readyToBeginTextMesh;
  [SerializeField] private Image backgroundImage;
  [SerializeField] private float waitBetweenTextSwitch;

  private void Awake() {
    StartCoroutine(ReadyToBeginRoutineText());
  }

  private IEnumerator ReadyToBeginRoutineText() {
    readyToBeginTextMesh.text = "READY?";

    yield return new WaitForSeconds(waitBetweenTextSwitch);

    readyToBeginTextMesh.text = "SET?!";

    yield return new WaitForSeconds(waitBetweenTextSwitch);

    readyToBeginTextMesh.text = "GO!";

    yield return new WaitForSeconds(waitBetweenTextSwitch);

    readyToBeginTextMesh.enabled = false;
    backgroundImage.enabled = false;
  }
}
