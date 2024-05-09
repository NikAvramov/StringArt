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

  #region Çâóêè
  public AudioClip voiñe_100;
  private AudioSource voiñe_100A;
  public AudioClip voiñe_200;
  private AudioSource voiñe_200A;
  public AudioClip voiñe_300;
  private AudioSource voiñe_300A;
  public AudioClip voiñe_400;
  private AudioSource voiñe_400A;
  public AudioClip voiñe_500;
  private AudioSource voiñe_500A;
  public AudioClip voiñe_11;
  private AudioSource voiñe_11A;
  public AudioClip voiñe_12;
  private AudioSource voiñe_12A;
  public AudioClip voiñe_13;
  private AudioSource voiñe_13A;
  public AudioClip voiñe_14;
  private AudioSource voiñe_14A;
  public AudioClip voiñe_15;
  private AudioSource voiñe_15A;
  public AudioClip voiñe_16;
  private AudioSource voiñe_16A;
  public AudioClip voiñe_17;
  private AudioSource voiñe_17A;
  public AudioClip voiñe_18;
  private AudioSource voiñe_18A;
  public AudioClip voiñe_19;
  private AudioSource voiñe_19A;
  public AudioClip voiñe_10;
  private AudioSource voiñe_10A;
  public AudioClip voiñe_20;
  private AudioSource voiñe_20A;
  public AudioClip voiñe_30;
  private AudioSource voiñe_30A;
  public AudioClip voiñe_40;
  private AudioSource voiñe_40A;
  public AudioClip voiñe_50;
  private AudioSource voiñe_50A;
  public AudioClip voiñe_60;
  private AudioSource voiñe_60A;
  public AudioClip voiñe_70;
  private AudioSource voiñe_70A;
  public AudioClip voiñe_80;
  private AudioSource voiñe_80A;
  public AudioClip voiñe_90;
  private AudioSource voiñe_90A;
  public AudioClip voiñe_1;
  private AudioSource voiñe_1A;
  public AudioClip voiñe_2;
  private AudioSource voiñe_2A;
  public AudioClip voiñe_3;
  private AudioSource voiñe_3A;
  public AudioClip voiñe_4;
  private AudioSource voiñe_4A;
  public AudioClip voiñe_5;
  private AudioSource voiñe_5A;
  public AudioClip voiñe_6;
  private AudioSource voiñe_6A;
  public AudioClip voiñe_7;
  private AudioSource voiñe_7A;
  public AudioClip voiñe_8;
  private AudioSource voiñe_8A;
  public AudioClip voiñe_9;
  private AudioSource voiñe_9A;
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
            case 1: result.Add(voiñe_100A); break;
            case 2: result.Add(voiñe_200A); break;
            case 3: result.Add(voiñe_300A); break;
            case 4: result.Add(voiñe_400A); break;
            case 5: result.Add(voiñe_500A); break;
          }
        }
      }
      else if (number2 == 0)
      {
        switch (number1)
        {
          case 1:
            result.Add(voiñe_100A);
            AddOneLastNumber(number3);
            break;
          case 2:
            result.Add(voiñe_200A);
            AddOneLastNumber(number3);
            break;
          case 3:
            result.Add(voiñe_300A);
            AddOneLastNumber(number3);
            break;
          case 4:
            result.Add(voiñe_400A);
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
            result.Add(voiñe_100A);
            AddTwoLastNumber(inits);
            break;
          case 2:
            result.Add(voiñe_200A);
            AddTwoLastNumber(inits);
            break;
          case 3:
            result.Add(voiñe_300A);
            AddTwoLastNumber(inits);
            break;
          case 4:
            result.Add(voiñe_400A);
            AddTwoLastNumber(inits);
            break;
        }
      }
    }

    //âñïîìîãàòåëüíûé ìåòîä äîáàâëåíèÿ îäíîé öèôðû ñïðàâà
    void AddOneLastNumber(int number)
    {
      switch (number)
      {
        case 1: result.Add(voiñe_1A); break;
        case 2: result.Add(voiñe_2A); break;
        case 3: result.Add(voiñe_3A); break;
        case 4: result.Add(voiñe_4A); break;
        case 5: result.Add(voiñe_5A); break;
        case 6: result.Add(voiñe_6A); break;
        case 7: result.Add(voiñe_7A); break;
        case 8: result.Add(voiñe_8A); break;
        case 9: result.Add(voiñe_9A); break;
      }
    }
    //âñïîìîãàòåëüíûé ìåòîä äîáàâëåíèÿ äâóõ öèôð ñïðàâà
    void AddTwoLastNumber(int[] number)
    {
      int number1 = number[0];
      int number2 = number[1];

      if (number1 == 1 && number2 != 0)
      {
        switch (number2)
        {
          case 1: result.Add(voiñe_11A); break;
          case 2: result.Add(voiñe_12A); break;
          case 3: result.Add(voiñe_13A); break;
          case 4: result.Add(voiñe_14A); break;
          case 5: result.Add(voiñe_15A); break;
          case 6: result.Add(voiñe_16A); break;
          case 7: result.Add(voiñe_17A); break;
          case 8: result.Add(voiñe_18A); break;
          case 9: result.Add(voiñe_19A); break;
        }
      }
      else if (number2 == 0)
      {
        switch (number1)
        {
          case 1: result.Add(voiñe_10A); break;
          case 2: result.Add(voiñe_20A); break;
          case 3: result.Add(voiñe_30A); break;
          case 4: result.Add(voiñe_40A); break;
          case 5: result.Add(voiñe_50A); break;
          case 6: result.Add(voiñe_60A); break;
          case 7: result.Add(voiñe_70A); break;
          case 8: result.Add(voiñe_80A); break;
          case 9: result.Add(voiñe_90A); break;
        }
      }
      else
      {
        switch (number1)
        {
          case 2:
            result.Add(voiñe_20A);
            AddOneLastNumber(number2);
            break;
          case 3:
            result.Add(voiñe_30A);
            AddOneLastNumber(number2);
            break;
          case 4:
            result.Add(voiñe_40A);
            AddOneLastNumber(number2);
            break;
          case 5:
            result.Add(voiñe_50A);
            AddOneLastNumber(number2);
            break;
          case 6:
            result.Add(voiñe_60A);
            AddOneLastNumber(number2);
            break;
          case 7:
            result.Add(voiñe_70A);
            AddOneLastNumber(number2);
            break;
          case 8:
            result.Add(voiñe_80A);
            AddOneLastNumber(number2);
            break;
          case 9:
            result.Add(voiñe_90A);
            AddOneLastNumber(number2);
            break;
        }
      }
    }

    return result;
  }
  private void InitializationVoices()
  {
    voiñe_100A = gameObject.AddComponent<AudioSource>();
    voiñe_100A.clip = voiñe_100;

    voiñe_200A = gameObject.AddComponent<AudioSource>();
    voiñe_200A.clip = voiñe_200;

    voiñe_300A = gameObject.AddComponent<AudioSource>();
    voiñe_300A.clip = voiñe_300;

    voiñe_400A = gameObject.AddComponent<AudioSource>();
    voiñe_400A.clip = voiñe_400;

    voiñe_500A = gameObject.AddComponent<AudioSource>();
    voiñe_500A.clip = voiñe_500;

    voiñe_11A = gameObject.AddComponent<AudioSource>();
    voiñe_11A.clip = voiñe_11;

    voiñe_12A = gameObject.AddComponent<AudioSource>();
    voiñe_12A.clip = voiñe_12;

    voiñe_13A = gameObject.AddComponent<AudioSource>();
    voiñe_13A.clip = voiñe_13;

    voiñe_14A = gameObject.AddComponent<AudioSource>();
    voiñe_14A.clip = voiñe_14;

    voiñe_15A = gameObject.AddComponent<AudioSource>();
    voiñe_15A.clip = voiñe_15;

    voiñe_16A = gameObject.AddComponent<AudioSource>();
    voiñe_16A.clip = voiñe_16;

    voiñe_17A = gameObject.AddComponent<AudioSource>();
    voiñe_17A.clip = voiñe_17;

    voiñe_18A = gameObject.AddComponent<AudioSource>();
    voiñe_18A.clip = voiñe_18;

    voiñe_19A = gameObject.AddComponent<AudioSource>();
    voiñe_19A.clip = voiñe_19;

    voiñe_10A = gameObject.AddComponent<AudioSource>();
    voiñe_10A.clip = voiñe_10;

    voiñe_20A = gameObject.AddComponent<AudioSource>();
    voiñe_20A.clip = voiñe_20;

    voiñe_30A = gameObject.AddComponent<AudioSource>();
    voiñe_30A.clip = voiñe_30;

    voiñe_40A = gameObject.AddComponent<AudioSource>();
    voiñe_40A.clip = voiñe_40;

    voiñe_50A = gameObject.AddComponent<AudioSource>();
    voiñe_50A.clip = voiñe_50;

    voiñe_60A = gameObject.AddComponent<AudioSource>();
    voiñe_60A.clip = voiñe_60;

    voiñe_70A = gameObject.AddComponent<AudioSource>();
    voiñe_70A.clip = voiñe_70;

    voiñe_80A = gameObject.AddComponent<AudioSource>();
    voiñe_80A.clip = voiñe_80;

    voiñe_90A = gameObject.AddComponent<AudioSource>();
    voiñe_90A.clip = voiñe_90;

    voiñe_1A = gameObject.AddComponent<AudioSource>();
    voiñe_1A.clip = voiñe_1;

    voiñe_2A = gameObject.AddComponent<AudioSource>();
    voiñe_2A.clip = voiñe_2;

    voiñe_3A = gameObject.AddComponent<AudioSource>();
    voiñe_3A.clip = voiñe_3;

    voiñe_4A = gameObject.AddComponent<AudioSource>();
    voiñe_4A.clip = voiñe_4;

    voiñe_5A = gameObject.AddComponent<AudioSource>();
    voiñe_5A.clip = voiñe_5;

    voiñe_6A = gameObject.AddComponent<AudioSource>();
    voiñe_6A.clip = voiñe_6;

    voiñe_7A = gameObject.AddComponent<AudioSource>();
    voiñe_7A.clip = voiñe_7;

    voiñe_8A = gameObject.AddComponent<AudioSource>();
    voiñe_8A.clip = voiñe_8;

    voiñe_9A = gameObject.AddComponent<AudioSource>();
    voiñe_9A.clip = voiñe_9;
  }
}
