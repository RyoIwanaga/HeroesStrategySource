namespace BoardGame
{
    public class LinkedNode
    {
        public LinkedNode PrevNode { get; set; }
        public LinkedNode NextNode { get; set; }

        protected LinkedNode(LinkedNode prev, LinkedNode next)
        {
            this.PrevNode = prev;
            this.NextNode = next;
        }

        protected LinkedNode() : this(null, null)
        {
        }

        public static void Connect(LinkedNode prev, LinkedNode next)
        {
            prev.NextNode = next;
            next.PrevNode = prev;
        }

        public static void Cut(LinkedNode prev, LinkedNode next)
        {
            prev.NextNode = null;
            next.PrevNode = null;
        }

        public void Connect(LinkedNode node)
        {
            Connect(this, node);
        }

        public void Cut(LinkedNode node)
        {
            Cut(this, node);
        }
    }
}
