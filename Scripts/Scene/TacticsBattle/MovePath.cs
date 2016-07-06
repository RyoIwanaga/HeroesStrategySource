using UnityEngine;
using System.Collections;

namespace Scene.TacticsBattle
{
    public struct MovePath
    {
        public Vector3 PosStart;
        public Vector3 PosEnd;
        public Vector3 VecUnitStart;
        public Vector3 VecUnitEnd;
        public bool IsStraight;

        public bool IsCurve { get { return !this.IsStraight; } }

        /// <summary>
        /// *****    ****o
        /// *   *    *  .*
        /// * o.o or * o *
        /// *   *    *   *
        /// *****    *****
        /// 
        /// </summary>
        public bool IsHarf
        {
            get
            {
                var vec = (PosStart - PosEnd);

                return vec.x <= 0.55f && vec.y <= 0.55f;
            }
        }

                MovePath(Vector3 ps, Vector3 pe, Vector3 vs, Vector3 ve, bool isStraight)
        {
            this.PosStart = ps;
            this.PosEnd = pe;
            this.VecUnitStart = vs;
            this.VecUnitEnd = ve;
            this.IsStraight = isStraight;
        }

        public static MovePath CreateStraight(Vector3 ps, Vector3 pe)
        {
            var unit = (pe - ps).normalized;

            return new MovePath(ps, pe, unit, unit, true);
        }

        public static MovePath CreateCurve(Vector3 ps, Vector3 pe, Vector3 vs, Vector3 ve)
        {
            return new MovePath(ps, pe, vs.normalized, ve.normalized, false);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}",
                this.IsStraight ? "|" : "c",
                PosStart.ToString(),
                PosEnd.ToString(),
                VecUnitStart.ToString(),
                VecUnitEnd.ToString());

        }

    }
}
