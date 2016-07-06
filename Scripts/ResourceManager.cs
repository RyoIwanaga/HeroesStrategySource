using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TacticsBG;

public class ResourceManager
{
    public static ResourceManager Instance = new ResourceManager();

    Object[] prefUnits;


    ResourceManager()
    {
        prefUnits = new Object[(int)Unit.Type.Max];

        this.InitUIs();
        this.InitSprites();
    }


    public Object Load(string path)
    {
//        Debug.Log(DebugUtil.FnName + path);

        return Resources.Load(path);
    }

    #region === UI ===

    Dictionary<string, Object> dictUIs;

    void InitUIs()
    {
        dictUIs = new Dictionary<string, Object>(10);
    }

    public Object LoadUI(string name)
    {
        Object item = null;
        if (! dictUIs.TryGetValue(name, out item))
        {
            item = Resources.Load(Define.Path.UI.BASE + name);
            dictUIs.Add(name, item);
        }

        return item;
    }

    #endregion

    #region === Sprite ===

    Dictionary<string, Sprite> dictSprites;

    void InitSprites()
    {
        dictSprites = new Dictionary<string, Sprite>(10);
    }

    public Sprite LoadSprite(string name)
    {
        Sprite sprite = null;
        if (! dictSprites.TryGetValue(name, out sprite))
        {
            sprite = Resources.Load<Sprite>(name);
            dictSprites.Add(name, sprite);
        }

        return sprite;
    }

    #endregion

    public Object LoadBGM(string path)
    {
        return Resources.Load(Define.Path.BGM.BASE + path);
    }

    /// <summary>
    /// TODO string をcache
    /// </summary>
    public Object GetUnitPrefab(Unit.Type unitType)
    {
        return Resources.Load(Define.Path.Models.BASE + unitType.ToString());
    }
}
