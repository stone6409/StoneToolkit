namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 布局树的编辑操作。
    /// </summary>
    public static class LayoutOperations
    {
        /// <summary>
        /// 把目标格子替换为分割容器，目标格子与新建格子分列两侧。
        /// </summary>
        /// <param name="root">当前根节点。</param>
        /// <param name="target">被分割的格子。</param>
        /// <param name="orientation">分割方向。</param>
        /// <param name="newContent">新产生的格子。</param>
        /// <returns>操作后的根节点。</returns>
        public static LayoutNode Split(
            LayoutNode root,
            ContentNode target,
            SplitOrientation orientation,
            ContentNode newContent)
        {
            // 必须在挂接新节点之前取出父节点。
            SplitNode parent = target.Parent;

            SplitNode split = new SplitNode
            {
                Orientation = orientation,
                First = target,
                Second = newContent
            };

            split.Parent = parent;
            if (parent == null)
            {
                return split;
            }

            if (ReferenceEquals(parent.First, target))
            {
                parent.First = split;
            }
            else
            {
                parent.Second = split;
            }

            return root;
        }

        /// <summary>
        /// 关闭格子，其父容器由兄弟节点顶替；根节点不可关闭。
        /// </summary>
        /// <param name="root">当前根节点。</param>
        /// <param name="target">要关闭的格子。</param>
        /// <returns>操作后的根节点。</returns>
        public static LayoutNode Close(LayoutNode root, ContentNode target)
        {
            SplitNode parent = target.Parent;
            if (parent == null)
            {
                return root;
            }

            LayoutNode sibling = ReferenceEquals(parent.First, target)
                ? parent.Second
                : parent.First;
            SplitNode grandParent = parent.Parent;

            sibling.Parent = grandParent;
            if (grandParent == null)
            {
                return sibling;
            }

            if (ReferenceEquals(grandParent.First, parent))
            {
                grandParent.First = sibling;
            }
            else
            {
                grandParent.Second = sibling;
            }

            return root;
        }
    }
}
