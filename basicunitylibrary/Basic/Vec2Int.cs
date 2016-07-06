namespace Basic
{
    public struct Vec2Int
    {
        public static Vec2Int Zero = new Vec2Int(0, 0);

		int _x;
        public int x {
			get {
				return _x;
			}

			set {
				_x = value;
			}
		}

		int _y;
        public int y {
			get {
				return _y;
			}

			set {
				_y = value;
			}
		}
	

        public Vec2Int(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public static Vec2Int operator + (Vec2Int lhs, Vec2Int rhs)
        {
            return new Vec2Int(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static Vec2Int operator - (Vec2Int lhs, Vec2Int rhs)
        {
            return new Vec2Int(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static bool operator == (Vec2Int lhs, Vec2Int rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator != (Vec2Int lhs, Vec2Int rhs)
        {
            return ! (lhs == rhs);
        }

		public override bool Equals(object o)
		{
			return false;
		}

		public override int GetHashCode ()
		{
			return this.y * 100 + this.x;
		}

        public override string ToString()
        {
            return string.Format("{0}, {1}", x, y);
        }
    }
}
