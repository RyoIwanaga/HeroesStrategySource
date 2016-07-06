using System.Collections;

using BoardGame;

namespace TacticsBG
{
    public static class UnitConstructor
    {
        static Unit BaseStatus(Unit.Type uType, int hp, int damage, int def, int mobility, int initiative, int shootRange)
        {
            return new Unit(
                uType,
                0, // Set this later
                Basic.Vec2Int.Zero, // Set this later
                hp, hp,
                damage,
                def,
                mobility,
                initiative,
                shootRange,
                false);
        }

        static Unit StatusTable(Unit.Type type)
        {
            switch(type)
            {
                case Unit.Type.Amazon:
                    return BaseStatus(type, 200, 70, 20, 3, 5, Unit.MELEE_RANGE);
                case Unit.Type.Skeleton:
                    return BaseStatus(type, 80, 40, 20, 4, 3, Unit.MELEE_RANGE);
                case Unit.Type.Zombie:
                    return BaseStatus(type, 120, 80, 20, 2, 2, Unit.MELEE_RANGE);
                case Unit.Type.Mage:
                    return BaseStatus(type, 160, 50, 20, 2, 3, Unit.DEFAULT_RANGE);
                default:
#if UNITY_EDITOR
                    UnityEngine.Debug.Assert(false);
#else
                    System.Diagnostics.Debug.Assert(false);
#endif
                    return null;
            }
        }

        public static Unit Construct(Unit.Type type, int owner, Basic.Vec2Int pos)
        {
            var unit = StatusTable(type);
            unit.Owner = owner;
            unit.Pos = pos;

            return unit;
        }
    }
}

