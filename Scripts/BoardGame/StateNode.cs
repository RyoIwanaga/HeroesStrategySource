using System;
using System.Collections.Generic;

namespace BoardGame
{
    public class StateNode : LinkedNode
    {
        public ActionNode ActionNodePrevious
        {
            get
            {
                return this.PrevNode as ActionNode;
            }

            set
            {
                this.PrevNode = value;
            }
        }
        
        public ActionNode ActionNodeNext
        {
            get
            {
                return this.NextNode as ActionNode;
            }

            set
            {
                this.NextNode = value;
            }
        }
        
        StateBase _state = null;
        public StateBase State { get { return _state; } }

        /* この盤面（State)から指すことのできるすべての手を格納
         * null: 未評価
         * .count == 0: ゲーム終了
         * .count > 0: 指すことのできる手が存在
         */
        public List<ActionContainer> ActionContainers { get; set; }

        public StateNode(StateBase state) : base()
        {
            _state = state;
            this.ActionContainers = null;
        }

        bool IsGameEnd()
        {
            if (this.ActionContainers == null)
            {
                throw new InvalidOperationException("Please create actions, before calling this function.");
            }
            else
            {
                return this.ActionContainers.Count == 0;
            }
        }

        /// <summary>
        /// 正規の ActionNode を選択し、平均 / 確率 ノードを破棄し、
        /// 繋げる、不要になった ActionContainers は破棄する
        /// </summary>
        /// <returns> Selected ActionNode</returns>
        public ActionNode SelectActionNode(int index)
        {
            var targetActionNode = this.ActionContainers[index].ActionTrue;
            this.Connect(targetActionNode);

            for (int i = 0; i < this.ActionContainers.Count; i++) {

                if (i == index)
                {
                    this.ActionContainers[i].CutAllConnectionExceptTrueAction();
                }
                else
                {
                    this.ActionContainers[i].CutAllConnection();
                }
            } 

            return targetActionNode;
        }
    }
}
