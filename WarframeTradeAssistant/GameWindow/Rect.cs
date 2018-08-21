namespace WarframeTradeAssistant.GameWindow
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        private readonly int left;

        private readonly int top;

        private readonly int right;

        private readonly int bottom;

        public Rect(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public int Bottom { get => this.bottom; }

        public int Left { get => this.left; }

        public int Right { get => this.right; }

        public int Top { get => this.top; }

        public static bool operator ==(Rect left, Rect right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rect left, Rect right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            var o = (Rect)obj;
            return this.left == o.left && this.top == o.top && this.right == o.right && this.bottom == o.bottom;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this.left.GetHashCode();
                hash = hash * 23 + this.top.GetHashCode();
                hash = hash * 23 + this.right.GetHashCode();
                hash = hash * 23 + this.bottom.GetHashCode();
                return hash;
            }
        }
    }
}
