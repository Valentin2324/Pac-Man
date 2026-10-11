using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Item : MonoBehaviour {

  [SerializeField] private int scoreAmount;

  private ItemScoreWorldUI itemScoreWorldUI;
  private SpriteRenderer itemSprite;
  private CircleCollider2D itemCollider2D;
  private Light2D itemLight;

  private void Awake() {
    itemCollider2D = GetComponent<CircleCollider2D>();
    itemSprite = GetComponentInChildren<SpriteRenderer>();
    itemScoreWorldUI = GetComponent<ItemScoreWorldUI>();
    itemLight = GetComponentInChildren<Light2D>();
  }

  public int GetAmountOfScore() {
    itemScoreWorldUI.ShowItemScoreAmount(scoreAmount);
    return scoreAmount;
  }

  public void DestroySelf() {
    itemSprite.enabled = false;
    itemCollider2D.enabled = false;
    itemLight.enabled = false;

    Destroy(gameObject, 1f);
  }
}
