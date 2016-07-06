using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Scene.TacticsBattle
{
    public class TacticsUnitController : MonoBehaviour
    {
        static Color HP_COLOR1 = new Color(230f / 255f, 0f, 0f, 150f / 255f);
        static Color HP_COLOR0 = new Color(0f, 230f / 255f, 0f, 150f / 255f);

        public HpSliderController HpSlider = null;
        public TargetImageController TargetButton = null;

        GameObject model = null;
        public UnitAnimatorController Animator = null;

        float speed = 2.5f;
        float turnRate = 1f;

        TacticsBG.Unit.Type unitType;

        Coroutine_.Sequencer _coSequencer = null;
        public Coroutine_.Sequencer CoSequencer { get { return _coSequencer; } }

        public void Init(GameObject model, int player, TacticsBG.Unit.Type type)
        {
            this.model = model;
            Animator = this.model.GetComponent<UnitAnimatorController>();

            unitType = type;

            _coSequencer = new Coroutine_.Sequencer(this);

            if (player == 0)
                HpSlider.FrontImage.color = HP_COLOR0;
            else 
                HpSlider.FrontImage.color = HP_COLOR1;
        }

        public void SetHpbar(float hpFraction)
        {
            HpSlider.SetValue(hpFraction);
        }

        public IEnumerator CoMove(Vector3 start, Vector3 end)
        {
            float timePrev = 0f;
            float accTime = 0f;

            Vector3 startPos = transform.position;
            float distance = (end - start).magnitude;   
            float needTime = distance / this.speed;

            while (accTime < needTime)
            {
                this.transform.position = Vector3.Lerp(start, end, accTime / needTime);
                yield return null;

                if (timePrev != 0f)
                {
                    accTime += Time.time - timePrev;
                    Animator.SetSpeed(1f);
                }

                timePrev = Time.time;
            }

            Animator.SetSpeed(0f);
        }

        public IEnumerator CoMove2(MovePath path)
        {
            float timePrev = 0f;
            float timeAcc = 0f;
            float distance = (path.PosEnd - path.PosStart).magnitude;
            float timeNeed = distance / this.speed;

            var vecStart = path.VecUnitStart * this.speed;
            var vecEnd = path.VecUnitEnd * this.speed;

            Quaternion directionStart = Quaternion.LookRotation(path.VecUnitStart);
            Quaternion directionEnd = Quaternion.LookRotation(path.VecUnitEnd);

            while (timeAcc < timeNeed)
            {
                float time = timeAcc / timeNeed;

                // -- move --

                Animator.SetSpeed(1f);

                if (path.IsStraight)
                {
                    this.transform.position = Vector3.Lerp(path.PosStart, path.PosEnd, time);
                }
                else
                {
                    this.transform.position = UnityBehaviour.SplineCurve.UnuniformSprine.Interpolate(time, path.PosStart, path.PosEnd, vecStart, vecEnd);
                }

                // -- turn --

                model.transform.localRotation = Quaternion.Lerp(directionStart, directionEnd, time);
                // --

                yield return null;

                if (timePrev > 0f)
                {
                    timeAcc += Time.time - timePrev;
                }

                timePrev = Time.time;
            }

            Animator.SetSpeed(0f);
            this.transform.position = path.PosEnd;
            model.transform.localRotation = directionEnd;
        }

        public IEnumerator CoTurn(Vector3 direction)
        {
            float timePrev = 0f;
            float accTime = 0f;

            Quaternion start = model.transform.rotation;
            Quaternion end = Quaternion.LookRotation(direction);
            float distance = (direction - this.transform.eulerAngles).magnitude;   
            float needTime = distance / this.speed;
            // fixme
            needTime = 0.2f;

            while (accTime < needTime)
            {
                float time = accTime / needTime;
                model.transform.localRotation = Quaternion.Lerp(start, end, Basic.Easing.SineInOut(time));
                yield return null;

                if (timePrev != 0f)
                {
                    accTime += Time.time - timePrev;
                }

                timePrev = Time.time;
            }
        }

        public IEnumerator CoMelee(int damage, TacticsUnitController targetUnit, bool isTargetDead,
            TacticsBattleSceneManager.CreateDamageTextDelegate CreateDamageFunction)
        {
            Animator.Triger(UnitAnimatorController.PARAM_DAMAGED_ATTACK_MELEE);

            yield return new WaitForSeconds(0.9f);

            if (unitType == TacticsBG.Unit.Type.Mage)
            {
                Act.Util.EffectPlayOneShot(ResourceManager.Instance.Load(Define.Path.Effects.MeleeFire), targetUnit.transform.position);
            }

            CreateDamageFunction(damage, targetUnit.transform.position);


            if (isTargetDead)
            {
                targetUnit.Animator.Triger(UnitAnimatorController.PARAM_DAMAGED_DEATH);
            }
            else
            {
                targetUnit.Animator.Triger(UnitAnimatorController.PARAM_DAMAGED_TRIGER);
            }
        }

        public IEnumerator CoFire(int damage, Vector3 start, Vector3 end, TacticsUnitController targetUnit, bool isTargetDead, 
            TacticsBattleSceneManager.CreateDamageTextDelegate CreateDamageFunction)
        {
            const float speed = 0.5f;
            float timePrev = 0f;
            float accTime = 0f;

            Vector3 startPos = transform.position;
            float distance = (end - start).magnitude;   
            float needTime = distance / speed;

            Animator.Triger(UnitAnimatorController.PARAM_DAMAGED_ATTACK_RANGED);
            yield return new WaitForSeconds(0.7f);


            var mage = this.transform.FindChild("Mage(Clone)").GetComponent<MageModel>();

            var ball = (GameObject)GameObject.Instantiate(ResourceManager.Instance.Load(Define.Path.Effects.Fireball));
            yield return Coroutine_.Action.LerpMove(ball,
                mage.RightFinger.transform.position,
                //start + new Vector3(0f, 0.5f, 0f), 
                end + new Vector3(0f, 0.5f, 0f), 
                needTime);

            CreateDamageFunction(damage, end);

            GameObject.Destroy(ball);


            Act.Util.EffectPlayOneShot(ResourceManager.Instance.Load(Define.Path.Effects.Explosion), end);

            if (isTargetDead)
            {
                targetUnit.Animator.Triger(UnitAnimatorController.PARAM_DAMAGED_DEATH);
            }
            else
            {
                targetUnit.Animator.Triger(UnitAnimatorController.PARAM_DAMAGED_TRIGER);
            }

            /*

            while (accTime < needTime)
            {
                this.transform.position = Vector3.Lerp(start, end, accTime / needTime);
                yield return null;

                if (timePrev != 0f)
                {
                    accTime += Time.time - timePrev;
                    animator.SetSpeed(1f);
                }

                timePrev = Time.time;
            }
                    animator.SetSpeed(0f);*/
        }
    }
}
