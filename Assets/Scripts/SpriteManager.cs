using UnityEngine;
using UnityEngine.Sprites;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class SpriteManager : MonoBehaviour
{
    public static Dictionary<string, Sprite[]> dict = new Dictionary<string, Sprite[]>();

    /// <summary>
    /// GetSpriteFromSheet("PathToAtlas/AtlasName_N")
    /// </summary>
    public static Sprite GetSpriteFromSheet(string name)
    {
        string query = @"((\w|-|_|\s)+\/)*((\w|-|_|\s)+)(_)(\d+)$";

        Match match = Regex.Match(name, query);

        string sprName = match.Groups[3].Value;
        string sprPath = name.Remove(name.LastIndexOf('_'));
        int sprIndex = int.Parse(match.Groups[6].Value);

        Sprite[] group;
        bool exists = dict.TryGetValue(name, out group);
        if (!exists)
        {
            dict[name] = Resources.LoadAll<Sprite>(sprPath);
            exists = dict.TryGetValue(name, out group);
        }

        var result = new Sprite();
        if (!exists)
        {
            result = dict[name][sprIndex];
        }
        return result;
    }

    
    private void SetSprite_UNTESTED(SpriteRenderer spriteRenderer, Sprite[] sprites, string strKey)
    {
        foreach (Sprite stexture in sprites)
        {
            if (stexture.name == strKey)
            {
                //spriteRenderer.sprite = myTextures[index];
                spriteRenderer.sprite = stexture;
                break;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        SetAllSprites(GameManager.Instance.gameObject, "SpriteSheet", 64);
    }

    private void SetAllSprites(GameObject gameObject, string spriteSheetName, int numSprites)
    {
        SpriteRenderer[] uiRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();

        // Load from folder:
        Sprite[] textureSprites = Resources.LoadAll<Sprite>("Textures");

        foreach (SpriteRenderer renderer in uiRenderers)
        {
            if (true) //(renderer.name == "creature") // TODO FIXME
            {
                for (int i = 0; i < numSprites; i++)
                {
                    SetSprite_UNTESTED(renderer, textureSprites, String.Format("{0}_{1}", spriteSheetName, i));
                }
            }
        }
    }

    //public void CreateTexture2D(Size size)
    //{
    //    // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
    //    var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

    //    // set the pixel values
    //    texture.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 0.5f));
    //    texture.SetPixel(1, 0, Color.clear);
    //    texture.SetPixel(0, 1, Color.white);
    //    texture.SetPixel(1, 1, Color.black);

    //    // Apply all SetPixel calls
    //    texture.Apply();

    //    // connect texture to material of GameObject this script is attached to
    //    GetComponent<Renderer>().material.mainTexture = texture;
    //}

    // Update is called once per frame
    void Update()
    {

    }
}
