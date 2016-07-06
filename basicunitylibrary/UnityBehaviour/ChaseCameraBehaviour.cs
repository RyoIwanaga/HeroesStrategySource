using UnityEngine;
using System.Collections;

using UnityBasic;


namespace UnityBehaviour.Camera
{
    public class ChaseCameraBehaviour : MonoBehaviour
    {
        public float Distance = 13f;
        // 0 ~ 180f
        public float RotationZ = 30f;
        // 0 ~ 360f
        public float RotationY = 0f;

        public GameObject targetObject = null;
        public Vector3 AddPosition = new Vector3(0f, 1f, 0f);

        public float speed = 1f;
        Vector3 posDesire;

        SphericalPolarCrood sPC;

        public Vector3 PosLooking
        {
            get { return targetObject.transform.position + AddPosition; }
        }

        Act.Actor _actor;
        public Act.Actor Actor { get { return _actor; } }

        #region === Unity message ===

        /// <summary>
        /// OnValidate()が先に呼ばれてしまうので、OnValidate()でも初期化処理が必要ならば実行
        /// </summary>
        void Awake()
        {
            sPC = new SphericalPolarCrood(Distance, RotationZ, RotationY);
            _actor = new Act.Actor(this);
        }

        void Start()
        {
            OnValidate();
        }

        void LateUpdate()
        {
            if (! _actor.Apply())
                UpdateTransform();
        }

        void OnValidate()
        {
            this.Distance = Mathf.Max(0f, this.Distance);
            this.RotationZ = Basic.Util.InRange(this.RotationZ, SphericalPolarCrood.MAX_THETA_DEG);
            this.RotationY = Basic.Util.InRange(this.RotationY, SphericalPolarCrood.MAX_PHI_DEG);

            if (sPC == null)
                return;

            sPC.R = Distance;
            sPC.Theta = Mathf.Deg2Rad * RotationZ;
            sPC.Phi = Mathf.Deg2Rad * RotationY;

            UpdateTransform();
        }

        #endregion

        void UpdateTransform()
        {
            /* Change pos */

            var posLooking = this.PosLooking;
            /*
            var sub = this.transform.position - targetPos;

            this.sPC.Theta = Mathf.Atan2(sub.z, sub.x);

            var desirePos = targetPos + this.sPC.Position;

            var displace = this.transform.position - desirePos; */
  //          var accelSpring = (-1 * displace) - ()

            
            var posLocal = sPC.Position;
            var posFixedTarget = targetObject.transform.position + AddPosition;

            this.transform.position = posLocal + posFixedTarget;



            //            /* Change rotate */

            transform.LookAt(posLooking);

            RaycastHit hit;
            var rayStart = posFixedTarget;
            var direction = this.transform.position - rayStart;
            if (Physics.Raycast(rayStart,  direction, out hit, this.Distance))
            {
//                Debug.Log(DebugUtil.FN + "hit" + hit.point);
                this.transform.position = hit.point;
            }

            Debug.DrawRay(rayStart, direction);
        }

        public void Lerp(float distanceStart, float distanceEnd, float second)
        {
            this.Actor.Add(Act.LerpCustom<float>.Create(distanceStart, distanceEnd, second, this.LerpDistanceFunc));
        }

        void LerpDistanceFunc(float distanceStart, float distanceEnd, float frac)
        {
            this.Distance = Mathf.Lerp(distanceStart, distanceEnd, frac);

            OnValidate();
        }
    }
}

