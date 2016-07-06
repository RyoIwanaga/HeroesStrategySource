using UnityEngine;

namespace UnityBasic
{
    /// <summary>
    /// 左手座標系（Unityと同じ）用の球面極座標
    /// </summary>
    public class SphericalPolarCrood
    {
        public static float MAX_THETA = Mathf.PI;
        public static float MAX_Phi = 2f * Mathf.PI;
        public static float MAX_THETA_DEG = 180f;
        public static float MAX_PHI_DEG = 90f;

        public float R { get; set; }
        public float Theta { get; set; }
        public float Phi { get; set; }

        public float X
        {
            get { return this.R * Mathf.Sin(this.Theta) * Mathf.Sin(this.Phi); }
        }

        public float Y
        {
            get { return this.R * Mathf.Cos(this.Theta); }
        }

        public float Z
        {
            // 数学では右手座標を使うので、左手座標に変換するために -1 を掛ける
            get { return -this.R * Mathf.Sin(this.Theta) * Mathf.Cos(this.Phi); }
        }

        public Vector3 Position
        {
            get { return new Vector3(this.X, this.Y, this.Z); }
        }

        // TODO
        public SphericalPolarCrood(Vector3 vec)
        {

        }

        public SphericalPolarCrood(float r, float t, float p)
        {
            this.R = r;
            this.Theta = t;
            this.Phi = p;
        }
    }
}
