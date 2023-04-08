using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
  public TMP_InputField InputNodes;
  public TMP_InputField InputLines;
  public TMP_InputField InputWight;
  public Button Go;

  public int nodes;
  public int lines;
  public float wight;
  public string shape;
  void Start()
  {

  }
  void Update()
  {

  }
  public void SaveCountOfNodes()
  {
    int.TryParse(InputNodes.text.Trim(), out nodes);
  }
  public void SaveCountOfLines()
  {
    int.TryParse(InputLines.text.Trim(), out lines);
  }
  public void SaveWight()
  {
    float.TryParse(InputWight.text.Trim(), out wight);
    wight = Mathf.Clamp01(wight / 100);
  }
  public void SaveShapeCanvas(int i)
  {
    shape = i switch
    {
      0 => "Square",
      1 => "Circle",
      2 => "Triangle",
      3 => "Hexagone",
      _ => null
    };
  }
  public void StartGeneration()
  {
    GetComponent<GenerateStringArt>().enabled = true;
  }
}
