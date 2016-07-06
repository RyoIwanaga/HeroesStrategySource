using System.Collections;

namespace TacticsBG
{
    public class Unit
    {
        public enum Type
        {
            Mage = 0, 
            Amazon,
            Skeleton,
            Zombie,
            Max,
        }

        const float DAMAGE_RANGE = 0.2f; // 100 damage -> 90 ~ 110 damage 
        public const int MELEE_RANGE = 0;
        public const int DEFAULT_RANGE = 6;

        public Type Type_ { set; get; }
        public int Owner { set; get; }
        public Basic.Vec2Int Pos { set; get; }

        public int HpMax { set; get; }
        public int HpCurrent { set; get; }
        public int Damage { set; get; }
        public int Defence { set; get; }
        public int Movility { set; get; }
        public int Initiative { set; get; }
        public int ShootRange { set; get; }
        public bool isWaited { set; get; }

        public float HpFraction
        {
            get
            {
                return (float)HpCurrent / (float)HpMax;
            }
        }

        public Unit (
            Type type,
            int owner,
            Basic.Vec2Int pos,
            int hpMax,
            int hpCurrent,
            int damage,
            int defence,
            int mobility,
            int initiative,
            int shootRange,
            bool isWaited)
        {
            this.Type_ = type;
            this.Owner = owner;
            this.Pos = pos;
            this.HpMax = hpMax;
            this.HpCurrent = hpCurrent;
            this.Damage = damage;
            this.Defence = defence;
            this.Movility = mobility;
            this.Initiative = initiative;
            this.ShootRange = shootRange;
            this.isWaited = isWaited;
        }

        public Unit(Unit rhs) : this(
            rhs.Type_,
            rhs.Owner,
            rhs.Pos,
            rhs.HpMax,
            rhs.HpCurrent,
            rhs.Damage,
            rhs.Defence,
            rhs.Movility,
            rhs.Initiative,
            rhs.ShootRange,
            rhs.isWaited)
        {
        }

        /// <summary>
        /// 細かくパラメータ調整
        /// </summary>
        public static Unit CreateForDebug(
            Type type,
            int owner,
            Basic.Vec2Int pos,
            int hpMax,
            int hpCurrent,
            int damage,
            int defence,
            int mobility,
            int initiative,
            int shootRange,
            bool isWaited)
        {
            return new Unit(
                type,
                owner,
                pos,
                hpMax,
                hpCurrent,
                damage,
                defence,
                mobility,
                initiative,
                shootRange,
                isWaited);
        }

        public static Unit Create(Type type, int owner, Basic.Vec2Int pos)
        {
            return new Unit(type, owner, pos, 0, 0, 0, 0, 3, 0, DEFAULT_RANGE, false);
        }

        public int DamageRangedHalved { get { return this.Damage / 2; } }

        public bool IsMelee { get { return ! this.IsRanged; } }
        public bool IsRanged { get { return this.ShootRange >= 2; } }

        public bool IsDead { get { return this.HpCurrent <= 0; } }
        public bool IsAlive { get { return !this.IsDead; } }

        public bool IsAttackable(Unit unit)
        {
            return ! unit.IsDead && ! this.IsSameTeam(unit);
        }

        public bool IsSameTeam(Unit unit)
        {
            return this.Owner == unit.Owner;
        }

        /// <summary>
        /// Create average damage for avarage node
        /// </summary>
        /// <returns>average damage</returns>
        public int CreateDamageAverage()
        {
            return this.Damage;
        }

        /// <summary>
        /// Create true(random) damage for true node
        /// </summary>
        /// <returns>true damage</returns>
        public int CreateDamageRandom()
        {
            float range = this.Damage * DAMAGE_RANGE;
            //            float randResult = UnityEngine.Random.Range(0f, range);
            float randResult = (float)Basic.Random.Instance.NextDouble() * range;

            return this.Damage + (int)(randResult - range / 2f);
        }

        /// <summary>
        /// Attack enemy with random or ture damage.
        /// </summary>
        /// <param name="target">attack target</param>
        /// <param name="damage">random / true damage</param>
        /// <returns>is target dead?</returns>
        public bool Attack(Unit target, int damage)
        {
            target.HpCurrent = System.Math.Max(0, target.HpCurrent - damage);

            return target.IsDead;
        }
    }
}
