using System.Collections;
using System.Collections.Generic;

using BoardGame;

namespace TacticsBG
{
    /// <summary>
    /// State for Tactics board game
    /// </summary>
    public class TBGState : BoardGame.StateBase
    {
        Basic.Vec2Int _size;
        public Basic.Vec2Int Size { get { return _size; } }

        Basic.Vec2Int[] _walls;
        public Basic.Vec2Int[] Walls { get { return _walls; } }


        Unit[] _units;
        public Unit[] Units
        {
            get { return _units; }
        }
        
        // TODO change to list ?
        List<int> _waitUnitIndexes; 
        public List<int> WaitUnitIndexes
        {
            get { return _waitUnitIndexes; }
        }

        public int UnitCurrentIndex { get { return _waitUnitIndexes[0]; } }
        public Unit UnitCurrent { get { return _units[this.UnitCurrentIndex]; } }
        public override int PlayerCurrent { get { return this.UnitCurrent.Owner; } }

        public TBGState (int playerMax, Basic.Vec2Int size, Basic.Vec2Int[] walls, Unit[] units, List<int> wUIndexes) 
            : base (-1, playerMax) // dont use player current
        {
            _size = size;
            _walls = walls;
            _units = units;
            _waitUnitIndexes = wUIndexes;
        }

        public TBGState(TBGState rhs) : this(
            rhs.PlayerMax,
            rhs.Size,
            rhs.Walls,
            new Unit[rhs.Units.Length],
            new List<int>(rhs.WaitUnitIndexes.Count))
        {
            for (int i = 0; i < rhs.Units.Length; i++)
            {
                _units[i] = new Unit(rhs.Units[i]);
            }

            for (int i = 0; i < rhs.WaitUnitIndexes.Count; i++)
            {
                _waitUnitIndexes.Add(rhs.WaitUnitIndexes[i]);
            }
        }

        /// <summary>
        /// ユニットの行動が完了した時に、
        /// wait list の先頭からユニットを削除したリストを生成する
        /// 死亡したユニットはリストから削除
        /// </summary>
        public void UpdateWaitListNext()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(_waitUnitIndexes.Count >= 1);
#else
            System.Diagnostics.Debug.Assert(_waitUnitIndexes.Count >= 1);
# endif

            /*
            if (_waitUnitIndexes.Count == 1)
            {
                _waitUnitIndexes = TacticsBG.CreateUnitWaitList(_units);
            }
            else
            {*/
                var newList = new List<int>(_waitUnitIndexes.Count - 1);

                for (int i = 1; i < _waitUnitIndexes.Count; i++)
                {
                    var index = _waitUnitIndexes[i];
                    if (_units[index].IsAlive)
                        newList.Add(index);    
                }

                _waitUnitIndexes = newList;

                if (newList.Count == 0)
                {
                    _waitUnitIndexes = TacticsBG.CreateUnitWaitList(_units);
                }
            //}
        }
    }
}
