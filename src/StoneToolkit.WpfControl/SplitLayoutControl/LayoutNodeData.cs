namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 布局树的序列化数据传输对象。
    /// </summary>
    public class LayoutNodeData
    {
        /// <summary>节点类型："Split" 或 "Content"。</summary>
        public string NodeType { get; set; } = "Content";

        /// <summary>分割方向（仅 Split 节点有效）。</summary>
        public SplitOrientation Orientation { get; set; }

        /// <summary>第一子节点占比（仅 Split 节点有效）。</summary>
        public double Ratio { get; set; } = 0.5;

        /// <summary>内容标识（仅 Content 节点有效）。</summary>
        public string ContentKey { get; set; }

        /// <summary>标题（仅 Content 节点有效）。</summary>
        public string Title { get; set; }

        /// <summary>左 / 上子节点。</summary>
        public LayoutNodeData First { get; set; }

        /// <summary>右 / 下子节点。</summary>
        public LayoutNodeData Second { get; set; }
    }
}
