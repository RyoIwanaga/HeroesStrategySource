using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

using Basic;

namespace Scene.TacticsBattle
{
    public class FloorButtonController : MonoBehaviour
    {
        enum Mode
        {
            // Do not draw
            None,
            // Only use Grid image
            GridOnly,

            Movable,
            CurrentUnit,
            Target,
            AttackDirection,
        };

        const float SECOND_FADE_IN = 1f;
        const float SECOND_FADE_OUT = 0.5f;

        #region = Color Settings =

        static Color COLOR_FIRST = new Color(0f, 0f, 0f, 0f);
        static Color COLOR_GRID_FIRST = new Color(0f, 0f, 0f, 0f);

        static Color COLOR_GRIDONLY = new Color(0f, 0f, 0f, 0f / 255f);
        static Color COLOR_GRID_GRIDONLY = new Color(1f, 1f, 1f, 80f / 255f);

        static Color COLOR_MOVABLE = new Color(0f, 255f / 255f, 0f, 80f / 255f);
        static Color COLOR_GRID_MOVABLE = new Color(0f, 255f / 255f, 0f, 160f / 255f);

        static Color COLOR_TARGET = new Color(255f / 255f, 0f, 0f, 80f / 255f);
        static Color COLOR_GRID_TARGET = new Color(255f / 255f, 0f / 255f, 0f, 160f / 255f);

        static Color COLOR_CURRENT = new Color(0f / 255f, 140f / 255f, 255f / 255f, 80f / 255f);
        static Color COLOR_GRID_CURRENT = new Color(0f / 255f, 140f / 255f, 255f / 255f, 160f / 255f);
        //    static Color COLOR_CURRENT = new Color(50f  / 255f, 150f  / 255f, 255f / 255f, 80f / 255f);
        //    static Color COLOR_GRID_CURRENT = new Color(50f  / 255f, 150f  / 255f, 255f / 255f, 160f / 255f);

        static Color COLOR_ATTACK_DIRECTION = new Color(200f / 255f, 255f / 255f, 50f / 255f, 80f / 255f);
        static Color COLOR_GRID_ATTACK_DIRECTION = new Color(200f / 255f, 255f / 255f, 50f / 255f, 160f / 255f);

        static Color COLOR_ATTACK_DIRECTION_CURRENT = new Color(80f / 255f, 80f / 255f, 80f / 255f, 80f / 255f);
        static Color COLOR_GRID_ATTACK_DIRECTION_CURRENT = new Color(200f / 255f, 255f / 255f, 50f / 255f, 160f / 255f);

        #endregion

        Mode mode;

        public delegate void CallbackOnPress();
        CallbackOnPress callbackOnPress = null;

        Coroutine_.Sequencer coGrid;
        Coroutine_.Sequencer coFloor;

        //    Coroutine fadingAnimation;

        // -- Floor --

        [SerializeField]
        RectTransform rectTrans = null;
        [SerializeField]
        Image image = null;
        [SerializeField]
        Button button = null;

        // -- Grid --

        [SerializeField]
        RectTransform rectTransGrid = null;
        [SerializeField]
        Image imageGrid = null;



        public void Init(Vector2 pos, float width)
        {
            Debug.Assert(this.rectTrans != null);
            Debug.Assert(this.image != null);

            this.coGrid = new Coroutine_.Sequencer(this);
            this.coFloor = new Coroutine_.Sequencer(this);

            this.transform.position = new Vector3(pos.x, 0.2f, pos.y);

            // adjust size
            var size = new Vector2(width, width);
            this.rectTrans.sizeDelta = size;
            this.rectTransGrid.sizeDelta = size;


            this.mode = Mode.None;
            this.image.color = COLOR_FIRST;
            this.imageGrid.color = COLOR_GRID_FIRST;
            this.ChangeModeGridOnly();
        }

        public void ChangeModeGridOnly()
        {
            if (mode == Mode.GridOnly)
            {
                return;
            }
            mode = Mode.GridOnly;

            coGrid.StopCurrent();
            coFloor.StopCurrent();

            coGrid.Add(Coroutine_.Action.LerpColor(this.imageGrid, this.imageGrid.color, COLOR_GRID_GRIDONLY, SECOND_FADE_IN, Easing.QuadInOut));
            coFloor.Add(Coroutine_.Action.LerpColor(this.image, this.image.color, COLOR_GRIDONLY, SECOND_FADE_IN, Easing.QuadInOut));

            this.button.enabled = false;
            this.callbackOnPress = null;
        }

        public void ChangeModeTarget(CallbackOnPress callback)
        {
            if (mode == Mode.Target)
            {
                return;
            }
            mode = Mode.Target;

            coGrid.StopCurrent();
            coFloor.StopCurrent();

            coGrid.Add(Coroutine_.Action.LerpColor(this.imageGrid, this.imageGrid.color, COLOR_GRID_TARGET, SECOND_FADE_IN, Easing.QuadInOut));
            coFloor.Add(Coroutine_.Action.LerpColor(this.image, this.image.color, COLOR_TARGET, SECOND_FADE_IN, Easing.QuadInOut));

            this.button.enabled = true;
            this.callbackOnPress = callback;
        }

        public void ChangeModeTargetNoClick()
        {
            if (mode == Mode.Target)
            {
                return;
            }
            mode = Mode.Target;

            coGrid.StopCurrent();
            coFloor.StopCurrent();

            coGrid.Add(Coroutine_.Action.LerpColor(this.imageGrid, this.imageGrid.color, COLOR_GRID_TARGET, SECOND_FADE_IN, Easing.QuadInOut));
            coFloor.Add(Coroutine_.Action.LerpColor(this.image, this.image.color, COLOR_TARGET, SECOND_FADE_IN, Easing.QuadInOut));

            this.button.enabled = false;
        }

        public void ChangeModeAttackDirection(CallbackOnPress callback, bool isCurrentUnitGrid = false)
        {
            if (mode == Mode.AttackDirection)
            {
                return;
            }
            mode = Mode.AttackDirection;

            coGrid.StopCurrent();
            coFloor.StopCurrent();

            if (isCurrentUnitGrid)
            {
                coGrid.Add(Coroutine_.Action.LerpColor(this.imageGrid, this.imageGrid.color, COLOR_GRID_ATTACK_DIRECTION_CURRENT, SECOND_FADE_IN, Easing.QuadInOut));
                coFloor.Add(Coroutine_.Action.LerpColor(this.image, this.image.color, COLOR_ATTACK_DIRECTION_CURRENT, SECOND_FADE_IN, Easing.QuadInOut));
            }
            else
            {
                coGrid.Add(Coroutine_.Action.LerpColor(this.imageGrid, this.imageGrid.color, COLOR_GRID_ATTACK_DIRECTION, SECOND_FADE_IN, Easing.QuadInOut));
                coFloor.Add(Coroutine_.Action.LerpColor(this.image, this.image.color, COLOR_ATTACK_DIRECTION, SECOND_FADE_IN, Easing.QuadInOut));
            }

            this.button.enabled = true;
            this.callbackOnPress = callback;
        }

        public void ChangeModeCurrentUnit()
        {
            if (mode == Mode.CurrentUnit)
            {
                return;
            }
            mode = Mode.CurrentUnit;

            coGrid.StopCurrent();
            coFloor.StopCurrent();

            coGrid.Add(Coroutine_.Action.LerpColor(this.imageGrid, this.imageGrid.color, COLOR_GRID_CURRENT, SECOND_FADE_IN, Easing.QuadInOut));
            coFloor.Add(Coroutine_.Action.LerpColor(this.image, this.image.color, COLOR_CURRENT, SECOND_FADE_IN, Easing.QuadInOut));

            this.button.enabled = false;
            this.callbackOnPress = null;
        }

        public void ChangeModeMovable(CallbackOnPress callback)
        {
            if (mode == Mode.Movable)
            {
                return;
            }
            mode = Mode.Movable;

            coGrid.StopCurrent();
            coFloor.StopCurrent();

            coGrid.Add(Coroutine_.Action.LerpColor(this.imageGrid, this.imageGrid.color, COLOR_GRID_MOVABLE, SECOND_FADE_IN, Easing.QuadInOut));
            coFloor.Add(Coroutine_.Action.LerpColor(this.image, this.image.color, COLOR_MOVABLE, SECOND_FADE_IN, Easing.QuadInOut));

            this.button.enabled = true;
            this.callbackOnPress = callback;
        }

        public void OnClickButton()
        {
            if (callbackOnPress != null)
            {
                callbackOnPress();
            }
        }
    }
}
