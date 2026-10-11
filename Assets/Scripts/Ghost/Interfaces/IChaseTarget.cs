using UnityEngine;

public interface IChaseTarget {
  // we include this function in each class that we extent IChaseTarget
  Transform GetChaseTarget();
}
