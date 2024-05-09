using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
  public GameObject message;
  public GameObject messageParent;

  public float widthLine;
  public Material material;
  //public GameObject currentLine;

  public TMP_Text pointPrefab;
  public TMP_Text activPoint;
  public TMP_Text endPoint;
  public Transform canvas;
  private GameObject[] lines;

  #region �����
  public AudioClip voi�e_100;
  private AudioSource voi�e_100A;
  public AudioClip voi�e_200;
  private AudioSource voi�e_200A;
  public AudioClip voi�e_300;
  private AudioSource voi�e_300A;
  public AudioClip voi�e_400;
  private AudioSource voi�e_400A;
  public AudioClip voi�e_500;
  private AudioSource voi�e_500A;
  public AudioClip voi�e_11;
  private AudioSource voi�e_11A;
  public AudioClip voi�e_12;
  private AudioSource voi�e_12A;
  public AudioClip voi�e_13;
  private AudioSource voi�e_13A;
  public AudioClip voi�e_14;
  private AudioSource voi�e_14A;
  public AudioClip voi�e_15;
  private AudioSource voi�e_15A;
  public AudioClip voi�e_16;
  private AudioSource voi�e_16A;
  public AudioClip voi�e_17;
  private AudioSource voi�e_17A;
  public AudioClip voi�e_18;
  private AudioSource voi�e_18A;
  public AudioClip voi�e_19;
  private AudioSource voi�e_19A;
  public AudioClip voi�e_10;
  private AudioSource voi�e_10A;
  public AudioClip voi�e_20;
  private AudioSource voi�e_20A;
  public AudioClip voi�e_30;
  private AudioSource voi�e_30A;
  public AudioClip voi�e_40;
  private AudioSource voi�e_40A;
  public AudioClip voi�e_50;
  private AudioSource voi�e_50A;
  public AudioClip voi�e_60;
  private AudioSource voi�e_60A;
  public AudioClip voi�e_70;
  private AudioSource voi�e_70A;
  public AudioClip voi�e_80;
  private AudioSource voi�e_80A;
  public AudioClip voi�e_90;
  private AudioSource voi�e_90A;
  public AudioClip voi�e_1;
  private AudioSource voi�e_1A;
  public AudioClip voi�e_2;
  private AudioSource voi�e_2A;
  public AudioClip voi�e_3;
  private AudioSource voi�e_3A;
  public AudioClip voi�e_4;
  private AudioSource voi�e_4A;
  public AudioClip voi�e_5;
  private AudioSource voi�e_5A;
  public AudioClip voi�e_6;
  private AudioSource voi�e_6A;
  public AudioClip voi�e_7;
  private AudioSource voi�e_7A;
  public AudioClip voi�e_8;
  private AudioSource voi�e_8A;
  public AudioClip voi�e_9;
  private AudioSource voi�e_9A;
  #endregion

  void Start()
  {
    InitializationVoices();

    lines = new GameObject[nodes.Count];
    activPoint = Instantiate(pointPrefab, canvas);
    endPoint = Instantiate(pointPrefab, canvas);
    widthLine = nodes[0].Width;

    currentNode = nodes[0];
    int i = 0;
    while (currentNode.IsReady == true && i != nodes.Count - 1)
    {
      i++;
      DrawLine();
      currentNode = nodes[i];
    }

    step.text = (currentNode.IDstep + 1).ToString();
    node.text = currentNode.IDnode.ToString();
    nextNode.text = nodes.FirstOrDefault(n => n.IDstep == currentNode.IDstep + 1).IDnode.ToString();

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
    nodes[currentNode.IDstep].IsReady = false;
    if (currentNode.IDstep > 0)
    {
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
  public void SaveProgress()
  {
    var path = Application.persistentDataPath + $"/{schemaName.text}.schema";
    if (File.Exists(path))
    {
      File.Delete(path);
    }
    using var fs = new FileStream(path, FileMode.OpenOrCreate);
    var bf = new BinaryFormatter();
    bf.Serialize(fs, nodes);

    var mes = Instantiate(message, messageParent.transform);
    Destroy(mes, 1);
  }
  public void AutoPlayOn()
  {
    StartCoroutine(MakeAStep(3.5f));
  }
  public void AutoPlayOff()
  {
    StopAllCoroutines();
  }
  private IEnumerator MakeAStep(float wait)
  {
    while (currentNode.IDstep <= nodes.Count)
    {
      NextStep();
      var nextNodeVoice = VoiceAnalizeNumber(nextNode.text);
      StartCoroutine(PlayVoice(nextNodeVoice));
      yield return new WaitForSeconds(wait);
    }
  }
  private IEnumerator PlayVoice(List<AudioSource> voices)
  {
    foreach (var voice in voices)
    {
      voice.Play();
      yield return new WaitForSeconds(voice.clip.length);
    }
  }
  private List<AudioSource> VoiceAnalizeNumber(string text)
  {
    List<AudioSource> result = new();

    if (text.Length == 1)
    {
      int number = int.Parse(text);
      AddOneLastNumber(number);
    }
    if (text.Length == 2)
    {
      int[] number = new int[text.Length];
      number[0] = text[0] - '0';
      number[1] = text[1] - '0';
      AddTwoLastNumber(number);
    }
    if (text.Length == 3)
    {
      int[] number = new int[text.Length];
      number[0] = text[0] - '0';
      number[1] = text[1] - '0';
      number[2] = text[2] - '0';
      int number1 = number[0];
      int number2 = number[1];
      int number3 = number[2];

      if (number3 == 0)
      {
        if (number2 == 0)
        {
          switch (number1)
          {
            case 1: result.Add(voi�e_100A); break;
            case 2: result.Add(voi�e_200A); break;
            case 3: result.Add(voi�e_300A); break;
            case 4: result.Add(voi�e_400A); break;
            case 5: result.Add(voi�e_500A); break;
          }
        }
      }
      else if (number2 == 0)
      {
        switch (number1)
        {
          case 1:
            result.Add(voi�e_100A);
            AddOneLastNumber(number3);
            break;
          case 2:
            result.Add(voi�e_200A);
            AddOneLastNumber(number3);
            break;
          case 3:
            result.Add(voi�e_300A);
            AddOneLastNumber(number3);
            break;
          case 4:
            result.Add(voi�e_400A);
            AddOneLastNumber(number3);
            break;
        }
      }
      else
      {
        var inits = new int[] { number2, number3 };
        switch (number1)
        {
          case 1:
            result.Add(voi�e_100A);
            AddTwoLastNumber(inits);
            break;
          case 2:
            result.Add(voi�e_200A);
            AddTwoLastNumber(inits);
            break;
          case 3:
            result.Add(voi�e_300A);
            AddTwoLastNumber(inits);
            break;
          case 4:
            result.Add(voi�e_400A);
            AddTwoLastNumber(inits);
            break;
        }
      }
    }

    //��������������� ����� ���������� ����� ����� ������
    void AddOneLastNumber(int number)
    {
      switch (number)
      {
        case 1: result.Add(voi�e_1A); break;
        case 2: result.Add(voi�e_2A); break;
        case 3: result.Add(voi�e_3A); break;
        case 4: result.Add(voi�e_4A); break;
        case 5: result.Add(voi�e_5A); break;
        case 6: result.Add(voi�e_6A); break;
        case 7: result.Add(voi�e_7A); break;
        case 8: result.Add(voi�e_8A); break;
        case 9: result.Add(voi�e_9A); break;
      }
    }
    //��������������� ����� ���������� ���� ���� ������
    void AddTwoLastNumber(int[] number)
    {
      int number1 = number[0];
      int number2 = number[1];

      if (number1 == 1 && number2 != 0)
      {
        switch (number2)
        {
          case 1: result.Add(voi�e_11A); break;
          case 2: result.Add(voi�e_12A); break;
          case 3: result.Add(voi�e_13A); break;
          case 4: result.Add(voi�e_14A); break;
          case 5: result.Add(voi�e_15A); break;
          case 6: result.Add(voi�e_16A); break;
          case 7: result.Add(voi�e_17A); break;
          case 8: result.Add(voi�e_18A); break;
          case 9: result.Add(voi�e_19A); break;
        }
      }
      else if (number2 == 0)
      {
        switch (number1)
        {
          case 1: result.Add(voi�e_10A); break;
          case 2: result.Add(voi�e_20A); break;
          case 3: result.Add(voi�e_30A); break;
          case 4: result.Add(voi�e_40A); break;
          case 5: result.Add(voi�e_50A); break;
          case 6: result.Add(voi�e_60A); break;
          case 7: result.Add(voi�e_70A); break;
          case 8: result.Add(voi�e_80A); break;
          case 9: result.Add(voi�e_90A); break;
        }
      }
      else
      {
        switch (number1)
        {
          case 2:
            result.Add(voi�e_20A);
            AddOneLastNumber(number2);
            break;
          case 3:
            result.Add(voi�e_30A);
            AddOneLastNumber(number2);
            break;
          case 4:
            result.Add(voi�e_40A);
            AddOneLastNumber(number2);
            break;
          case 5:
            result.Add(voi�e_50A);
            AddOneLastNumber(number2);
            break;
          case 6:
            result.Add(voi�e_60A);
            AddOneLastNumber(number2);
            break;
          case 7:
            result.Add(voi�e_70A);
            AddOneLastNumber(number2);
            break;
          case 8:
            result.Add(voi�e_80A);
            AddOneLastNumber(number2);
            break;
          case 9:
            result.Add(voi�e_90A);
            AddOneLastNumber(number2);
            break;
        }
      }
    }

    return result;
  }
  private void InitializationVoices()
  {
    voi�e_100A = gameObject.AddComponent<AudioSource>();
    voi�e_100A.clip = voi�e_100;

    voi�e_200A = gameObject.AddComponent<AudioSource>();
    voi�e_200A.clip = voi�e_200;

    voi�e_300A = gameObject.AddComponent<AudioSource>();
    voi�e_300A.clip = voi�e_300;

    voi�e_400A = gameObject.AddComponent<AudioSource>();
    voi�e_400A.clip = voi�e_400;

    voi�e_500A = gameObject.AddComponent<AudioSource>();
    voi�e_500A.clip = voi�e_500;

    voi�e_11A = gameObject.AddComponent<AudioSource>();
    voi�e_11A.clip = voi�e_11;

    voi�e_12A = gameObject.AddComponent<AudioSource>();
    voi�e_12A.clip = voi�e_12;

    voi�e_13A = gameObject.AddComponent<AudioSource>();
    voi�e_13A.clip = voi�e_13;

    voi�e_14A = gameObject.AddComponent<AudioSource>();
    voi�e_14A.clip = voi�e_14;

    voi�e_15A = gameObject.AddComponent<AudioSource>();
    voi�e_15A.clip = voi�e_15;

    voi�e_16A = gameObject.AddComponent<AudioSource>();
    voi�e_16A.clip = voi�e_16;

    voi�e_17A = gameObject.AddComponent<AudioSource>();
    voi�e_17A.clip = voi�e_17;

    voi�e_18A = gameObject.AddComponent<AudioSource>();
    voi�e_18A.clip = voi�e_18;

    voi�e_19A = gameObject.AddComponent<AudioSource>();
    voi�e_19A.clip = voi�e_19;

    voi�e_10A = gameObject.AddComponent<AudioSource>();
    voi�e_10A.clip = voi�e_10;

    voi�e_20A = gameObject.AddComponent<AudioSource>();
    voi�e_20A.clip = voi�e_20;

    voi�e_30A = gameObject.AddComponent<AudioSource>();
    voi�e_30A.clip = voi�e_30;

    voi�e_40A = gameObject.AddComponent<AudioSource>();
    voi�e_40A.clip = voi�e_40;

    voi�e_50A = gameObject.AddComponent<AudioSource>();
    voi�e_50A.clip = voi�e_50;

    voi�e_60A = gameObject.AddComponent<AudioSource>();
    voi�e_60A.clip = voi�e_60;

    voi�e_70A = gameObject.AddComponent<AudioSource>();
    voi�e_70A.clip = voi�e_70;

    voi�e_80A = gameObject.AddComponent<AudioSource>();
    voi�e_80A.clip = voi�e_80;

    voi�e_90A = gameObject.AddComponent<AudioSource>();
    voi�e_90A.clip = voi�e_90;

    voi�e_1A = gameObject.AddComponent<AudioSource>();
    voi�e_1A.clip = voi�e_1;

    voi�e_2A = gameObject.AddComponent<AudioSource>();
    voi�e_2A.clip = voi�e_2;

    voi�e_3A = gameObject.AddComponent<AudioSource>();
    voi�e_3A.clip = voi�e_3;

    voi�e_4A = gameObject.AddComponent<AudioSource>();
    voi�e_4A.clip = voi�e_4;

    voi�e_5A = gameObject.AddComponent<AudioSource>();
    voi�e_5A.clip = voi�e_5;

    voi�e_6A = gameObject.AddComponent<AudioSource>();
    voi�e_6A.clip = voi�e_6;

    voi�e_7A = gameObject.AddComponent<AudioSource>();
    voi�e_7A.clip = voi�e_7;

    voi�e_8A = gameObject.AddComponent<AudioSource>();
    voi�e_8A.clip = voi�e_8;

    voi�e_9A = gameObject.AddComponent<AudioSource>();
    voi�e_9A.clip = voi�e_9;
  }
}
