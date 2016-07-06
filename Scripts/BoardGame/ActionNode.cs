namespace BoardGame
{
    public class ActionNode : LinkedNode
    {
        public StateNode StateNodeNext
        {
            get { return this.NextNode as StateNode; }
            set { this.NextNode = value; }
        }

        public StateNode StateNodePrev
        {
            get { return this.PrevNode as StateNode; }
            set { this.NextNode = value; }
        }

        ActionBase _action;
        public ActionBase Action { get { return _action; } }

        /// <summary>
        /// 原因（Action)には結果が伴う
        /// </summary>
        public ActionNode (ActionBase action, StateNode stateNodeNext) : base()
        {
            _action = action;
            this.StateNodeNext = stateNodeNext;

            this.Connect(stateNodeNext);
        }

        public void SetPreviousActionNodeForNextStateNode()
        {
            this.StateNodeNext.ActionNodePrevious = this;
        }
    }
}
