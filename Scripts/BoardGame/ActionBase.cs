namespace BoardGame
{
    public class ActionBase
    {
        int _playerOwner;
        public int PlayerOwner { get { return _playerOwner; } }

        protected ActionBase (int playerOwner)
        {
            _playerOwner = playerOwner;
        }
    }
}
