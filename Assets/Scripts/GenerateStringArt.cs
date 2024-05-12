using System.Collections.Generic;
using UnityEngine;

public class GenerateStringArt : MonoBehaviour
{
  #region Поля
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
  public Direct direct; //вспомогательная переменная которая будет хранить напраление с наибольшим уровенем серого
  public GameObject linesContainer;
  public List<NodesMap> Schema;

  public float widthOfFiber;
  public int sizeCanvasInMilimeters;

  private List<Vector2> linePixel;//вспомогательные переменные хранящие координаты пикселей в текущей линии
  private float sumGrayScale;//и сумму серого в этой линии
  private Nodes endPoint;

  private List<Vector2> allPixelInLine;//вспомогательная переменная для метода Безенхейма
  private int deltaX;
  private int deltaY;
  private int xPosition;
  private int yPosition;

  #endregion

  void Update()
  {
    if (CurrentStep <= steps)
    {
      direct.Direction = activPoint;//задаем изначальные значения чтобы сравнивать
      direct.GrayScale = 1.1f;
      //в цикле перебираем все направления из активной точи и считаем среднее значение по уровню серого
      for (int j = 0; j < nodes.Count; j++)
      {
        if (nodes[j] != activPoint)
        {
          linePixel = GetAllPixelInLine(activPoint.Coords, nodes[j].Coords);
          sumGrayScale = 0f;
          foreach (var pixel in linePixel)
          {
            sumGrayScale += pixelArray[(int)pixel.x, (int)pixel.y];
          }
          if (sumGrayScale / linePixel.Count < direct.GrayScale)
          {
            direct.GrayScale = sumGrayScale / linePixel.Count;
            direct.Direction = nodes[j];
            direct.PixelsInLine = linePixel;
          }
        }
      }
      endPoint = direct.Direction;
      DrawLineTo(CurrentStep + 1, endPoint.Coords);

      foreach (var pixel in direct.PixelsInLine)
      {
        pixelArray[(int)pixel.x, (int)pixel.y] += width;//добавляем серого там где нарисовали линию
        //ограничиваем диапазон серого между 0 и 1
        pixelArray[(int)pixel.x, (int)pixel.y] = Mathf.Clamp(pixelArray[(int)pixel.x, (int)pixel.y], 0f, 1f);
      }

      Schema.Add(new NodesMap(
        CurrentStep, endPoint.ID, width, (int)endPoint.Coords.x, (int)endPoint.Coords.y, 
        countOfPoint, steps, sizeCanvasInMilimeters, widthOfFiber));

      activPoint = endPoint;

      GetComponent<UIControl>().ProgressBar.value = CurrentStep;
      CurrentStep++;
    }
  }
  //метод для получения координат всех пикселей на линии между 2 пикселями
  //реализация алгоритма Безенхейма
  public List<Vector2> GetAllPixelInLine(Vector2 start, Vector2 end)
  {
    allPixelInLine = new List<Vector2>();
    deltaX = (int)end.x - (int)start.x;
    deltaY = (int)end.y - (int)start.y;

    if (Mathf.Abs(deltaX) >= Mathf.Abs(deltaY))
    {
      if (deltaX > 0)
      {
        for (int i = 0; i < Mathf.Abs(deltaX); i++)
        {
          xPosition = (int)start.x + i;
          yPosition = Mathf.RoundToInt(((float)deltaY / (float)deltaX) * (xPosition - (int)start.x) + start.y);
          allPixelInLine.Add(new Vector2(xPosition, yPosition));
        }
      }
      else
      {
        for (int i = 0; i < Mathf.Abs(deltaX); i++)
        {
          xPosition = (int)start.x - i;
          yPosition = Mathf.RoundToInt(((float)deltaY / (float)deltaX) * (xPosition - (int)start.x) + start.y);
          allPixelInLine.Add(new Vector2(xPosition, yPosition));
        }
      }
    }
    else
    {
      if (deltaY > 0)
      {
        for (int i = 0; i < Mathf.Abs(deltaY); i++)
        {
          yPosition = (int)start.y + i;
          xPosition = Mathf.RoundToInt(((float)deltaX / (float)deltaY) * (yPosition - (int)start.y) + start.x);
          allPixelInLine.Add(new Vector2(xPosition, yPosition));
        }
      }
      else
      {
        for (int i = 0; i < Mathf.Abs(deltaY); i++)
        {
          yPosition = (int)start.y - i;
          xPosition = Mathf.RoundToInt(((float)deltaX / (float)deltaY) * (yPosition - (int)start.y) + start.x);
          allPixelInLine.Add(new Vector2(xPosition, yPosition));
        }
      }
    }
    return allPixelInLine;
  }
  public void DrawLineTo(int index, Vector3 point)
  {
    linesContainer.GetComponent<LineRenderer>().SetPosition(index, point);
  }
  public void DrawAllLines(List<NodesMap> schema)
  {
    linesContainer = new GameObject();
    var lineRender = linesContainer.AddComponent<LineRenderer>();
    lineRender.positionCount = schema.Count + 1;
    lineRender.startWidth = width;
    lineRender.endWidth = width;
    lineRender.material = material;

    var pointArray = new Vector3[schema.Count + 1];
    pointArray[0] = nodes[0].Coords;
    for (int i = 1; i < pointArray.Length; i++)
    {
      pointArray[i] = new Vector3(schema[i - 1].X, schema[i - 1].Y, 0);
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
  public List<Nodes> CreateSquareCanvas()//метод для создания квадратной картины
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
  public List<Nodes> CreateCircleCanvas()//метод для создания круглой картины
  {
    var coords = new List<Nodes>();
    var deltaFi = 360f / countOfPoint;//шаг углового расстояния между двумя соседними точками
    for (int i = 0; i < countOfPoint; i++)
    {
      float fi = i * deltaFi;//текущий угол в грпдусах(полярная координата)
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
  public List<Nodes> CreateTriangleCanvas()//метод для создания треугольной картины
  {
    var coords = new List<Nodes>();

    return coords;
  }
  public List<Nodes> CreateHexagoneCanvas()//метод для создания шестиугольной картины
  {
    var coords = new List<Nodes>();

    return coords;
  }
}
public class Direct//класс, который хранит направление и средний уровень серого в этом направлении
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

  public int TotalCountOfNodes;
  public int TotalCountOfLines;
  public int SizeOfCanvas;
  public float WidthOfFiber;

  public NodesMap(int iDstep, int idnode, float width, int x, int y, int totalNods, int totalLines, int sizeCanvas, float widthFiber)
  {
    IDstep = iDstep;
    IDnode = idnode;
    Width = width;
    X = x;
    Y = y;
    IsReady = false;

    TotalCountOfNodes = totalNods;
    TotalCountOfLines = totalLines;
    SizeOfCanvas = sizeCanvas;
    WidthOfFiber = widthFiber;
  }
  public override string ToString()
  {
    return $"Номер шага {IDstep}, номер гвоздя {IDnode}, Х = {X}, Y = {Y}, статус шага - {IsReady}";
  }
}