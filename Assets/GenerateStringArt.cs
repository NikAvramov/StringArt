using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateStringArt : MonoBehaviour
{
  #region ����
  public int size;
  public int countOfPoint;
  public float width;
  public Material material;
  public int steps;
  public int CurrentStep { get; set; }
  public List<Vector2> nodes;
  public Texture2D image;
  public CanvasShape canvas;

  public float[,] pixelArray;
  public Vector2 activPoint;
  public Direct direct; //��������������� ���������� ������� ����� ������� ���������� � ���������� �������� ������
  public GameObject linesContainer;
  #endregion
  
  void Update()
  {
    if (CurrentStep <= steps)
    {
      direct.Direction = activPoint;//������ ����������� �������� ����� ����������
      direct.GrayScale = 1.1f;
      //� ����� ���������� ��� ����������� �� �������� ���� � ������� ������� �������� �� ������ ������
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
            direct.PixelsInLine = linePixel;
          }
        }
      }
      var endPoint = direct.Direction;
      DrawLine(activPoint, endPoint);

      foreach (var pixel in direct.PixelsInLine)
      {
        pixelArray[(int)pixel.x, (int)pixel.y] += width;//��������� ������ ��� ��� ���������� �����
        //������������ �������� ������ ����� 0 � 1
        pixelArray[(int)pixel.x, (int)pixel.y] = Mathf.Clamp(pixelArray[(int)pixel.x, (int)pixel.y], 0f, 1f);
      }
      activPoint = endPoint;
      GetComponent<UIControl>().ProgressBar.value = CurrentStep; 
      CurrentStep++;
    }
  }
  //����� ��� ��������� ��������� ���� �������� �� ����� ����� 2 ���������
  //���������� ��������� ����������
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
    line.transform.SetParent(linesContainer.transform);
    var lineRender = line.AddComponent<LineRenderer>();
    lineRender.positionCount = 2;
    lineRender.startWidth = width;
    lineRender.endWidth = width;
    lineRender.material = material;
    lineRender.SetPositions(new Vector3[] { start, end });
  }
  public List<Vector2> CreateNodes()
  {
    return canvas switch
    {
      CanvasShape.Square => CreateSquareCanvas(),
      CanvasShape.Circle => CreateCircleCanvas(),
      CanvasShape.Triangle => CreateTriangleCanvas(),
      CanvasShape.Hexagone => CreateHexagoneCanvas(),
      _ => null
    };
  }
  public List<Vector2> CreateSquareCanvas()//����� ��� �������� ���������� �������
  {
    var coords = new List<Vector2>();
    int countNodeInSide = countOfPoint / 4;
    int delta = size / countNodeInSide;
    for (int i = 0; i < countNodeInSide; i++)
    {
      coords.Add(new Vector2(0, i * delta));
      coords.Add(new Vector2(i * delta, size - 1));
      coords.Add(new Vector2(size - 1, size - i * delta - 1));
      coords.Add(new Vector2(size - i * delta - 1, 0));
    }
    return coords;
  }
  public List<Vector2> CreateCircleCanvas()//����� ��� �������� ������� �������
  {
    var coords = new List<Vector2>();
    var deltaFi = 360f / countOfPoint;//��� �������� ���������� ����� ����� ��������� �������
    for (int i = 0; i < countOfPoint; i++)
    {
      float fi = 0 + i * deltaFi;//������� ����(�������� ����������)
      float r = (float)size / 2;

      float x = r * Mathf.Cos(fi) + r;
      x = Mathf.Clamp(x, 0, size - 1);
      x = Mathf.Round(x);
      float y = r * Mathf.Sin(fi) + r;
      y = Mathf.Clamp(y, 0, size - 1);
      y = Mathf.Round(y);
      coords.Add(new Vector2(x, y));
    }
    return coords;
  }
  public List<Vector2> CreateTriangleCanvas()//����� ��� �������� ����������� �������
  {
    var coords = new List<Vector2>();

    return coords;
  }
  public List<Vector2> CreateHexagoneCanvas()//����� ��� �������� ������������� �������
  {
    var coords = new List<Vector2>();

    return coords;
  }
}
public class Direct//�����, ������� ������ ����������� � ������� ������� ������ � ���� �����������
{
  public float GrayScale { get; set; }
  public Vector2 Direction { get; set; }
  public List<Vector2> PixelsInLine { get; set; }
}
public enum CanvasShape
{
  Square,
  Circle,
  Triangle,
  Hexagone,
}