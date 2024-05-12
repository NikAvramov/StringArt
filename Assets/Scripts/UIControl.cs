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
  public TMP_InputField InputSize;
  public TMP_InputField InputSchemaName;
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
  public int size;
  private bool verifiInputUser = true;
  public CanvasShape shape;
  public string schemaName;
  public Texture2D image;
  #endregion

  #region Методы
  public void SaveCountOfNodes()
  {
    bool result = int.TryParse(InputNodes.text.Trim(), out int inputUser);
    if (result)
    {
      verifiInputUser = true;
      if (inputUser > 500)
      {
        nodes = 500;
      }
      else if (inputUser < 100)
      {
        nodes = 100;
      }
      else
      {
        nodes = inputUser;
      }
    }
    else
    {
      verifiInputUser = false;
    }
  }
  public void SaveCountOfLines()
  {
    bool result = int.TryParse(InputLines.text.Trim(), out int inputUser);
    if (result)
    {
      verifiInputUser = true;
      if (inputUser > 10000)
      {
        lines = 10000;
      }
      else if (inputUser < 500)
      {
        lines = 500;
      }
      else
      {
        lines = inputUser;
      }
    }
    else
    {
      verifiInputUser = false;
    }
    ProgressBar.minValue = 0;
    ProgressBar.maxValue = lines;
  }
  public void SaveWight()
  {
    var verifiString = InputWight.text.Trim().Replace('.', ',');
    bool result = float.TryParse(verifiString, out float inputUser);
    if (result)
    {
      verifiInputUser = true;
      if (inputUser >= 1f)
      {
        wight = 1f;
      }
      else if (inputUser <= 0.01f)
      {
        wight = 0.01f;
      }
      else
      {
        wight = inputUser;
      }
    }
    else
    {
      verifiInputUser = false;
    }
  }
  public void SaveSize()
  {
    bool result = int.TryParse(InputSize.text.Trim(), out int inputUser);
    if (result)
    {
      verifiInputUser = true;
      if (inputUser > 1500)
      {
        size = 1500;
      }
      else if (inputUser < 150)
      {
        size = 150;
      }
      else
      {
        size = inputUser;
      }
    }
    else
    {
      verifiInputUser = false;
    }
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
    if (GetComponent<GenerateStringArt>().enabled == false && image != null && verifiInputUser)
    {
      GetComponent<GenerateStringArt>().countOfPoint = nodes;
      GetComponent<GenerateStringArt>().steps = lines;
      GetComponent<GenerateStringArt>().width = (640 * wight) / size;
      GetComponent<GenerateStringArt>().widthOfFiber = wight;
      GetComponent<GenerateStringArt>().canvas = shape;
      GetComponent<GenerateStringArt>().size = image.width;
      GetComponent<GenerateStringArt>().sizeCanvasInMilimeters = size;
      GetComponent<GenerateStringArt>().pixelArray = GetPixelArray();
      GetComponent<GenerateStringArt>().CurrentStep = 0;
      GetComponent<GenerateStringArt>().nodes = GetComponent<GenerateStringArt>().CreateNodes();
      GetComponent<GenerateStringArt>().activPoint = GetComponent<GenerateStringArt>().nodes[0];
      GetComponent<GenerateStringArt>().direct = new();
      CreateLineRenderer(GetComponent<GenerateStringArt>().activPoint.Coords);
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
    string inputUser = InputSchemaName.text.Trim();
    if (inputUser.Length <= 30 && !string.IsNullOrEmpty(inputUser))
    {
      schemaName = inputUser;
    }
  }
  public void SaveSchema()
  {
    if (GetComponent<GenerateStringArt>().CurrentStep >= GetComponent<GenerateStringArt>().steps && !string.IsNullOrEmpty(schemaName))
    {
      var path = Application.persistentDataPath + $"/{schemaName}.schema";
      if (File.Exists(path))
      {
        File.Delete(path);
      }
      using var fs = new FileStream(path, FileMode.OpenOrCreate);
      var bf = new BinaryFormatter();
      bf.Serialize(fs, GetComponent<GenerateStringArt>().Schema);

      SavePreView();

      var mes = Instantiate(message, messageParent.transform);
      Destroy(mes, 1);
      schemaName = null;
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
  private void SavePreView()
  {
    string filename = $"{schemaName}.png";
    var path = Path.Combine(Application.persistentDataPath, filename);

    if (Application.platform == RuntimePlatform.Android)
      ScreenCapture.CaptureScreenshot(filename);
    else if (Application.platform == RuntimePlatform.WindowsEditor)
      ScreenCapture.CaptureScreenshot(path);
  }
  public void CreateLineRenderer(Vector3 startPoint)
  {
    GetComponent<GenerateStringArt>().linesContainer = new GameObject();
    GetComponent<GenerateStringArt>().linesContainer.transform.position = startPoint;
    var lineRender = GetComponent<GenerateStringArt>().linesContainer.AddComponent<LineRenderer>();
    lineRender.positionCount = lines + 2;
    lineRender.startWidth = (640 * wight) / size;
    lineRender.endWidth = (640 * wight) / size;
    lineRender.material = GetComponent<GenerateStringArt>().material;
    lineRender.SetPosition(0, startPoint);
  }
  #endregion
}
