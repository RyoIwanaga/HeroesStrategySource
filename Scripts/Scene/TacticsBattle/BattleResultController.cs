using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleResultController : MonoBehaviour
{
    static float COLOR_PLAYER_ICON_ALPHA_WIN = 1f;
    static float COLOR_PLAYER_ICON_ALPHA_LOSE = 30f / 255f;
    static Color COLOR_PLAYER_ICON_WIN = new Color(0f, 60f / 255f, 180f / 255f, 1f);
    static Color COLOR_PLAYER_ICON_LOSE = new Color(1f, 0f, 0f, 1f);

    public Text Title;
    public Text PlayerNameLeft;
    public Text PlayerNameRight;
    public Image PlayerImageLeft;
    public Image PlayerImageRight;

    System.Action _callbackOnClickLeft;
    public System.Action CallbackOnClickLeftButton { set { _callbackOnClickLeft = value; } } // setonly

    System.Action _callbackOnClickRight;
    public System.Action CallbackOnClickRightButton { set { _callbackOnClickRight = value; } } // setonly

    /// <summary>
    /// Delegate replay and back to title function.
    /// </summary>
    public void Init(string leftPlayerName, string rightPlayerName, bool isLeftWin, bool isPlayerVsPlayer, System.Action lButtonCallback, System.Action rButtonCallback)
    {
        PlayerNameLeft.text = leftPlayerName;
        PlayerNameRight.text = rightPlayerName;

        if (isPlayerVsPlayer)
        {
            Title.text = "対戦結果";
        }
        else if(isLeftWin)
        {
            Title.text = "戦いに勝利しました";
        }
        else
        {
            Title.text = "戦いに敗北しました";
        }

        if(isLeftWin)
        {
            Color LColor = COLOR_PLAYER_ICON_WIN;
            LColor.a = COLOR_PLAYER_ICON_ALPHA_WIN;

            Color RColor = COLOR_PLAYER_ICON_WIN;
            RColor.a = COLOR_PLAYER_ICON_ALPHA_LOSE;

            PlayerImageLeft.color = LColor;
            PlayerImageRight.color = RColor;
        }
        else
        {
            Color LColor = COLOR_PLAYER_ICON_LOSE;
            LColor.a = COLOR_PLAYER_ICON_ALPHA_LOSE;

            Color RColor = COLOR_PLAYER_ICON_LOSE;
            RColor.a = COLOR_PLAYER_ICON_ALPHA_WIN;

            PlayerImageLeft.color = LColor;
            PlayerImageRight.color = RColor;
        }

        this.CallbackOnClickLeftButton = lButtonCallback;
        this.CallbackOnClickRightButton = rButtonCallback;
    }

    public void OnClickLeft()
    {
        if (_callbackOnClickLeft != null)
        {
            this.gameObject.SetActive(false);
            _callbackOnClickLeft();
        }
    }

    public void OnClickRight()
    {
        if (_callbackOnClickRight != null)
            _callbackOnClickRight();
    }
}
