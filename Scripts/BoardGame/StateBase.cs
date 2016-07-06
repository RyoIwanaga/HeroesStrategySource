namespace BoardGame
{
    /// <summary>
    /// ボードゲームの盤面の状態を管理
    /// TODO globalはどうするか
    /// </summary>
    public class StateBase
    {
        int _playerCurrent;
        public virtual int PlayerCurrent { get { return _playerCurrent; } }

        int _playerMax;
        public int PlayerMax { get { return _playerMax; } }

        protected StateBase (int playerCurrent, int playerMax)
        {
            _playerCurrent = playerCurrent;
            _playerMax = playerMax;
        }
    }
}
