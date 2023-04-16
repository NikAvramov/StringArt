using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ListSchemaUIControl : MonoBehaviour
{
  public Dictionary<string, List<NodesMap>> Schemas;

  public UnityEngine.UI.Button buttonPrefab;
  public Transform parentForButton;
  public TMP_Text buttonTextPrefab;

  public Canvas menu;
  public Canvas workpace;
  void Start()
  {
    Schemas = new();

    DirectoryInfo dir = new(Application.persistentDataPath);
    FileInfo[] files = dir.GetFiles("*.schema");

    for (int i = 0; i < files.Length; i++)
    {
      using var fs = new FileStream(files[i].FullName, FileMode.Open);
      var bf = new BinaryFormatter();
      var schema = (List<NodesMap>)bf.Deserialize(fs);
      var name = files[i].Name.Replace(".schema", string.Empty);
      Schemas.Add(name, schema);

      var but = Instantiate(buttonPrefab, parentForButton);
      but.name = name;

      //but.onClick.AddListener(GetComponent<LoadSchema>().LoadSchemaWithName);

      var buttonText = Instantiate(buttonTextPrefab, but.transform);
      buttonText.text = name;
    }
  }
  public void LoadMainMenu()
  {
    SceneManager.LoadScene(0);
  }
  public void LoadStartGeneration()
  {
    SceneManager.LoadScene(1);
  }

}
