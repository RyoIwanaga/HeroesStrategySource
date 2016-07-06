using System.Collections;

using BoardGame;

namespace TacticsBG
{
    public class TBGActionAttackRanged : TBGActionBase
    {
        Basic.Vec2Int _posFrom;
        public Basic.Vec2Int PosFrom { get { return _posFrom; } }

        int _damage;
        public int Damage { get { return _damage; } }

        int _targetUnitIndex;
        public int TargetUnitIndex { get { return _targetUnitIndex; } }

        Basic.Vec2Int _targetUnitPos;
        public Basic.Vec2Int TargetUnitPos { get { return _targetUnitPos; } }

        bool _isFullDamage;
        public bool IsFullDamage { get { return _isFullDamage; } }

        bool _isTargetDead;
        public bool IsTargetDead { get { return _isTargetDead; } }

        public TBGActionAttackRanged(int playerOwner, int unitIndex,
            Basic.Vec2Int posFrom,
            int damage,
            int targetUnitIndex,
            Basic.Vec2Int targetUnitPos,
            bool isFullDamage,
            bool isTargetDead)
            : base(playerOwner, unitIndex)
        {
            _posFrom = posFrom;
            _damage = damage;
            _targetUnitIndex = targetUnitIndex;
            _targetUnitPos = targetUnitPos;
            _isFullDamage = isFullDamage;
            _isTargetDead = isTargetDead;
        }
    }
}
