using System.Collections;

using BoardGame;

namespace TacticsBG
{
    public static class ActionContainerConstructor
    {
        /// <summary>
        /// Create ActionContainer that contain true and average node.
        /// </summary>
        public static ActionContainer CreateAttackRanged(TBGState s, int targetUnitIndex)
        {
            var unitCurrent = s.UnitCurrent;

            return ActionContainer.CreateAverage(
                CreateActionNodeAttackRanged(s, targetUnitIndex, unitCurrent.CreateDamageRandom()),
                CreateActionNodeAttackRanged(s, targetUnitIndex, unitCurrent.CreateDamageAverage()));
        }

        static ActionNode CreateActionNodeAttackRanged(TBGState s, int targetUnitIndex, int damage)
        {
            var cpState = new TBGState(s);
            var cpUnitCurrent = cpState.UnitCurrent;
            var cpUnitTarget = cpState.Units[targetUnitIndex];
            bool isDead = cpUnitCurrent.Attack(cpUnitTarget, damage);

            // update wait list
            cpState.UpdateWaitListNext();

            return new ActionNode(new TBGActionAttackRanged(
                s.PlayerCurrent,
                s.UnitCurrentIndex,
                s.UnitCurrent.Pos,
                damage,
                targetUnitIndex,
                s.Units[targetUnitIndex].Pos,
                true,
                isDead),
                //
                new StateNode(cpState));
        }

        /// <summary>
        /// Create ActionContainer that contain true and average node.
        /// </summary>
        public static ActionContainer CreateAttackMelee(TBGState s, int targetUnitIndex, BoardPath path)
        {
            var unitCurrent = s.UnitCurrent;

            return ActionContainer.CreateAverage(
                CreateActionNodeAttackMelee(s, targetUnitIndex, unitCurrent.CreateDamageRandom(), path),
                CreateActionNodeAttackMelee(s, targetUnitIndex, unitCurrent.CreateDamageAverage(), path));
        }

        static ActionNode CreateActionNodeAttackMelee(TBGState s, int targetUnitIndex, int damage, BoardPath path)
        {
            var cpState = new TBGState(s);
            var cpUnitCurrent = cpState.UnitCurrent;
            var cpUnitTarget = cpState.Units[targetUnitIndex];
            var cpPath = new BoardPath(path);
            bool isDead = cpUnitCurrent.Attack(cpUnitTarget, damage);

            // move unit
            cpUnitCurrent.Pos = cpPath.Last;

            // update wait list
            cpState.UpdateWaitListNext();

            return new ActionNode(new TBGActionAttackMelee(
                s.PlayerCurrent,
                s.UnitCurrentIndex,
                cpPath,
                damage,
                targetUnitIndex,
                s.Units[targetUnitIndex].Pos,
                isDead),
                //
                new StateNode(cpState));
        }

        /// <summary>
        /// Create ActionContainer that contain only true node.
        /// </summary>
        public static ActionContainer CreateMove(TBGState s, BoardPath path)
        {
            var cpState = new TBGState(s);
            var cpUnitCurrent = cpState.UnitCurrent;
            var cpPath = new BoardPath(path);

            // update unit pos
            cpUnitCurrent.Pos = cpPath.Last;

            // update wait list
            cpState.UpdateWaitListNext();

            var moveNode = new ActionNode(new TBGActionMove(
                    s.UnitCurrent.Owner,
                    s.UnitCurrentIndex,
                    cpPath),
                    //
                    new StateNode(cpState));

            return ActionContainer.CreateTrue(moveNode);
        }
    }
}
