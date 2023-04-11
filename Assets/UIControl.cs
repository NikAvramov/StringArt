using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NativeGallery;

public class UIControl : MonoBehaviour
{
  public TMP_InputField InputNodes;
  public TMP_InputField InputLines;
  public TMP_InputField InputWight;
  public Slider ProgressBar;

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
    ProgressBar.minValue = 0;
    ProgressBar.maxValue = lines;
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
  public float[,] GetPixelArray()
  {
    var pixelArray = new float[image.height, image.width];
    for (int i = 0; i < image.height; i++)
    {
      for (int j = 0; j < image.width; j++)
      {
        var color = image.GetPixel(i, j);
        pixelArray[i, j] = color.grayscale;
      }
    }
    return pixelArray;
  }
  public void StartGeneration()
  {
    if(GetComponent<GenerateStringArt>().enabled == false)
    {
      GetComponent<GenerateStringArt>().countOfPoint = nodes;
      GetComponent<GenerateStringArt>().steps = lines;
      GetComponent<GenerateStringArt>().width = wight;
      _ = Enum.TryParse(shape, true, out GetComponent<GenerateStringArt>().canvas);
      GetComponent<GenerateStringArt>().image = image;
      GetComponent<GenerateStringArt>().size = image.width;
      GetComponent<GenerateStringArt>().pixelArray = GetPixelArray();
      GetComponent<GenerateStringArt>().CurrentStep = 0;
      GetComponent<GenerateStringArt>().nodes = GetComponent<GenerateStringArt>().CreateNodes();
      GetComponent<GenerateStringArt>().activPoint = GetComponent<GenerateStringArt>().nodes[0];
      GetComponent<GenerateStringArt>().direct = new();
      GetComponent<GenerateStringArt>().linesContainer = new();
      GetComponent<GenerateStringArt>().enabled = true;
    }
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
  public void StopAndReset()
  {
    GetComponent<GenerateStringArt>().enabled = false;
    Destroy(GetComponent<GenerateStringArt>().linesContainer);
    ProgressBar.value = 0;
  }
}
