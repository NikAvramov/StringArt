using System.Collections.Generic;
using UnityEngine;

public class CreateLinesFromPoint : MonoBehaviour
{
  public int size;
  public int countOfPoint;
  public float width;
  public Material material;
  public int steps;
  public List<Vector2> nodes;
  public Texture2D image;
  public float[,] pixelArray;
  void Start()
  {
    //читаем картинку попиксельно и записываем значения серого в двумерный массив
    pixelArray = new float[image.height, image.width];

    for (int i = 0; i < image.height; i++)
    {
      for (int j = 0; j < image.width; j++)
      {
        var color = image.GetPixel(i, j);
        pixelArray[i, j] = color.grayscale;
      }
    }
    //создаем список из точек которые является узлами по периметру
    nodes = new();
    int countNodeInSide = countOfPoint / 4;
    int delta = size / countNodeInSide;

    for (int i = 0; i < countNodeInSide; i++)
    {
      nodes.Add(new Vector2(0, i * delta));
      nodes.Add(new Vector2(i * delta, size - 1));
      nodes.Add(new Vector2(size - 1, size - i * delta - 1));
      nodes.Add(new Vector2(size - i * delta - 1, 0));
    }
    //тут заготовка на круглое изображение
    //var deltaFi = 360f / countOfPoint;//шаг углового расстояния между двумя соседними точками
    //for (int i = 0; i < countOfPoint; i++)
    //{
    //  float fi = 0 + i * deltaFi;//текущийу угол(полярная координата)
    //  nodes.Add(new Vector2 (radius * Mathf.Cos(fi), radius * Mathf.Sin(fi)));
    //}
    //берем точку и смотрим в каком напрвлении нужна самая темная линия, провилим ее там и редактируем массив с серыми,
    //точка куда привели линию становится новой стартовой точкой, и так  steps раз
    var activPoint = nodes[0];

    for (int i = 0; i < steps; i++)
    {
      var direct = new Direct(1f, activPoint);//задаем изначальные значения чтобы сравнивать
      for (int j = 0; j < nodes.Count; j++)
      {
        if (nodes[j] != activPoint)
        {
          var linePixel = GetAllPixelInLine(activPoint, nodes[j]);
          float sumGrayScale = 0f;
          foreach (var pixel in linePixel)
          {
            sumGrayScale += pixelArray[(int)pixel.x, (int)pixel.y];
          }
          float mediana = sumGrayScale / linePixel.Count;
          if (mediana < direct.GrayScale)
          {
            direct.GrayScale = mediana;
            direct.Direction = nodes[j];
          }
        }
      }
      var endPoint = direct.Direction;
      DrawLine(activPoint, endPoint);

      var line = GetAllPixelInLine(activPoint, endPoint);
      foreach (var pixel in line)
      {
        pixelArray[(int)pixel.x, (int)pixel.y] += width;//добавляем серого там где нарисовали линию
        Mathf.Clamp(pixelArray[(int)pixel.x, (int)pixel.y], 0f, 1f);
      }
      activPoint = endPoint;
    }

  }
  public List<Vector2> GetAllPixelInLine(Vector2 start, Vector2 end)
  {
    var values = new List<Vector2>();

    var deltaX = (int)end.x - (int)start.x;
    var deltaY = (int)end.y - (int)start.y;

    if (Mathf.Abs(deltaX) >= Mathf.Abs(deltaY))
    {
      if (deltaX > 0)
      {
        for (int i = 0; i < Mathf.Abs(deltaX); i++)
        {
          int x = (int)start.x + i;
          int y = Mathf.RoundToInt(((float)deltaY / (float)deltaX) * (x - (int)start.x) + start.y);
          var coord = new Vector2(x, y);
          values.Add(coord);
        }
      }
      else
      {
        for (int i = 0; i < Mathf.Abs(deltaX); i++)
        {
          int x = (int)start.x - i;
          int y = Mathf.RoundToInt(((float)deltaY / (float)deltaX) * (x - (int)start.x) + start.y);
          var coord = new Vector2(x, y);
          values.Add(coord);
        }
      }
    }
    else
    {
      if (deltaY > 0)
      {
        for (int i = 0; i < Mathf.Abs(deltaY); i++)
        {
          int y = (int)start.y + i;
          int x = Mathf.RoundToInt(((float)deltaX / (float)deltaY) * (y - (int)start.y) + start.x);
          var coord = new Vector2(x, y);
          values.Add(coord);
        }
      }
      else
      {
        for (int i = 0; i < Mathf.Abs(deltaY); i++)
        {
          int y = (int)start.y - i;
          int x = Mathf.RoundToInt(((float)deltaX / (float)deltaY) * (y - (int)start.y) + start.x);
          var coord = new Vector2(x, y);
          values.Add(coord);
        }
      }
    }
    return values;
  }
  public void DrawLine(Vector2 start, Vector2 end)
  {
    var line = new GameObject();
    var lineRender = line.AddComponent<LineRenderer>();
    lineRender.positionCount = 2;
    lineRender.startWidth = width;
    lineRender.endWidth = width;
    lineRender.material = material;
    lineRender.SetPositions(new Vector3[] { start, end });
  }
}
public class Direct
{
  public float GrayScale { get; set; }
  public Vector2 Direction { get; set; }
  public Direct(float grayScale, Vector2 direction)
  {
    GrayScale = grayScale;
    Direction = direction;
  }
}
