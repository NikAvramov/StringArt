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
  public List<Nodes> nodes;
  public CanvasShape canvas;
  public float[,] pixelArray;
  public Nodes activPoint;
  public Direct direct; //��������������� ���������� ������� ����� ������� ���������� � ���������� �������� ������
  public GameObject linesContainer;
  public List<NodesMap> Schema;
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
          var linePixel = GetAllPixelInLine(activPoint.Coords, nodes[j].Coords);
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
      DrawLineTo(CurrentStep + 1, endPoint.Coords);

      foreach (var pixel in direct.PixelsInLine)
      {
        pixelArray[(int)pixel.x, (int)pixel.y] += width;//��������� ������ ��� ��� ���������� �����
        //������������ �������� ������ ����� 0 � 1
        pixelArray[(int)pixel.x, (int)pixel.y] = Mathf.Clamp(pixelArray[(int)pixel.x, (int)pixel.y], 0f, 1f);
      }

      Schema.Add(new NodesMap(CurrentStep, endPoint.ID, width, (int)endPoint.Coords.x, (int)endPoint.Coords.y));
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
  public void DrawLineTo(int index, Vector3 point)
  {
    linesContainer.GetComponent<LineRenderer>().SetPosition(index, point);
  }
  public void DrawAllLines(List<NodesMap> schema)
  {
    var allLine = new GameObject();
    allLine.transform.SetParent(linesContainer.transform);
    var lineRender = allLine.AddComponent<LineRenderer>();
    lineRender.positionCount = schema.Count + 1;
    lineRender.startWidth = width;
    lineRender.endWidth = width;
    lineRender.material = material;

    var pointArray = new Vector3[schema.Count + 1];
    pointArray[0] = new Vector3(0, 0, 0);
    for (int i = 0; i < pointArray.Length; i++)
    {
      pointArray[i + 1] = new Vector3(schema[i].X, schema[i].Y, 0);
    }

    lineRender.SetPositions(pointArray);
  }
  public List<Nodes> CreateNodes()
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
  public List<Nodes> CreateSquareCanvas()//����� ��� �������� ���������� �������
  {
    var coords = new List<Nodes>();
    int countNodeInSide = countOfPoint / 4;
    int delta = size / countNodeInSide;
    for (int i = 0; i < countNodeInSide; i++)
    {
      coords.Add(new Nodes(i, new Vector2(0, i * delta)));
      coords.Add((new Nodes(i + countNodeInSide, new Vector2(i * delta, size - 1))));
      coords.Add((new Nodes(i + 2 * countNodeInSide, new Vector2(size - 1, size - i * delta - 1))));
      coords.Add((new Nodes(i + 3 * countNodeInSide, new Vector2(size - i * delta - 1, 0))));
    }
    return coords;
  }
  public List<Nodes> CreateCircleCanvas()//����� ��� �������� ������� �������
  {
    var coords = new List<Nodes>();
    var deltaFi = 360f / countOfPoint;//��� �������� ���������� ����� ����� ��������� �������
    for (int i = 0; i < countOfPoint; i++)
    {
      float fi = i * deltaFi;//������� ���� � ��������(�������� ����������)
      float fiRad = fi * Mathf.PI / 180;
      float r = (float)size / 2;

      float x = r * Mathf.Cos(fiRad) + r;
      x = Mathf.Clamp(x, 0, size - 1);
      x = Mathf.Round(x);
      float y = r * Mathf.Sin(fiRad) + r;
      y = Mathf.Clamp(y, 0, size - 1);
      y = Mathf.Round(y);
      coords.Add(new Nodes(i, new Vector2(x, y)));
    }
    return coords;
  }
  public List<Nodes> CreateTriangleCanvas()//����� ��� �������� ����������� �������
  {
    var coords = new List<Nodes>();

    return coords;
  }
  public List<Nodes> CreateHexagoneCanvas()//����� ��� �������� ������������� �������
  {
    var coords = new List<Nodes>();

    return coords;
  }
}
public class Direct//�����, ������� ������ ����������� � ������� ������� ������ � ���� �����������
{
  public float GrayScale { get; set; }
  public Nodes Direction { get; set; }
  public List<Vector2> PixelsInLine { get; set; }
}
public enum CanvasShape
{
  Square,
  Circle,
  Triangle,
  Hexagone,
}
public class Nodes
{
  public int ID;
  public Vector2 Coords;

  public Nodes(int id, Vector2 coords)
  {
    ID = id;
    Coords = coords;
  }
}
[System.Serializable]
public class NodesMap
{
  public int IDstep;
  public int IDnode;
  public float Width;
  public int X;
  public int Y;
  public bool IsReady;
  public NodesMap(int iDstep, int idnode, float width, int x, int y)
  {
    IDstep = iDstep;
    IDnode = idnode;
    Width = width;
    X = x;
    Y = y;
    IsReady = false;
  }
  public override string ToString()
  {
    return $"����� ���� {IDstep}, ����� ������ {IDnode}, � = {X}, Y = {Y}, ������ ���� - {IsReady}";
  }
}