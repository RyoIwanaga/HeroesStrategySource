using System.Collections.Generic;

namespace BoardGame
{
    public abstract class BoardGameBase<StateType> where StateType : StateBase
    {
        public StateNode StateNodeRoot { get; set; }
        public StateNode StateNodeCurrent { get; set; }
        public StateType StateCurrent { get { return this.StateNodeCurrent.State as StateType; } }

        protected BoardGameBase(StateType stateFirst)
        {
            this.StateNodeRoot = StateNodeCurrent = new StateNode(stateFirst);
        }

        // 盤面から指すことのできる手をすべて返すルールを記述する
        // Depth が 0 より大きい場合は、平均ノードを生成する必要がないので、
        // Range の平均を ActionTrue として使う
        protected abstract List<ActionContainer> CreateActions(StateType state, bool isCreatingAI);

        /// <summary>
        /// 現在の State Node がアクションリストを作っていないならば、
        /// 保存して返す。実行済みであれば返す。
        /// </summary>
        public List<ActionContainer> ForceActionList(bool isAI)
        {
            if (StateNodeCurrent.ActionContainers == null)
            {
                StateNodeCurrent.ActionContainers = CreateActions(StateNodeCurrent.State as StateType, isAI);
            }

            return StateNodeCurrent.ActionContainers;
        }

        public void SelectActionNode(int index)
        {
            var selectedActionNode = StateNodeCurrent.SelectActionNode(index);

            StateNodeCurrent = selectedActionNode.StateNodeNext;
        }

        /// <summary>
        /// Calculate score value of board state
        /// </summary>
        protected abstract float ScoreState(StateType s);

        protected float ScoreActionContainer(int player, ActionContainer container, int depth)
        {
            ActionNode average = container.ActionAverage;

            if (average == null)
                UnityEngine.Debug.Assert(false, "Please Implement weight node version");

            var nextStateNode = average.StateNodeNext;

            if (depth == 0)
            {
                return ScoreState(nextStateNode.State as StateType);
            }

            var actions = this.CreateActions(nextStateNode.State as StateType, true); 

            if (actions == null || actions.Count == 0)
            {
                return ScoreState(nextStateNode.State as StateType);
            }
            else
            {
                var scores = this.CreateScores(player, nextStateNode, depth - 1);

                if (scores.Length == 1)
                    return scores[0]; // Exit

                if (nextStateNode.State.PlayerCurrent == player)
                {
                    float max = scores[0];

                    for (int i = 1; i < scores.Length; i++)
                    {
                        if (max < scores[i])
                            max = scores[i];
                    }

                    return max;
                }
                else
                {
                    float min = scores[0];

                    for (int i = 1; i < scores.Length; i++)
                    {
                        if (min > scores[i])
                            min = scores[i];
                    }

                    return min;
                }
            }
        }

        public float[] CreateScores(int player, StateNode stateNode, int depth)
        {
//            DebugUtil.FuncName1(string.Format("p:{0}, d:{1}", player, depth));

            var actions = CreateActions(stateNode.State as StateType, true); 
            var scores = new float[actions.Count];

            for (int i = 0; i < actions.Count; i++)
            {
                scores[i] = ScoreActionContainer(player, actions[i], depth);
            }

            return scores;
        }
    }
}
