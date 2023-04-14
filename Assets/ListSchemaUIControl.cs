using UnityEngine;
using UnityEngine.SceneManagement;

public class ListSchemaUIControl : MonoBehaviour
{
  public void LoadMainMenu()
  {
    SceneManager.LoadScene(0);
  }
  public void LoadStartGeneration()
  {
    SceneManager.LoadScene(1);
  }
}
