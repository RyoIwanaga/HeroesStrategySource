using UnityEngine;
using System.Collections;

namespace Scene.Title
{
    /// <summary>
    /// あとでタイトルマネージャーに移動
    /// </summary>
    public class TitleUIController : MonoBehaviour
    {
        public void Start()
        {
            AudioManager.Script().BGMCrossFade(Define.Path.BGM.Title);
        }

        public void OnClickVsAI()
        {
            var args = SceneArgumentManager.NewArgs();
            args.Add(false);
            args.Add(true);

            SceneArgumentManager.LoadScene(Define.Scene.TacticsBattle, args);
        }

        public void OnClickVsHuman()
        {
            var args = SceneArgumentManager.NewArgs();
            args.Add(false);
            args.Add(false);

            SceneArgumentManager.LoadScene(Define.Scene.TacticsBattle, args);
        }
    }
}
