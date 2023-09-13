using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIControl : MonoBehaviour
{
  public void LoadListShema()
  {
    SceneManager.LoadScene(2);
  }
  public void LoadStartGeneration()
  {
    SceneManager.LoadScene(1);
  }
  public void ExitApp()
  { 
    Application.Quit();
  }
}
