namespace BoardGame
{
    /* AI が使用する、確率ノード用
     */
    public class ActionAndWeight
    {
        float _weight;
        ActionNode _action;

        public float Weight { get { return _weight; } }
        public ActionNode Action { get { return _action; } }

        public ActionAndWeight(float weight, ActionNode action)
        {
            _weight = weight;
            _action = action;
        }
    }
}