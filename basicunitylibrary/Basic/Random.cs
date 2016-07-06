// #define BASIC__RANDOM__SEED

namespace Basic
{
    public class Random : System.Random
    {
        static Random _instance = null;
        public static Random Instance
        {
            get
            {
                if (Random._instance == null)
                {
#if BASIC__RANDOM__SEED // For debugging
                    Random._instance = new Random(0);
#else
                    Random._instance = new Random();
#endif
                }

                return Random._instance;
            }
        }

        /// <summary>
        /// Generate random number min to max - 1
        /// </summary>
        public int Int(int max, int min = 0)
        {
            int range = max - min;

            return min + _instance.Next() / range;
        }
    }
}
