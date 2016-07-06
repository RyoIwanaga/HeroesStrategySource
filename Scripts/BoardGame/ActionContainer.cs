using System.Collections.Generic;

namespace BoardGame
{
    public class ActionContainer
    {
        ActionNode _actionTrue;
        public ActionNode ActionTrue { get { return _actionTrue; } }

        ActionNode _actionAverage;
        public ActionNode ActionAverage
        {
            get
            {
                if (this.IsWeightNode())
                {
                    return null;
                }
                else if (_actionAverage != null)
                {
                    return _actionAverage;
                }
                else
                {
                    return _actionTrue;
                }
            }
        }

        List<ActionAndWeight> _listActionAndWeight;
        public List<ActionAndWeight> ListActionAndWeight
        {
            get
            {
                if (this.IsWeightNode())
                {
                    return _listActionAndWeight;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsWeightNode()
        {
            return _listActionAndWeight != null;
        }

        public ActionContainer(ActionNode true_, ActionNode average, List<ActionAndWeight> listActionAndWeight)
        {
            _actionTrue = true_;
            _actionAverage = average;
            _listActionAndWeight = listActionAndWeight;
        }

        public static ActionContainer CreateTrue(ActionNode true_)
        {
            return new ActionContainer(true_, null, null);
        }

        public static ActionContainer CreateAverage(ActionNode true_, ActionNode average)
        {
            return new ActionContainer(true_, average, null);
        }

        public void CutAllConnection()
        {
            if (_actionTrue != null)
            {
                _actionTrue.Cut(_actionTrue.NextNode);
                _actionTrue = null;
            }

            if (_actionAverage != null)
            {
                _actionAverage.Cut(_actionAverage.NextNode);
                _actionAverage = null;
            }

            if (_listActionAndWeight != null)
            {
                for (int i = 0; i < _listActionAndWeight.Count; i++)
                {
                    var action = _listActionAndWeight[i].Action;
                    action.Cut(action.NextNode);
                }

                _listActionAndWeight = null;
            }
        }

        public void CutAllConnectionExceptTrueAction()
        {
            if (_actionAverage != null)
            {
                _actionAverage.Cut(_actionAverage.NextNode);
                _actionAverage = null;
            }

            if (_listActionAndWeight != null)
            {
                for (int i = 0; i < _listActionAndWeight.Count; i++)
                {
                    var action = _listActionAndWeight[i].Action;
                    action.Cut(action.NextNode);
                }

                _listActionAndWeight = null;
            }
        }
    }
}
