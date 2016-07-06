using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Scene.TacticsBattle
{
    public static class BoardPathToWalkPathConverter
    {
        public static List<MovePath> Convert(BoardGame.BoardPath path)
        {
            Debug.Assert(path.Count >= 2);

            var acc = new List<MovePath>(path.Count);

            if (path.Count == 2)
            {
                var from = new Vector3(path[0].x, 0f, path[0].y);
                var to = new Vector3(path[1].x, 0f, path[1].y);

                acc.Add(MovePath.CreateStraight(from, to));
            }
            else
            {
                var pre2 = new Vector3(path[0].x, 0f, path[0].y);
                var pre1 = new Vector3(path[1].x, 0f, path[1].y);

                acc.Add(MovePath.CreateStraight(pre2, pre1));

                for (int i = 2; i < path.Count; i++)
                {
                    var pos = new Vector3(path[i].x, 0f, path[i].y);

                    AddToAcc(acc, pos, pre1 - pre2);

                    pre2 = pre1;
                    pre1 = pos;
                }
            }

            return acc;
        }

        static void AddToAcc(List<MovePath> acc, Vector3 pos, Vector3 vecPre)
        {
            if (acc.Count >= 1)
            {
                var last = acc[acc.Count - 1];
                var vec = pos - last.PosEnd;
                var angle = Vector3.Angle(vec, vecPre);//Mathf.Abs(radFixed - radFixedLast);

                Debug.Assert(last.IsStraight);

                /* o-? = straight, x-x curve
                 * 
                 * +---+---+---+
                 * |   |   |   |
                 * | o---o---o-| 
                 * |   |   |   |
                 * +---+---x---+
                 * 
                 * +---+
                 * |   |
                 * | o |
                 * |  \|
                 * +---\---+
                 *     |\  |
                 *     | \ |
                 *     |  \|
                 *     +---\---+
                 *         |\  |
                 *         | o |
                 *         |   |
                 *         +---+
                 */
                if (angle < 10f) // almost equal
                {
                    acc[acc.Count - 1] = MovePath.CreateStraight(last.PosStart, pos);
                }
                /* o-? = straight, x-x curve
                 * 
                 * +---+---+
                 * |   |   |
                 * | o-|-x |
                 * |   |  \|
                 * +---+---x---+
                 *         |\  |
                 *         | o |
                 *         |   |
                 *         +---+
                 */
//                else if (radDiff < 55) // 45 degree
//                {
//                    acc.Add(MovePath.CreateCurve(last.PosEnd, last.PosEnd + vec / 2, last.VecUnitEnd, vec));
//                    acc.Add(MovePath.CreateStraight(last.PosEnd + vec / 2, pos));
//                }
                /* o-? = straight, x-x curve
                 * 
                 * +---+   +---+
                 * |   |   |   |
                 * | o |   | o |
                 * |  \|   |/  |
                 * +---x---x---+
                 *     |\ /|
                 *     | - |
                 *     |   |
                 *     +---+
                 */
                else if (angle < 100) // 90 degree
                {
                    var curveStart = last.PosEnd - vecPre / 2;
                    var curveEnd = last.PosEnd + vec / 2;
                    bool isStraight = true;

                    if (acc.Count >= 2)
                        if (acc[acc.Count - 2].IsStraight)
                            isStraight = true;
                        else
                            isStraight = false;
                    else
                        isStraight = true;


                    if (isStraight)
                    {
                        // o -> x
                        acc[acc.Count - 1] = MovePath.CreateStraight(last.PosStart, curveStart);

                        // x -> x
                        acc.Add(MovePath.CreateCurve(curveStart, curveEnd, last.VecUnitEnd, vec));
                    }
                    else
                    {
                        // rewite only 0.5 length
                        if (last.IsHarf)
                        {
                            acc[acc.Count - 1] = MovePath.CreateCurve(curveStart, curveEnd, last.VecUnitEnd, vec);
                        }
                        else
                        {
                            var lastCopy = last;
                            lastCopy.PosEnd -= vecPre / 2f;
                            acc[acc.Count - 1] = lastCopy;

                            acc.Add(MovePath.CreateCurve(curveStart, curveEnd, last.VecUnitEnd, vec));
                        }
                    }

                    // x -> o
                    acc.Add(MovePath.CreateStraight(curveEnd, pos));
                }
                else
                {
                    Debug.Assert(false);
                }
            }
        }
    }
}
