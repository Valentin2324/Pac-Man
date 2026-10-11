using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PaletteManager : MonoBehaviour {

  public static PaletteManager Instance { get; private set; }
  
  [SerializeField] private GameObject palettePrefab;
  [SerializeField] private GameObject powerPalettePrefab;
  [SerializeField] private Tilemap floorTile;
  [SerializeField] private Tilemap forbiddenFloorTile;
  [SerializeField] private Tilemap wallsTile;
  [SerializeField] private List<Transform> powerPaletteMarkers;

  private List<Transform> activePowerPalettes;
  private List<Transform> activePalettes;

  private void Awake() {
    Instance = this;
    activePowerPalettes = new List<Transform>();
    activePalettes = new List<Transform>();

    SpawnPalletes();
  }

  private void SpawnPalletes() {
    BoundsInt floorBounds = floorTile.cellBounds;

    foreach (Vector3Int cell in floorBounds.allPositionsWithin) {
      if (!floorTile.HasTile(cell)) continue;
      if (wallsTile.HasTile(cell)) continue;
      if (forbiddenFloorTile.HasTile(cell)) continue;

      Vector3 worldPosition = floorTile.CellToWorld(cell) + floorTile.cellSize / 2f;
      bool isPowerPalette = IsPowerPaletteCell(cell);
      GameObject palette = isPowerPalette ? powerPalettePrefab : palettePrefab;

      GameObject instance = Instantiate(palette, worldPosition, Quaternion.identity);

      if (isPowerPalette) {
        activePowerPalettes.Add(instance.transform);
      }
      else {
        activePalettes.Add(instance.transform);
      }
    }
  }

  private bool IsPowerPaletteCell(Vector3Int cell) {
    Vector3 cellCenter = floorTile.CellToWorld(cell) + floorTile.cellSize / 2f;
    foreach (Transform marker in powerPaletteMarkers) {
      if (Vector3.Distance(marker.position, cellCenter) < 0.1f) return true;
    }
    return false;
  }

  public List<Transform> GetPowerPaletteMarkers() {
    return activePowerPalettes;
  }
  public List<Transform> GetPaletteMarkers() {
    return activePalettes;
  }
  
  public void RemovePaletteFromList(Transform marker) {
    activePalettes.Remove(marker);
  }
  public void RemovePowerPaletteFromList(Transform marker) {
    activePowerPalettes.Remove(marker);
  }
}
