namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 默认版面生成器：按注册表顺序选取前三个内容，生成「左 + 右上 + 右下」的初始版面。
    /// </summary>
    public static class LayoutDefaults
    {
        /// <summary>按注册表内容生成默认版面。</summary>
        public static LayoutNode Create(ContentRegistry registry)
        {
            if (registry == null || registry.All.Count == 0)
            {
                return CreateContent(new EmptyContentFactory());
            }

            if (registry.All.Count == 1)
            {
                return CreateContent(registry.All[0]);
            }

            ContentNode third = registry.All.Count > 2
                ? CreateContent(registry.All[2])
                : CreateContent(new EmptyContentFactory());

            return new SplitNode
            {
                Orientation = SplitOrientation.Horizontal,
                Ratio = 0.32,
                First = CreateContent(registry.All[0]),
                Second = new SplitNode
                {
                    Orientation = SplitOrientation.Vertical,
                    Ratio = 0.68,
                    First = CreateContent(registry.All[1]),
                    Second = third
                }
            };
        }

        private static ContentNode CreateContent(IContentFactory factory)
        {
            return new ContentNode
            {
                ContentKey = factory.Key,
                Title = factory.DisplayName
            };
        }
    }
}
