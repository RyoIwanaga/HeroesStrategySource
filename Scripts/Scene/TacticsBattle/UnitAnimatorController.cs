using UnityEngine;
using System.Collections;

namespace Scene.TacticsBattle
{
    public class UnitAnimatorController : MonoBehaviour
    {
        public static string PARAM_FLOAT_SPEED = "Speed";
        public static string PARAM_FLOAT_TURN = "Turn";
        public static string PARAM_DAMAGED_TRIGER = "Damaged";
        public static string PARAM_DAMAGED_DEATH = "Death";
        public static string PARAM_DAMAGED_ATTACK_MELEE = "AttackMelee";
        public static string PARAM_DAMAGED_ATTACK_RANGED = "AttackRanged";

        Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
            // TODO null check debug
        }

        public void SetSpeed(float f)
        {
            animator.SetFloat(PARAM_FLOAT_SPEED, f);
        }

        public void SetTurn(float f)
        {
            animator.SetFloat(PARAM_FLOAT_TURN, f);
        }

        public void Triger(string s)
        {
            animator.SetTrigger(s);
        }
    }
}
