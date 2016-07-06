using System.Collections.Generic;

namespace TacticsBG
{
    public class TBGActionMove : TBGActionBase
    {
        BoardGame.BoardPath _path;
        public BoardGame.BoardPath Path { get { return _path; } }

        public TBGActionMove(int playerOwner, int unitIndex,
            BoardGame.BoardPath path)
            : base(playerOwner, unitIndex)
        {
            _path = path;
        }
    }
}
