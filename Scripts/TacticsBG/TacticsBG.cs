using System.Collections;
using System;
using System.Collections.Generic;

using BoardGame;

namespace TacticsBG
{
    /// <summary>
    /// TacticsBG = TacticsBattleGame
    /// </summary>
    public class TacticsBG : BoardGame.BoardGameBase<TBGState>
    {
        static int INITIAL_ACTION_NODE_SIZE = 16;

        public TacticsBG(TBGState stateFirst) : base(stateFirst)
        {
        }

        /// <summary>
        /// 自軍ユニットのヘルスが大きく、敵ユニットのヘルスが小さいボードの点数が高い
        /// </summary>
        protected override float ScoreState(TBGState s)
        {
            int player = s.PlayerCurrent;
            float value = 0f;

            for (int i = 0; i < s.Units.Length; i++)
            {
                var unit = s.Units[i];

                // team unit
                if (unit.Owner == player)
                {
                    value += unit.HpCurrent;
                }
                // enemy unit
                else
                {
                    value -= unit.HpCurrent;
                }
            }

            return value;
        }

        /// <summary>
        /// Tactics board game の現在の盤面から、手を生成するルールを記述する
        /// </summary>
        protected override List<ActionContainer> CreateActions(TBGState s, bool isCreatingAI)
        {
            var result = new List<ActionContainer>(INITIAL_ACTION_NODE_SIZE);

            // AI は攻撃アクションが生成されている場合は移動アクションの生成はしない
            var isAttackActionCreated = false;

            // ゲーム終了条件を満たす場合は、ゲーム終了のため空の手を生成する
            var winners = Winners(s);
            if (winners != null)
                return result;

            var currentUnit = s.UnitCurrent;
            var currentUnitIndex = s.UnitCurrentIndex;
            var neibors = BoardGame.BoardGameCommon.Neibors(currentUnit.Pos, s.Size);

            // ranged unit は隣接されると射撃ができなくなる
            bool isSurrounded = false;

            var pathes = TacticsBG.CreatePathes(s.UnitCurrent, s.UnitCurrent.Movility, s);

            // ---- Create melee attack----

            // -- Attack neibor --

            // for neibors
            for (int indexNeibor = 0; indexNeibor < neibors.Count; indexNeibor++)
            {
                // for units
                for (int indexUnit = 0; indexUnit < s.Units.Length; indexUnit++)
                {
                    // 隣接していて、アタック可能な場合
                    var target = s.Units[indexUnit];
                    if (currentUnit.IsAttackable(target) 
                        && neibors[indexNeibor] == target.Pos)
                    {
                        isSurrounded = true;

                        result.Add(ActionContainerConstructor.CreateAttackMelee(s, indexUnit, new BoardPath(currentUnit.Pos)));

                        // for AI
                        isAttackActionCreated = true;
                    }
                }
            }

            // -- Move attack --

            if (currentUnit.IsMelee
                || (currentUnit.IsRanged && isSurrounded))
            {
                // for path
                for (int indexPath = 0; indexPath < pathes.Count; indexPath++)
                {
                    var path = pathes[indexPath];
                    var goal = path.Last;
                    var goalsNeibor = BoardGameCommon.Neibors(goal, s.Size);

                    // for neibors from goal of path
                    for (int indexGNeibor = 0; indexGNeibor < goalsNeibor.Count; indexGNeibor++)
                    {
                        // for units
                        for (int indexUnit = 0; indexUnit < s.Units.Length; indexUnit++)
                        {
                            // 隣接していて、アタック可能な場合
                            var target = s.Units[indexUnit];
                            if (currentUnit.IsAttackable(target)
                                && goalsNeibor[indexGNeibor] == target.Pos)
                            {
                                result.Add(ActionContainerConstructor.CreateAttackMelee(s, indexUnit, path));

                                // for AI
                                isAttackActionCreated = true;
                            }
                        }
                    }
                }
            }
            else
            {
                // ---- Create Ranged Attack ----

                for (int i = 0; i < s.Units.Length; i++)
                {
                    var targetUnit = s.Units[i];

                    // Is possible to attack ?
                    if (s.UnitCurrent.IsAttackable(targetUnit))
                    {
                        result.Add(ActionContainerConstructor.CreateAttackRanged(s, i));

                        // for AI
                        isAttackActionCreated = true;
                    }
                }
            }

            // AI は攻撃済みの場合は、移動アクションは生成しない
            if (isAttackActionCreated && isCreatingAI)
            {
                return result;
            }

            // ---- Create Move ----

            for (int i = 0; i < pathes.Count; i++)
            {
                result.Add(ActionContainerConstructor.CreateMove(s, pathes[i]));
            }

            return result;
        }


        /// <summary>
        /// 通過や止まることの出来ない座標のリストを生成
        /// 生きているユニットと障害物が該当
        /// </summary>
        static List<Basic.Vec2Int> CreateWallPositions(Unit unit, TBGState state)
        {
            var result = new List<Basic.Vec2Int>((int)(state.Size.x * state.Size.y * 0.7f));
            var team = unit.Owner;

            // add enemy unit's pos
            for (int i = 0; i < state.Units.Length; i++)
            {
                var curUnit = state.Units[i];

                if (curUnit.IsAlive)
                {
                    result.Add(curUnit.Pos);
                }
            }

            // add wall's pos
            for (int i = 0; i < state.Walls.Length; i++)
            {
                result.Add(state.Walls[i]);
            }

            return result;
        }

        /// <summary>
        /// 止まることの出来ない座標のリストを生成
        /// 現在は特に指定なし
        /// </summary>
        static List<Basic.Vec2Int> CreateOnlyPassablePositions(Unit unit, TBGState state)
        {
            var result = new List<Basic.Vec2Int>((int)(state.Size.x * state.Size.y * 0.7f));
            var team = unit.Owner;

            /*

            for (int i = 0; i < state.Units.Length; i++)
            {
                var curUnit = state.Units[i];

                if (curUnit.IsSameTeam(unit))
                {
                    result.Add(curUnit.Pos);
                }
            }

            */

            return result;
        }

        /// <summary>
        /// 現在のユニットが移動可能なPathのリストを生成する
        /// </summary>
        static List<BoardPath> CreatePathes(Unit unit, int range, TBGState state)
        {
            var acc = new List<BoardPath>();
            var path = new BoardPath(range * range);
            path.Add(unit.Pos);

            _CreatePathes(acc, path, range, state,
                CreateWallPositions(unit, state),
                CreateOnlyPassablePositions(unit, state));

            return acc;
        }

        static void _CreatePathes(List<BoardPath> acc, BoardPath path, int range, TBGState state, List<Basic.Vec2Int> walls, List<Basic.Vec2Int> cantStops)
        {
//            DebugOnly.FuncName1(string.Format("range: {0}", range));

            if (range == 0)
            {
//                DebugOnly.FuncName1("Exit");
                return;
            }

            var last = path[path.Count - 1];

            var neibors = BoardGameCommon.Neibors(last, state.Size);
            for (int i = 0; i < neibors.Count; i++)
            {
                var thisPos = neibors[i];

                // -- Wall ? --

                bool IsWall = false;
                for (int wi = 0; wi < walls.Count; wi++)
                {
                    if (walls[wi] == thisPos)
                    {
                        IsWall = true;
                        break;
                    }
                }
                if (IsWall) continue;

                // -- PassOnly ? --

                bool isPassOnly = false;
                for (int pi = 0; pi < cantStops.Count; pi++)
                {
                    if (cantStops[pi] == thisPos)
                    {
                        isPassOnly = true;
                    }
                }


                // Copy path
                BoardPath copyPath = new BoardPath(range);
                for (int j = 0; j < path.Count; j++)
                {
                    copyPath.Add(path[j]);
                }

                // append this neibor pos
                copyPath.Add(thisPos);

                if (!isPassOnly)
                {
                    if (TryAppendNewPath(acc, copyPath))
                    {
                    }
                }

                _CreatePathes(acc, copyPath, range - 1, state, walls, cantStops);
            }
        }

        /// <summary>
        /// Acc は、移動距離が最も少なく、曲がった回数が最も少ない path だけを保存する。
        /// 新しい path を上記の条件を満たすときのみ追加する
        /// </summary>
        static bool TryAppendNewPath(List<BoardPath> acc, BoardPath newPath)
        {
            var newLastPos = newPath[newPath.Count - 1];
            var accSize = acc.Count;

            // Find same goal
            for (int i = 0; i < acc.Count; i++)
            {
                var current = acc[i];
                var currentLastPos = current[current.Count - 1];

                // Found!
                if (currentLastPos == newLastPos)
                {
                    // -- Compare size --

                    if (current.ForceLength > newPath.ForceLength)
                    {
                        // Exchange
                        acc[i] = newPath;

                        return true;
                    }
                    else if (current.Count == newPath.Count)
                    {
                        // -- Compare line --

                        if (current.ForceCountLine > newPath.ForceCountLine)
                        {
                            // Exchange
                            acc[i] = newPath;

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            acc.Add(newPath);

            return true;
        }

        /// <summary>
        /// Determin winner
        /// </summary>
        /// <returns>
        /// return array of winner
        /// null : no winner
        /// </returns>
        static public int[] Winners(TBGState state)
        {
            bool isAllUnitDeadA = true;
            bool isAllUnitDeadB = true;

            for (int i = 0; i < state.Units.Length; i++)
            {
                var unit = state.Units[i];

                if (unit.Owner == 0 && unit.IsAlive)
                {
                    isAllUnitDeadA = false;
                }

                if (unit.Owner == 1 && unit.IsAlive)
                {
                    isAllUnitDeadB = false;
                }
            }

            if (isAllUnitDeadA && isAllUnitDeadB)
            {
                return new int[] { 0, 1 };
            }
            else if (isAllUnitDeadA)
            {
                return new int[] { 1 };
            }
            else if (isAllUnitDeadB)
            {
                return new int[] { 0 };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 生存している Unit の initiative を元に、
        /// 行動順を生成
        /// </summary>
        public static List<int> CreateUnitWaitList(Unit[] units)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(units.Length >= 1);
#else
            System.Diagnostics.Debug.Assert(units.Length >= 1);
# endif

            // -- Initialize -

            var sort = new int[units.Length]; 
            for(int i = 0; i < units.Length; i++) {
                sort[i] = i;
            }

            // -- Bubble sort --

            for (int i = 0; i < units.Length - 1; i++)
            {
                for (int j = units.Length - 1; j > i; j--)
                {
                    if (units[j].Initiative > units[i].Initiative)
                    {
                        int swp = sort[j];
                        sort[j] = sort[i];
                        sort[i] = swp;
                    }
                }
            }

            // remove dead unit
            var onlyAlivedUnitIndex = new List<int>(sort.Length);
            for (int i = 0; i < units.Length; i++)
            {
                var uIndex = sort[i];
                if (units[uIndex].IsAlive)
                {
                    onlyAlivedUnitIndex.Add(uIndex);
                }
            }

            return onlyAlivedUnitIndex;
        }
    }
}
