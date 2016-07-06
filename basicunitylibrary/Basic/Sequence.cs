using System.Collections;

namespace Basic
{
    static public class Sequence
    {
        public static T Apply<T>(IEnumerable seq, System.Func<T, T, T> fn, T start)
        {
            int i = 0;
            T win = start;

            foreach (var item in seq)
            {
                var value = (T)item;

                if (i == 0)
                {
                    win = value;
                }
                else
                {
                    win = fn(win, value);
                }

                i++;
            }

            return win;
        }

        /// <summary>
        /// Return index of item in sequince
        /// </summary>
        /// <returns>Return -1 when fail to find.</returns>
        public static int Index<T>(IEnumerable seq, T item)
        {
            int i = 0;

            foreach (var thing in seq)
            {
                if (item.Equals((T)thing))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        public static bool Every<T>(IEnumerable seq, System.Func<T, bool> pred)
        {
            bool isTrueAll = true;

            foreach(var item in seq)
            {
                if (!pred((T)item))
                    isTrueAll = false;
            }

            return isTrueAll;
        }
    }
}
