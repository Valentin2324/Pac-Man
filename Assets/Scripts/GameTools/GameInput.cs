using System;
using UnityEngine;

public class GameInput : MonoBehaviour {
  
  public static GameInput Instance { get; private set; }
  
  private InputSystem inputActions;

  private void Awake() {
    Instance = this;

    inputActions = new InputSystem();
    inputActions.Enable();
  }

  private void OnDestroy() {
    inputActions.Disable();
  }

  public bool IsUpActionPressed() {
    return inputActions.PacMan.PacManUp.IsPressed();
  }

  public bool IsLeftActionPressed() {
    return inputActions.PacMan.PacManLeft.IsPressed();
  }

  public bool IsDownActionPressed() {
    return inputActions.PacMan.PacManDown.IsPressed();
  }

  public bool IsRightActionPressed() {
    return inputActions.PacMan.PacManRight.IsPressed();
  }
}
