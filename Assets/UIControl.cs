using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NativeGallery;


public class UIControl : MonoBehaviour
{
  #region Поля
  public TMP_InputField InputNodes;
  public TMP_InputField InputLines;
  public TMP_InputField InputWight;
  public TMP_InputField InputSchemaName;
  public Slider ProgressBar;
  public GameObject picPrefab;
  public GameObject picture;

  public int nodes;
  public int lines;
  public float wight;
  public string shape;
  public string schemaName;
  public Texture2D image;
  #endregion
  #region Методы
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
    if (GetComponent<GenerateStringArt>().enabled == false && image != null)
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
      GetComponent<GenerateStringArt>().Schema = new();

      Destroy(picture);
      GetComponent<GenerateStringArt>().enabled = true;
    }
  }
  public void GetImage()
  {
    Destroy(picture);
    image = null;
    var permission = GetImageFromGallery((path) =>
    {
      if (path != null)
      {
        Texture2D img = LoadImageAtPath(path, -1, false);
        if (img.height == img.width)
        {
          var imageScale = ScaleAndCropTexture.ScaleTexture(img, 640, 640);
          imageScale.filterMode = FilterMode.Point;
          imageScale.wrapMode = TextureWrapMode.Clamp;
          image = imageScale;
          SetQuad();
        }
      }
    });
  }
  public void SetQuad()
  {
    picture = Instantiate(picPrefab);
    picture.GetComponent<Renderer>().sharedMaterial.mainTexture = image;
  }
  public void StopAndReset()
  {
    GetComponent<GenerateStringArt>().enabled = false;
    Destroy(GetComponent<GenerateStringArt>().linesContainer);
    ProgressBar.value = 0;
  }
  public void SaveSchemaName()
  {
    schemaName = InputSchemaName.text.Trim();
  }
  public void SaveSchema()
  {
    if (GetComponent<GenerateStringArt>().CurrentStep >= GetComponent<GenerateStringArt>().steps && schemaName != "")
    {
      var path = Application.persistentDataPath + $"/{schemaName}.schema";
      var bf = new BinaryFormatter();
      var fs = new FileStream(path, FileMode.Create);
      bf.Serialize(fs, GetComponent<GenerateStringArt>().Schema);
      fs.Close();
    }
  }
  #endregion
}
