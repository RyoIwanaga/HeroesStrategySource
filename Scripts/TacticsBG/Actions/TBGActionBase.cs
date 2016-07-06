using System.Collections;

namespace TacticsBG
{
    public class TBGActionBase : BoardGame.ActionBase
    {
        int _unitIndex;
        public int UnitIndex { get { return _unitIndex; } }

        protected TBGActionBase (int playerOwner, int unitIndex) : base(playerOwner)
        {
            _unitIndex = unitIndex;
        }
    }
}

