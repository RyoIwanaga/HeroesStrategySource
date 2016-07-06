using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Scene 間のパラメータのやり取りを制御
/// TODO もっと作りこむ
/// </summary>
public static class SceneArgumentManager
{
    static List<object> _args = null;

    public static void LoadScene(string str)
    {
        SceneManager.LoadScene(str);
    }

    public static void LoadScene(string str, List<object> args)
    {
        SceneManager.LoadScene(str);
        _args = args; 
    }

    public static List<object> NewArgs()
    {
        return new List<object>();
    }

    public static List<object> ReceiveArgs()
    {
        if (_args != null)
        {
            var ret = _args;
            _args = null;

            return ret;
        }
        else
        {
            return null;
        }
    }
}
