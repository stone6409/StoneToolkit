namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 叶子节点，代表一个显示格。
    /// </summary>
    public class ContentNode : LayoutNode
    {
        private string _contentKey;
        private string _title;

        /// <summary>内容标识，用于从注册表创建实际视图。</summary>
        public string ContentKey
        {
            get => _contentKey;
            set
            {
                if (_contentKey == value)
                {
                    return;
                }

                _contentKey = value;
                OnPropertyChanged();
            }
        }

        /// <summary>格子标题栏文字。</summary>
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value)
                {
                    return;
                }

                _title = value;
                OnPropertyChanged();
            }
        }
    }
}
