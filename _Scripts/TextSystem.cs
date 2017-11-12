using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to instantiate 3D texts
/// </summary>
public class TextSystem : MonoBehaviour
{

    void Start()
    {

    }

    /// <summary>
    /// Creates a custom text
    /// </summary>
    /// <param name="desc"></param>
    /// <param name="name"></param>
    /// <param name="position"></param>
    /// <param name="quat"></param>
    /// <param name="font"></param>
    /// <param name="fontSize"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static GameObject initializeText(string desc, string name, Vector3 position, Quaternion quat, Font font,  int fontSize, Color color)
    {

        GameObject obj = new GameObject();

        obj.transform.rotation = quat;
        obj.transform.position = position;

        TextMesh text = obj.AddComponent<TextMesh>();
        text.name = name;
        text.text = desc;
        text.font = font;
        text.fontSize = fontSize;
        obj.GetComponent<TextMesh>().color = color;
        obj.GetComponent<MeshRenderer>().material = font.material;


        return obj;
    }
}
