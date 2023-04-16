using UnityEngine;

public class LoadSchema : MonoBehaviour
{
  public GameObject link;
  void Start()
  {
    link = GameObject.FindGameObjectsWithTag("MainCamera")[0];
  }
  public void LoadSchemaWithName()
  {
    string name = transform.name;
    link.GetComponent<StepWeavingUIControl>().nodes = link.GetComponent<ListSchemaUIControl>().Schemas[name];
    link.GetComponent<ListSchemaUIControl>().menu.enabled = false;
    link.GetComponent<ListSchemaUIControl>().workpace.enabled = true;
  }
}
