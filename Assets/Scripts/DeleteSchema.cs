using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class DeleteSchema : MonoBehaviour
{
  public GameObject message;
  public GameObject messageParent;
  public GameObject link;
  public GameObject buttonLine;
  private void Start()
  {
    link = GameObject.FindGameObjectsWithTag("MainCamera")[0];
    messageParent = link.GetComponent<ListSchemaUIControl>().messageParent;
  }
  public void DeleteSchemaWithName ()
  {
    string filename = $"{transform.name}.schema";
    var path = Path.Combine(Application.persistentDataPath, filename);
    if (File.Exists(path))
    {
      File.Delete(path);
      Destroy(buttonLine);

      var mes = Instantiate(message, messageParent.transform);
      Destroy(mes, 1);
    }
  }
}
