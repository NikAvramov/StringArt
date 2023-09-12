using UnityEngine;

public class UpdateSafeArea : MonoBehaviour
{
  void Start()
  {
    var safeArea = Screen.safeArea;
    var canvas = GetComponent<RectTransform>();

    var anchorMin = safeArea.position;
    var anchorMax = anchorMin + safeArea.size;

    anchorMin.x /= Screen.width;
    anchorMin.y /= Screen.height;
    anchorMax.x /= Screen.width;
    anchorMax.y /= Screen.height;

    canvas.anchorMin = anchorMin;
    canvas.anchorMax = anchorMax;
  }
}
