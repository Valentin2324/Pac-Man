using System.Collections;
using UnityEngine;

public class SpawnItem : MonoBehaviour {
  
  [SerializeField] private GameObject itemPrefab;
  [SerializeField] private Transform itemSpawnPosition;

  private bool hasSpawned = false;
  private bool itSelfDestroyed = false;

  private void Update() {
    if (hasSpawned) return;

    if (PaletteManager.Instance.GetPaletteMarkers().Count <= 150 && !itSelfDestroyed) {
      StartCoroutine(ItemLifeSpan());
      hasSpawned = true;
    }
    else if (itSelfDestroyed) {
      StartCoroutine(AfterSelfDestroyLifeSpan());
      hasSpawned = true;
    }
  }

  private IEnumerator ItemLifeSpan() {
    GameObject itemGameObject = Instantiate(itemPrefab, itemSpawnPosition.position, Quaternion.identity);
    Item spawnedItem = itemGameObject.GetComponent<Item>();

    float itemLifeSpan = 10f;
    yield return new WaitForSeconds(itemLifeSpan);

    if (!PacMan.Instance.GetIsItemTooked()) {
      spawnedItem.DestroySelf();
      itSelfDestroyed = true;
      hasSpawned = false;
    }
  }

  private IEnumerator AfterSelfDestroyLifeSpan() {
    float waitBeforeSpawn = 15f;
    yield return new WaitForSeconds(waitBeforeSpawn);

    StartCoroutine(ItemLifeSpan());
  }
}
