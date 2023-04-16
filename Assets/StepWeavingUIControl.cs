using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StepWeavingUIControl : MonoBehaviour
{
  public List<NodesMap> nodes;
  public NodesMap currentNode;
  public TMP_Text step;
  public TMP_Text node;
  public TMP_Text nextNode;
  public TMP_Text schemaName;
  public float widthLine;
  public Material material;
  public GameObject currentLine;
  public TMP_Text pointPrefab;
  public TMP_Text activPoint;
  public TMP_Text endPoint;
  public Transform canvas;
  private GameObject[] lines;
  void Start()
  {
    currentNode = nodes.FirstOrDefault(n => n.IsReady == false);
  
    step.text = (currentNode.IDstep + 1).ToString();
    node.text = currentNode.IDnode.ToString();
    nextNode.text = nodes.FirstOrDefault(n => n.IDstep == currentNode.IDstep + 1).IDnode.ToString();
    widthLine = nodes[0].Width;

    lines = new GameObject[nodes.Count];

    activPoint = Instantiate(pointPrefab, canvas);
    endPoint = Instantiate(pointPrefab, canvas);
    DrawLine();
  }
  public void DrawLine()
  {
    Vector2 start = new(currentNode.X, currentNode.Y);
    var nextNode = nodes.FirstOrDefault(n => n.IDstep == currentNode.IDstep + 1);
    Vector2 end = new(nextNode.X, nextNode.Y);

    var line = new GameObject();
    lines[currentNode.IDstep] = line;
    var lineRender = line.AddComponent<LineRenderer>();
    lineRender.positionCount = 2;
    lineRender.startWidth = widthLine;
    lineRender.endWidth = widthLine;
    lineRender.material = material;
    lineRender.SetPositions(new Vector3[] { start, end });

    activPoint.transform.position = start;
    activPoint.text = currentNode.IDnode.ToString();
    endPoint.transform.position = end;
    endPoint.text = nextNode.IDnode.ToString();
  }
  public void NextStep()
  {
    if (currentNode.IDstep < nodes.Count - 1)
    {
      nodes[currentNode.IDstep].IsReady = true;
      currentNode = nodes.FirstOrDefault(n => n.IDstep == currentNode.IDstep + 1);
      step.text = (currentNode.IDstep + 1).ToString();
      node.text = currentNode.IDnode.ToString();
      nextNode.text = nodes.FirstOrDefault(n => n.IDstep == currentNode.IDstep + 1).IDnode.ToString();
      DrawLine();
    }
  }
  public void PreviousStep()
  {
    if(currentNode.IDstep > 0)
    {
      nodes[currentNode.IDstep].IsReady = false;
      Destroy(lines[currentNode.IDstep]);
      lines[currentNode.IDstep] = null;
      currentNode = nodes.FirstOrDefault(n => n.IDstep == currentNode.IDstep - 1);
      step.text = (currentNode.IDstep + 1).ToString();
      node.text = currentNode.IDnode.ToString();
      nextNode.text = nodes.FirstOrDefault(n => n.IDstep == currentNode.IDstep + 1).IDnode.ToString();

      Vector2 start = new(currentNode.X, currentNode.Y);
      var nextPointNode = nodes.FirstOrDefault(n => n.IDstep == currentNode.IDstep + 1);
      Vector2 end = new(nextPointNode.X, nextPointNode.Y);
      activPoint.transform.position = start;
      activPoint.text = currentNode.IDnode.ToString();
      endPoint.transform.position = end;
      endPoint.text = nextPointNode.IDnode.ToString();
    }
  }
}
