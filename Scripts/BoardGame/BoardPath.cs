using System.Collections.Generic;

namespace BoardGame
{
    /// <summary>
    /// ユニットの移動の軌跡を管理する
    /// </summary>
    public class BoardPath
    {
        #region === Property ===

        List<Basic.Vec2Int> _list;

        float _lineCount = -1f;
        float CountLine
        {
            get
            {
                int size = this.Count; 
                if (size == 0)
                {
                    return 0f;
                }

                return _CountLine(1, Basic.Vec2Int.Zero, 0, size - 1);
            }
        }
        public float ForceCountLine
        {
            get
            {
                if (_lineCount == -1f)
                {
                    _lineCount = this.CountLine;
                }

                return _lineCount;
            }
        }

        List<Basic.Vec2Int> _vertex;
        List<Basic.Vec2Int> Vertexs
        {
            get
            {
                var points = _list;
                var acc = new List<Basic.Vec2Int>(points.Count);

                acc.Add(points[0]);

                if (points.Count == 0)
                    return acc; // Exit


                var posPrev = points[0];
                var vecPrev = points[1] - points[0];

                for (int i = 1; i < points.Count; i++)
                {
                    var posCur = points[i];
                    var vecCur = posCur - posPrev;

                    if (vecCur != vecPrev)
                    {
                        acc.Add(posPrev);
                    }

                    vecPrev = vecCur;
                    posPrev = posCur;
                }

                acc.Add(posPrev);

                return acc;
            }
        }
        public List<Basic.Vec2Int> ForceVertexs
        {
            get
            {
                if (_vertex == null)
                {
                    _vertex = this.Vertexs;
                }

                return _vertex;
            }
        }


        float _length = -1f;
        float Length
        {
            get
            {
                var count = _list.Count;

#if UNITY_EDITOR
                UnityEngine.Debug.Assert(count >= 1);
#else
                System.Diagnostics.Debug.Assert(count >= 1);
#endif

                if (count == 1)
                {
                    return 0f;
                }
                else if (count == 2)
                {
                    var vec = _list[1] - _list[0];

                    return IsSkew(vec) ? 1.414f : 1f;
                }
                else
                {
                    var posPrevious = _list[0];
                    float accLength = 0f;
                    
                    for (int i = 1; i < count; i++)
                    {
                        var posThis = _list[i];
                        var vecSub = posPrevious - posThis;

                        accLength += IsSkew(vecSub) ? 1.414f : 1f;

                        posPrevious = posThis;
                    }

                    return accLength;
                }
            }
        }
        public float ForceLength
        {
            get
            {
                if (_length == -1f)
                {
                    _length = this.Length;
                }

                return _length;
            }
        }
        
        public int Count { get { return _list.Count; } }

        public Basic.Vec2Int Last
        {
            get
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Assert(Count >= 1);
#else
                System.Diagnostics.Debug.Assert(Count >= 1);
#endif

                return this[this.Count - 1];
            }
        }

        public Basic.Vec2Int this[int i]
        {
            get { return _list[i]; }
        }

        public void Add(Basic.Vec2Int item)
        {
            ResetCache();
            _list.Add(item);
        }


#endregion

#region === Helper ===

        void ResetCache()
        {
            _lineCount = -1f;
            _length = -1f;
            _vertex = null;
        }

        int _CountLine(int accCount, Basic.Vec2Int vec, int index, int indexMax)
        {
            if (index == indexMax)
            {
                return accCount;
            }
            else
            {
                var currentToNextVector = this[indexMax] - this[index];

                // 同じ方向に移動している、もしくは一度目の移動なので、line を増やさない
                if (index == 0 && vec == currentToNextVector)
                {
                    return _CountLine(accCount, currentToNextVector, index + 1, indexMax);
                }
                // 曲がった
                else
                {
                    return _CountLine(accCount + 1, currentToNextVector, index + 1, indexMax);
                }
            }
        }

        static bool IsSkew(Basic.Vec2Int vec)
        {
            return System.Math.Abs(vec.x) > 0 && System.Math.Abs(vec.y) > 0;
        }

#endregion


        public BoardPath(int size)
        {
            _list = new List<Basic.Vec2Int>();
        }

        /// <summary>
        /// For stay attack 
        /// </summary>
        /// <param name="onePoint"></param>
        public BoardPath(Basic.Vec2Int onePoint) : this(1)
        {
            this._list.Add(onePoint);
        }

        /// <summary>
        /// Copy constractor
        /// </summary>
        public BoardPath(BoardPath rhs) : this(rhs.Count)
        {

            for (int i = 0; i < rhs.Count; i++)
            {
                this.Add(rhs[i]);
            }
        }

    }
}
