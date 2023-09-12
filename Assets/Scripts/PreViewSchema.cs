using System.IO;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using static NativeGallery;

public class PreViewSchema : MonoBehaviour
{
  public Image imgPrefab;
  public GameObject ingParent;
  public GameObject link;
  private void Start()
  {
    link = GameObject.FindGameObjectsWithTag("MainCamera")[0];
    ingParent = link.GetComponent<ListSchemaUIControl>().messageParent;
  }
  public void PreViewShemaWithName()
  {
    string filename = $"{transform.name}.png";
    var path = Path.Combine(Application.persistentDataPath, filename);
    if (File.Exists(path))
    {
      Debug.Log($"файл с именем {transform.name}.png существует");
      Texture2D texture = LoadImageAtPath(path, -1, false);
      var img = Instantiate(imgPrefab, ingParent.transform);

      Rect rect = new(0, 0, texture.width, texture.height);
      var preViewImg = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
      img.sprite = preViewImg;

      Destroy(img, 3);
    }
  }

}
