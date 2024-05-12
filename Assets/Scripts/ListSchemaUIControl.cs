using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ListSchemaUIControl : MonoBehaviour
{
  public Dictionary<string, List<NodesMap>> Schemas;

  public Transform parentForButton;
  public TMP_Text buttonTextPrefab;
  public Image buttonLinePrefab;
  public GameObject messageParent;
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

      var butLine = Instantiate(buttonLinePrefab, parentForButton);
      var a = butLine.GetComponentsInChildren<Button>();
      Button preview = a[0];
      Button schemaNane = a[1];
      Button delete = a[2];

      preview.name = name;
      delete.name = name;
      schemaNane.GetComponentInChildren<TMP_Text>().text = name;
      schemaNane.name = name;
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
  public void LoadListShema()
  {
    SceneManager.LoadScene(2);
  }

}
