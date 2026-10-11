using UnityEngine;

public class PowerPalette : MonoBehaviour {

  private void OnTriggerEnter2D(Collider2D collider2D) {
    if (collider2D.gameObject.TryGetComponent(out Wasp wasp)) {
      PaletteManager.Instance.RemovePowerPaletteFromList(transform);

      DestroySelf();
    }
  }

  public void DestroySelf() {
    Destroy(gameObject);
  }
}
