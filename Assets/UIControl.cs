using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static NativeGallery;

public class UIControl : MonoBehaviour
{
  #region Поля
  public TMP_InputField InputNodes;
  public TMP_InputField InputLines;
  public TMP_InputField InputWight;
  public TMP_InputField InputSchemaName;
  //public TMP_Dropdown ShapeDropdown;
  public Slider ProgressBar;

  public Button startButton;
  public Sprite activButtonImage;
  public Texture2D defaultTexture;
  public Sprite defaultButtonImage;
  public GameObject message;
  public GameObject messageParent;

  public Canvas menu;
  public Canvas workpace;

  public int nodes;
  public int lines;
  public float wight;
  public CanvasShape shape;
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
  public void SaveShapeCanvas(TMP_Dropdown shapeDropdown)
  {
    shape = shapeDropdown.value switch
    {
      0 => CanvasShape.Square,
      1 => CanvasShape.Circle,
      2 => CanvasShape.Triangle,
      3 => CanvasShape.Hexagone,
      _ => CanvasShape.Square
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
      GetComponent<GenerateStringArt>().canvas = shape;
      GetComponent<GenerateStringArt>().image = image;
      GetComponent<GenerateStringArt>().size = image.width;
      GetComponent<GenerateStringArt>().pixelArray = GetPixelArray();
      GetComponent<GenerateStringArt>().CurrentStep = 0;
      GetComponent<GenerateStringArt>().nodes = GetComponent<GenerateStringArt>().CreateNodes();
      GetComponent<GenerateStringArt>().activPoint = GetComponent<GenerateStringArt>().nodes[0];
      GetComponent<GenerateStringArt>().direct = new();
      GetComponent<GenerateStringArt>().linesContainer = new();
      GetComponent<GenerateStringArt>().Schema = new();

      menu.enabled = false;
      workpace.enabled = true;
      GetComponent<GenerateStringArt>().enabled = true;
    }
  }
  public void GetImage()
  {
    image = null;
    //startButton.GetComponent<Image>().sprite = 
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
          
          Rect rect = new(0, 0, image.width, image.height);
          activButtonImage = Sprite.Create(image, rect, new Vector2(0.5f, 0.5f));
          startButton.GetComponent<Image>().sprite = activButtonImage;
        }
      }
    });
  }
  public void StopAndReset()
  {
    GetComponent<GenerateStringArt>().enabled = false;
    Destroy(GetComponent<GenerateStringArt>().linesContainer);
    ProgressBar.value = 0;
    menu.enabled = true;
    workpace.enabled = false;

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
      if (File.Exists(path))
      {
        File.Delete(path);
      }
      using var fs = new FileStream(path, FileMode.OpenOrCreate);
      var bf = new BinaryFormatter();
      bf.Serialize(fs, GetComponent<GenerateStringArt>().Schema);
      var mes = Instantiate(message, messageParent.transform);
      Destroy(mes, 1);
    }
  }
  public void LoadMainMenu()
  {
    SceneManager.LoadScene(0);
  }
  public void LoadListShema()
  {
    SceneManager.LoadScene(2);
  }
  #endregion
}
