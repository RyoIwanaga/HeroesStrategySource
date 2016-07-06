using System.Collections;

using BoardGame;


namespace TacticsBG
{
    public class TBGActionAttackMelee : TBGActionBase
    {
        BoardPath _path;
        public BoardPath Path { get { return _path; } }

        int _damage;
        public int Damage { get { return _damage; } }

        int _targetUnitIndex;
        public int TargetUnitIndex { get { return _targetUnitIndex; } }

        Basic.Vec2Int _targetUnitPos;
        public Basic.Vec2Int TargetUnitPos { get { return _targetUnitPos; } }

        bool _isTargetDead;
        public bool IsTargetDead { get { return _isTargetDead; } }

        public TBGActionAttackMelee (int playerOwner, int unitIndex,
            BoardGame.BoardPath path,
            int damage,
            int targetUnitIndex,
            Basic.Vec2Int targetUnitPos,
            bool isTargetDead)
            : base(playerOwner, unitIndex)
        {
            _path = path;
            _damage = damage;
            _targetUnitIndex = targetUnitIndex;
            _targetUnitPos = targetUnitPos;
            _isTargetDead = isTargetDead;
        }
    }
}
