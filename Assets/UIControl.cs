using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NativeGallery;

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
  public Texture2D image;
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
  public void GetImage()
  {
    Texture2D target = null;
    var permission = GetImageFromGallery((path) =>
    {
      Debug.Log("Image path: " + path);
      if (path != null)
      {
        Texture2D image = LoadImageAtPath(path, -1, false);
        if (image.height == image.width)
        {
          image.filterMode = FilterMode.Point;
          image.wrapMode = TextureWrapMode.Clamp; ;
          target = image;
        }
      }
    });
    image = target;
  }
}
