// #define DEBUG_HP1
// #define DEBUG_PLAYER_VS_PLAYER

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using BoardGame;
using TacticsBG;

namespace Scene.TacticsBattle
{
    /// <summary>
    /// TacticsBG Class を使用して、フィールドやユニットのアニメーションを管理する
    /// </summary>
    public class TacticsBattleSceneManager : MonoBehaviour
    {
        static int BOARD_WIDTH = 10;
        static int BOARD_HEIGHT = 6;

        static float WORLD_UNIT_DISTANCE = 1.5f; // 2.2f
        static float DELAY_TURN = 0.5f;

        static int AI_DEPTH = 2;

        public delegate void CreateDamageTextDelegate(int damage, Vector3 position);

        bool isPlayerAI;
        bool isEnemyAI;
        bool isPlayerVsPlayer;
        bool isReplaying;

        TacticsBG.TacticsBG tacticsBG;


        [SerializeField]
        UnityBehaviour.Camera.ChaseCameraBehaviour camera;
        [SerializeField]
        GameObject cameraTarget = null;
        [SerializeField]
        GameObject unitObjectHolder = null;
        [SerializeField]
        Canvas canvas = null;
        [SerializeField]
        Canvas worldCanvas = null;

        [SerializeField]
        BattleResultController windowGameEnd = null;
        // ----

        List<TacticsUnitController> unitControllers;
        FloorButtonController[,] floorButtonControllers;
        List<TargetImageController> targetImages;
        List<AttackDirectionController> attackDirectionImages;
        
        /// <summary>
        /// Tactics battle の初期設定 を行う
        /// </summary>
        void Awake()
        {
            var args = SceneArgumentManager.ReceiveArgs();
            if (args != null && args.Count == 2)
            {
                isPlayerAI = (bool)args[0];
                isEnemyAI = (bool)args[1];

                if (isPlayerAI == false && isEnemyAI == false)
                {
                    isPlayerVsPlayer = true;
                }
            }
            else
            {
                //isPlayerAI = false;
#if DEBUG_PLAYER_VS_PLAYER
                isPlayerAI = false;
                isEnemyAI = false;
                isPlayerVsPlayer = true;
#else
                isPlayerAI = false;
                isEnemyAI = true;
                isPlayerVsPlayer = false;
#endif
            }

            isReplaying = false;

            // -- Create units


            // 0-2 or 7-11

            var units = new List<Unit>();

            units.Add(UnitConstructor.Construct(Unit.Type.Amazon, 0, new Basic.Vec2Int(2, 2)));
            units.Add(UnitConstructor.Construct(Unit.Type.Mage, 0, new Basic.Vec2Int(1, 4)));
//            units.Add(UnitConstructor.Construct(Unit.Type.Mage, 0, new Basic.Vec2Int(5, 3)));


            if (false)
            {
                units.Add(UnitConstructor.Construct(Unit.Type.Mage, 1, new Basic.Vec2Int(6, 4)));
                units.Add(UnitConstructor.Construct(Unit.Type.Mage, 1, new Basic.Vec2Int(7, 1)));
                units.Add(UnitConstructor.Construct(Unit.Type.Mage, 1, new Basic.Vec2Int(7, 3)));
            }
            else
            {
                //units.Add(UnitConstructor.Construct(Unit.Type.Skeleton, 1, new Basic.Vec2Int(3, 3)));
                units.Add(UnitConstructor.Construct(Unit.Type.Skeleton, 1, new Basic.Vec2Int(6, 4)));
                units.Add(UnitConstructor.Construct(Unit.Type.Skeleton, 1, new Basic.Vec2Int(7, 1)));
                units.Add(UnitConstructor.Construct(Unit.Type.Skeleton, 1, new Basic.Vec2Int(7, 3)));
            }

#if DEBUG_HP1
                foreach (var u in units)
                {
                    u.HpCurrent = 1;
                }
#endif

            // enemy
            /*
            units.Add(UnitConstructor.Construct(Unit.Type.Mage, 0, new Basic.Vec2Int(0, 0)));4
            units.Add(UnitConstructor.Construct(Unit.Type.Mage, 0, new Basic.Vec2Int(2, 3)));
            units.Add(UnitConstructor.Construct(Unit.Type.Mage, 1, new Basic.Vec2Int(7, 4)));*/
            
            /*
            units.Add(UnitConstructor.Construct(Unit.Type.Zombie, 1, new Basic.Vec2Int(6, 4)));
            units.Add(UnitConstructor.Construct(Unit.Type.Skeleton, 1, new Basic.Vec2Int(7, 0)));
            units.Add(UnitConstructor.Construct(Unit.Type.Skeleton, 1, new Basic.Vec2Int(7, 3)));*/
            
            

            // -- Create walls --
            // 3 - 6

            var walls = new List<Basic.Vec2Int>();
            walls.Add(new Basic.Vec2Int(3, 5));
            walls.Add(new Basic.Vec2Int(4, 2));
            walls.Add(new Basic.Vec2Int(6, 1));
            walls.Add(new Basic.Vec2Int(5, 4));

            var arrUnits = units.ToArray();
            this.tacticsBG = new TacticsBG.TacticsBG(new TBGState(
                2,
                new Basic.Vec2Int(BOARD_WIDTH, BOARD_HEIGHT),
                walls.ToArray(),
                units.ToArray(),
                TacticsBG.TacticsBG.CreateUnitWaitList(arrUnits)));

            // 
            targetImages = new List<TargetImageController>(units.Count);
            attackDirectionImages = new List<AttackDirectionController>(8);
        }

        /// <summary>
        /// GameObject の初期化 を行う
        /// 処理の最後に Main() を呼ぶ
        /// </summary>
        void Start()
        {
            Debug.Log(DebugUtil.FN);

            AudioManager.Script().BGMCrossFade(Define.Path.BGM.Battle);

            this.InitUnitObjects();
            this.InitFloorButtons();

            this.camera.Lerp(this.camera.Distance * 1.2f, this.camera.Distance, 2f);

            this.HandleStateNode();
        }

        void InitUnitObjects()
        {
            Object prefUnit = Resources.Load(Define.Path.Models.Unit);
            this.unitControllers = new List<TacticsUnitController>();

            var state = tacticsBG.StateNodeRoot.State as TBGState;

            for (int i = 0; i < state.Units.Length; i++)
            {
                var unit = state.Units[i];
                var prefModel = ResourceManager.Instance.GetUnitPrefab(unit.Type_);
                var modelObject = (GameObject)Instantiate(prefModel, Vector3.zero, Quaternion.identity);
                var unitObject = (GameObject)Instantiate(prefUnit, ConvertBoardToWorldPos(unit.Pos), Quaternion.identity);

                // 上で指定した world pos を保持
                unitObject.transform.SetParent(unitObjectHolder.transform, true);
                // unit Object からの相対位置
                modelObject.transform.SetParent(unitObject.transform, false);
                modelObject.transform.Rotate(new Vector3(0f, unit.Owner == 0 ? 90f : 270f, 0f));

                var unitController = unitObject.GetComponent<TacticsUnitController>();
                unitController.Init(modelObject, unit.Owner, unit.Type_);
                this.unitControllers.Add(unitController);
            }
        }

        Vector3 ConvertBoardToWorldPos(Basic.Vec2Int pos, float addY)
        {
            var cameraPos = cameraTarget.transform.position;
            return new Vector3(
                    (pos.x - BOARD_WIDTH / 2f + 0.5f) * WORLD_UNIT_DISTANCE + cameraPos.x,
                    addY,
                    (pos.y - BOARD_HEIGHT / 2f + 0.5f) * WORLD_UNIT_DISTANCE + cameraPos.z);
        }

        Vector3 ConvertWalkToWorldPos(Vector3 pos)
        {
            var cameraPos = cameraTarget.transform.position;
            return new Vector3(
                    (pos.x - BOARD_WIDTH / 2f + 0.5f) * WORLD_UNIT_DISTANCE + cameraPos.x,
                    pos.y,
                    (pos.z - BOARD_HEIGHT / 2f + 0.5f) * WORLD_UNIT_DISTANCE + cameraPos.z);
        }

        Vector3 ConvertBoardToWorldPos(Basic.Vec2Int pos)
        {
            return ConvertBoardToWorldPos(pos, 0f);
        }

        void InitFloorButtons()
        {
            Object prefFloorButton = ResourceManager.Instance.LoadUI(Define.Path.UI.FloorButton);
            this.floorButtonControllers = new FloorButtonController[BOARD_HEIGHT, BOARD_WIDTH];
            var walls = tacticsBG.StateCurrent.Walls;

            for (int y = 0; y < BOARD_HEIGHT; y++)
            {
                for (int x = 0; x < BOARD_WIDTH; x++)
                {
                    // -- Skip wall --

                    var isSkipPlace = false;
                    for (int i = 0; i < walls.Length; i++)
                    {
                        if (x == walls[i].x && y == walls[i].y)
                        {
                            isSkipPlace = true;
                            break;
                        }
                    }

                    if (isSkipPlace) continue;
                    // ----

                    var uiFloor = Instantiate(prefFloorButton) as GameObject;
                    // uiFloor.GetComponent<Image>().CrossFadeColor(new Color(1f, 0f, 0f, 1f), 2f, false, true);
                    var uiFController = uiFloor.GetComponent<FloorButtonController>();
                    var pos3 = ConvertBoardToWorldPos(new Basic.Vec2Int(x, y));


                    uiFloor.name = string.Format("{0} y:{1} x:{2}", Define.Path.UI.FloorButton, y, x);
                    
                    uiFloor.transform.SetParent(worldCanvas.transform, false);

                    //uiFloor.GetComponent<Image>().color = new Color()
                    floorButtonControllers[y, x] = uiFController;
                    uiFController.Init(new Vector2(pos3.x, pos3.z), WORLD_UNIT_DISTANCE);
                }
            }
        }

        void SelectActionNode(int index)
        {
            tacticsBG.SelectActionNode(index);
        }

        void HandleStateNode()
        {
            var stateNode = tacticsBG.StateNodeCurrent;
            var state = tacticsBG.StateCurrent;
            Debug.Log(DebugUtil.FN + string.Format("Player {0} Turn", state.PlayerCurrent));
            SyncState(state);
            DisableUI();


            if (isReplaying)
            {
                if (tacticsBG.StateNodeCurrent.ActionNodeNext == null)
                {
                    GameEnd(state);
                }
                else
                {
                    tacticsBG.StateNodeCurrent = stateNode.ActionNodeNext.StateNodeNext;
                    AnimationThenHandleStateNode(stateNode.ActionNodeNext.Action);
                }
            }
            else
            {
                StartCoroutine(Coroutine_.Action.WaitFunc(DELAY_TURN, () =>
                {

                    bool isAI = (state.PlayerCurrent == 0 && isPlayerAI)
                        || (state.PlayerCurrent == 1 && isEnemyAI);
                    var actions = tacticsBG.ForceActionList(isAI);

                    if (actions.Count == 0)
                    {
                        this.GameEnd(state);
                    }
                    else if (isAI)
                    {
                        // var select = tacticsBG.se
                        float[] scores = tacticsBG.CreateScores(state.PlayerCurrent, stateNode, AI_DEPTH);
                        float scoreMax = Basic.Sequence.Apply(scores, max, 0f);
                        int index = Basic.Sequence.Index(scores, scoreMax);

                        SelectActionNode(index);
                        AnimationThenHandleStateNode(actions[index].ActionTrue.Action);
                    }
                    else
                    {
                        UpdateUIForHuman(state, actions);
                    }
                }));
            }
        }

        float max(float a, float b)
        {
            return Mathf.Max(a, b);
        }

        void GameEnd(TBGState state)
        {
            var winner = TacticsBG.TacticsBG.Winners(state);
            Debug.Log(DebugUtil.FN + "Game End");

            StartCoroutine(Coroutine_.Action.WaitFunc(2f, () =>
            {
                windowGameEnd.gameObject.SetActive(true);
                windowGameEnd.Init("墓荒らし", "墓守",
                    winner[0] == 0 ? true : false,
                    isPlayerVsPlayer,
                    // LButton
                    this.Replay,
                    // RButton
                    () => { SceneArgumentManager.LoadScene(Define.Scene.Title); });
            }));
        }

		void AnimationThenHandleStateNode(ActionBase action)
        {
            {
                var cast = action as TBGActionMove;
                if (cast != null)
                {
                    AnimationMoveThenHandleStateNode(cast);
                    return;
                }
            }

            {
                var cast = action as TBGActionAttackRanged;
                if (cast != null)
                {
                    AnimationAttackRangedThenHandleStateNode(cast);
                    return;
                }
            }

            {
                var cast = action as TBGActionAttackMelee;
                if (cast != null)
                {
                    AnimationAttackMeleeThenHandleStateNode(cast);
                    return;
                }
            }

            Debug.Assert(false, "Please implement animation");
        }

        /// <summary>
        /// Execute move animation then select node.
        /// </summary>
		void AnimationMoveThenHandleStateNode(TBGActionMove action)
        {
            this.DisableUI();

            AnimationMove(action.UnitIndex, action.Path);

            var unit = unitControllers[action.UnitIndex];

            unit.CoSequencer.Add(Coroutine_.Action.Func(() => HandleStateNode()));
        }

        void AnimationMove(int unitIndex, BoardPath path)
        {
            var unit = unitControllers[unitIndex];
            var movePath = BoardPathToWalkPathConverter.Convert(path);
            
            // totdo furimuki

            foreach (var item in movePath)
            {
                Debug.Log(item);
                // fix postion
                var copy = item;
                copy.PosStart = ConvertWalkToWorldPos(item.PosStart);
                copy.PosEnd = ConvertWalkToWorldPos(item.PosEnd);

                unit.CoSequencer.Add(unit.CoMove2(copy));
            }
        }

        /// <summary>
        /// Execute ranged attack animation then select node.
        /// </summary>
		void AnimationAttackRangedThenHandleStateNode(TBGActionAttackRanged action)
        {
            this.DisableUI();


            var unit = unitControllers[action.UnitIndex];
            var unitTarget = unitControllers[action.TargetUnitIndex];
            var vec = action.TargetUnitPos - action.PosFrom;

            unit.CoSequencer.Add(new IEnumerator[]
            {
                unit.CoTurn(new Vector3(vec.x, 0f, vec.y)),
                unitTarget.CoTurn(new Vector3(- vec.x, 0f, - vec.y))
            });
            unit.CoSequencer.Add(unit.CoFire(
                action.Damage,
                ConvertBoardToWorldPos(action.PosFrom),
                ConvertBoardToWorldPos(action.TargetUnitPos),
                unitTarget, action.IsTargetDead, 
                this.CreateDamageText));
            unit.CoSequencer.Add(Coroutine_.Action.Func(() => HandleStateNode()));
        }

		void AnimationAttackMeleeThenHandleStateNode(TBGActionAttackMelee action)
        {
            this.DisableUI();

            var unit = this.unitControllers[action.UnitIndex];

            var path = action.Path;

            if (path.Count >= 2)
            {
                AnimationMove(action.UnitIndex, action.Path);
            }

            var attackVec = action.TargetUnitPos - action.Path.Last;

            unit.CoSequencer.Add(unit.CoTurn(
                new Vector3(attackVec.x, 0f, attackVec.y)));

            unit.CoSequencer.Add(unit.CoMelee(action.Damage, this.unitControllers[action.TargetUnitIndex], action.IsTargetDead, this.CreateDamageText));
            unit.CoSequencer.Add(Coroutine_.Action.Func(() => HandleStateNode()));
        }

        /// <summary>
        /// Hp やユニットの位置などを同期させる
        /// </summary>
        /// <param name="state"></param>
        void SyncState(TBGState state)
        {
            var units = state.Units;
            for (int i = 0; i < units.Length; i++)
            {
                unitControllers[i].transform.position = ConvertBoardToWorldPos(units[i].Pos);

                if (unitControllers[i].HpSlider.gameObject.activeSelf == true)
                    unitControllers[i].SetHpbar(units[i].HpFraction);
            }
        }

        void DisableUI()
        {
            foreach (var floorButton in floorButtonControllers)
            {
                if (floorButton != null)
                    floorButton.ChangeModeGridOnly();
            }

            this.AttackDirectionImagesClear();
            this.TargetImagesClear();
        }

        void UpdateUIForHuman(TBGState state, List<ActionContainer> actions)
        {
            var unitCurrent = state.UnitCurrent;

            // 選択状態のユニットを目立たせる
            floorButtonControllers[unitCurrent.Pos.y, unitCurrent.Pos.x].ChangeModeCurrentUnit();

            this.InterpretActionList(state, this.tacticsBG.ForceActionList(false));
        }

        /// <summary>
        /// Action list を解釈し、UIを設定する
        /// </summary>
        /// <param name="actions"></param>
        void InterpretActionList(TBGState state, List<ActionContainer> actions)
        {
            var size = actions.Count;
            for (int i = 0; i < size; i++)
            {
                var actionNode = actions[i].ActionTrue;
                var action = actions[i].ActionTrue.Action;
                int captureIndex = i;

                // -- Move

                var actMove = action as TBGActionMove;
                if (actMove != null)
                {
                    Basic.Vec2Int goalPos = actMove.Path[actMove.Path.Count - 1];

                    // Create move button
                    floorButtonControllers[goalPos.y, goalPos.x].ChangeModeMovable(() =>
                    {
                        this.SelectActionNode(captureIndex);
                        this.AnimationMoveThenHandleStateNode(actMove);
                    });
                }

                // -- Attack ranged

                var actRanged = action as TBGActionAttackRanged;
                if (actRanged != null)
                {
                    this.targetImages.Add(this.TargetImageCreate(Define.Path.Sprite.WeaponBow, actRanged.TargetUnitIndex));

                    // Create ranged attack button
                    floorButtonControllers[actRanged.TargetUnitPos.y, actRanged.TargetUnitPos.x].ChangeModeTarget(() =>
                    {
                        this.TargetImagesClear();
                        this.SelectActionNode(captureIndex);
                        this.AnimationAttackRangedThenHandleStateNode(actRanged);
                    });
                }
            }

            // -- Melee attack 

            // Collect index and melee attack
            var actionIndexAndMeleeAttacks = new Dictionary<int, TBGActionAttackMelee>(actions.Count);
            for (int i = 0; i < actions.Count; i++)
            {
                var action = actions[i].ActionTrue.Action;
                var cast = action as TBGActionAttackMelee;
                if (cast != null)
                {
                    actionIndexAndMeleeAttacks.Add(i, cast);
                }
            }

            if (actionIndexAndMeleeAttacks.Count > 0)
            {
                for (int indexUnit = 0; indexUnit < state.Units.Length; indexUnit++)
                {
                    // Collect attack action of attacking same target
                    var actionIndexAndAttackMeleeTargetUnits = new Dictionary<int, TBGActionAttackMelee>(actionIndexAndMeleeAttacks.Count);
                    var attackTargetPos = Basic.Vec2Int.Zero;
                    var captureUnitIndex = indexUnit;

                    foreach (KeyValuePair<int, TBGActionAttackMelee> indexAndAttack in actionIndexAndMeleeAttacks)
                    {
                        if (indexAndAttack.Value.TargetUnitIndex == indexUnit)
                        {
                            attackTargetPos = indexAndAttack.Value.TargetUnitPos;
                            actionIndexAndAttackMeleeTargetUnits.Add(indexAndAttack.Key, indexAndAttack.Value);
                        }
                    }

                    // Create target button
                    if (actionIndexAndAttackMeleeTargetUnits.Count >= 1)
                    {
                        this.targetImages.Add(this.TargetImageCreate(Define.Path.Sprite.WeaponAxe, captureUnitIndex));

                        // Create melee attack button
                        floorButtonControllers[attackTargetPos.y, attackTargetPos.x].ChangeModeTarget(() =>
                        {
                            this.DisableUI();
                            this.TargetImagesClear();
                                
                            var captureAttacks = actionIndexAndAttackMeleeTargetUnits;
                            var captureX = attackTargetPos.x;
                            var captureY = attackTargetPos.y;

                            floorButtonControllers[captureY, captureX].ChangeModeTargetNoClick();

                            foreach (KeyValuePair<int, TBGActionAttackMelee> indexAndAttack in captureAttacks)
                            {

                                var index = indexAndAttack.Key;
                                var attack = indexAndAttack.Value;
                                var attackFromPos = attack.Path.Last;
                                var attackVec = attack.TargetUnitPos - attackFromPos;
                                var unitObject = unitControllers[attack.UnitIndex];
                                var targetUnitObject = unitControllers[attack.TargetUnitIndex];

                                bool isAttackFromActiveUnit = false;
                                if (attackFromPos == state.UnitCurrent.Pos)
                                    isAttackFromActiveUnit = true;

                                // Create attack direction image
                                this.attackDirectionImages.Add(AttackDirectionImageCreate(attackFromPos, attack.TargetUnitPos));

                                // create attack direction button
                                floorButtonControllers[attackFromPos.y, attackFromPos.x].ChangeModeAttackDirection(() =>
                                {
                                    this.AttackDirectionImagesClear();
                                    this.SelectActionNode(index);
                                    this.AnimationAttackMeleeThenHandleStateNode(attack);
                                }, isAttackFromActiveUnit);
                            }
                    });
                    }
                }
            }
        }

        void Replay()
        {
            Debug.Log(DebugUtil.FN);

            isReplaying = true;


            this.tacticsBG.StateNodeCurrent = this.tacticsBG.StateNodeRoot;
            foreach (var u in unitControllers) {
                Destroy(u.gameObject);
            }
            InitUnitObjects();

            StartCoroutine(Coroutine_.Action.WaitFunc(1f, this.HandleStateNode));
        }

        TargetImageController TargetImageCreate(string imageName, int targetUnitIndex)
        {
            var target = (GameObject)Instantiate(ResourceManager.Instance.LoadUI(Define.Path.UI.AttackTarget));
            target.transform.SetParent(canvas.transform, false);

            var controller = target.GetComponent<TargetImageController>();
            controller.Init(imageName, () =>
            {
                return this.unitControllers[targetUnitIndex].transform.position;
            });

            return controller;
        }

        void TargetImagesClear()
        {
            foreach (var item in targetImages)
            {
                GameObject.Destroy(item.gameObject);
            }

            targetImages.Clear();
        }

        AttackDirectionController AttackDirectionImageCreate(Basic.Vec2Int pos, Basic.Vec2Int posTarget)
        {
            var target = (GameObject)Instantiate(ResourceManager.Instance.LoadUI(Define.Path.UI.AttackDirection));
            var posAdd = ConvertBoardToWorldPos(pos);
            target.transform.position = new Vector3(posAdd.x, 0.3f, posAdd.z);
            target.transform.SetParent(worldCanvas.transform);

            var controller = target.GetComponent<AttackDirectionController>();
            controller.Init(pos, posTarget, ConvertBoardToWorldPos);
            
            return controller;
        }

        void AttackDirectionImagesClear()
        {
            foreach (var item in attackDirectionImages)
            {
                GameObject.Destroy(item.gameObject);
            }

            attackDirectionImages.Clear();
        }

        void CreateDamageText(int damage, Vector3 position)
        {
            var damageText = Instantiate(ResourceManager.Instance.LoadUI(Define.Path.UI.FloatingDamageText)) as GameObject;
            damageText.transform.SetParent(this.canvas.transform, false);
            damageText.GetComponent<FloatingDamageController>().Init(damage, position); 
        }
    }
}
