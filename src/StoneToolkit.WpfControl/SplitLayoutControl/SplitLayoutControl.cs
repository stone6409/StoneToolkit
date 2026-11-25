using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 分栏布局控件：把界面递归拆分为若干格子，每格承载任意内容视图，
    /// 支持左右 / 上下分栏、关闭格子、切换内容，并可序列化为 JSON 持久化。
    /// </summary>
    /// <remarks>
    /// XAML 用法：
    /// <code>
    /// &lt;sc:SplitLayoutControl x:Name="Layout" Root="{Binding LayoutRoot, Mode=TwoWay}" /&gt;
    /// </code>
    /// </remarks>
    public class SplitLayoutControl : ContentControl
    {
        /// <summary>布局树根节点。</summary>
        public static readonly DependencyProperty RootProperty =
            DependencyProperty.Register(
                nameof(Root),
                typeof(LayoutNode),
                typeof(SplitLayoutControl),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnRootChanged));

        /// <summary>版面 JSON。赋值时反序列化并刷新；读取时返回当前版面的序列化结果。</summary>
        public static readonly DependencyProperty LayoutJsonProperty =
            DependencyProperty.Register(
                nameof(LayoutJson),
                typeof(string),
                typeof(SplitLayoutControl),
                new FrameworkPropertyMetadata(string.Empty, OnLayoutJsonChanged));

        private static readonly DependencyPropertyKey RegistryPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Registry),
                typeof(ContentRegistry),
                typeof(SplitLayoutControl),
                new FrameworkPropertyMetadata(null));

        /// <summary>内容工厂注册表（只读依赖属性）。</summary>
        public static readonly DependencyProperty RegistryProperty = RegistryPropertyKey.DependencyProperty;

        /// <summary>分隔条粗细。</summary>
        public static readonly DependencyProperty SplitterThicknessProperty =
            DependencyProperty.Register(
                nameof(SplitterThickness),
                typeof(double),
                typeof(SplitLayoutControl),
                new FrameworkPropertyMetadata(4.0, OnAppearanceChanged));

        /// <summary>分隔条颜色。</summary>
        public static readonly DependencyProperty SplitterBrushProperty =
            DependencyProperty.Register(
                nameof(SplitterBrush),
                typeof(Brush),
                typeof(SplitLayoutControl),
                new FrameworkPropertyMetadata(CreateFrozenBrush(0xC7, 0xCF, 0xDA), OnAppearanceChanged));

        /// <summary>格子边框颜色。</summary>
        public static readonly DependencyProperty CellBorderBrushProperty =
            DependencyProperty.Register(
                nameof(CellBorderBrush),
                typeof(Brush),
                typeof(SplitLayoutControl),
                new FrameworkPropertyMetadata(CreateFrozenBrush(0xC7, 0xCF, 0xDA), OnAppearanceChanged));

        /// <summary>格子标题栏背景。</summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register(
                nameof(HeaderBackground),
                typeof(Brush),
                typeof(SplitLayoutControl),
                new FrameworkPropertyMetadata(CreateFrozenBrush(0x00, 0x7A, 0xCC), OnAppearanceChanged));

        /// <summary>格子标题栏前景。</summary>
        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register(
                nameof(HeaderForeground),
                typeof(Brush),
                typeof(SplitLayoutControl),
                new FrameworkPropertyMetadata(CreateFrozenBrush(0xFF, 0xFF, 0xFF), OnAppearanceChanged));

        private static readonly EmptyContentFactory FallbackFactory = new EmptyContentFactory();

        private bool _isApplyingJson;

        static SplitLayoutControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SplitLayoutControl),
                new FrameworkPropertyMetadata(typeof(SplitLayoutControl)));
        }

        public SplitLayoutControl()
        {
            SetValue(RegistryPropertyKey, new ContentRegistry());
        }

        /// <summary>任意结构性变更后触发。</summary>
        public event EventHandler LayoutChanged;

        /// <summary>内容工厂注册表。</summary>
        public ContentRegistry Registry => (ContentRegistry)GetValue(RegistryProperty);

        /// <summary>
        /// 追加到每个格子右键菜单末尾的全局菜单项提供者（如保存 / 加载 / 恢复默认版面）。
        /// </summary>
        /// <remarks>
        /// 每次构建菜单时调用一次；同一个 <see cref="MenuItem"/> 不能同时属于两个菜单，
        /// 因此实现方必须返回新实例。
        /// </remarks>
        public Func<IEnumerable<MenuItem>> MenuItemsFactory { get; set; }

        /// <summary>布局树根节点。</summary>
        public LayoutNode Root
        {
            get => (LayoutNode)GetValue(RootProperty);
            set => SetValue(RootProperty, value);
        }

        /// <summary>版面 JSON。</summary>
        public string LayoutJson
        {
            get => (string)GetValue(LayoutJsonProperty);
            set => SetValue(LayoutJsonProperty, value);
        }

        /// <summary>分隔条粗细。</summary>
        public double SplitterThickness
        {
            get => (double)GetValue(SplitterThicknessProperty);
            set => SetValue(SplitterThicknessProperty, value);
        }

        /// <summary>分隔条颜色。</summary>
        public Brush SplitterBrush
        {
            get => (Brush)GetValue(SplitterBrushProperty);
            set => SetValue(SplitterBrushProperty, value);
        }

        /// <summary>格子边框颜色。</summary>
        public Brush CellBorderBrush
        {
            get => (Brush)GetValue(CellBorderBrushProperty);
            set => SetValue(CellBorderBrushProperty, value);
        }

        /// <summary>格子标题栏背景。</summary>
        public Brush HeaderBackground
        {
            get => (Brush)GetValue(HeaderBackgroundProperty);
            set => SetValue(HeaderBackgroundProperty, value);
        }

        /// <summary>格子标题栏前景。</summary>
        public Brush HeaderForeground
        {
            get => (Brush)GetValue(HeaderForegroundProperty);
            set => SetValue(HeaderForegroundProperty, value);
        }

        /// <summary>设置根节点并刷新界面。</summary>
        public void SetRoot(LayoutNode root)
        {
            Root = root;
        }

        /// <summary>对目标格子执行分栏。</summary>
        public void Split(ContentNode target, SplitOrientation orientation)
        {
            if (target == null || Root == null)
            {
                return;
            }

            IContentFactory factory = Registry.All.Count > 0
                ? Registry.All[0]
                : FallbackFactory;

            ContentNode created = new ContentNode
            {
                ContentKey = factory.Key,
                Title = factory.DisplayName
            };

            Root = LayoutOperations.Split(Root, target, orientation, created);
            Refresh();
        }

        /// <summary>关闭目标格子。</summary>
        public void Close(ContentNode target)
        {
            if (target == null || Root == null)
            {
                return;
            }

            Root = LayoutOperations.Close(Root, target);
            Refresh();
        }

        /// <summary>切换目标格子的内容。</summary>
        public void ChangeContent(ContentNode target, string contentKey)
        {
            if (target == null)
            {
                return;
            }

            IContentFactory factory = Registry.Resolve(contentKey);
            target.ContentKey = contentKey;
            target.Title = factory != null ? factory.DisplayName : contentKey;
            Refresh();
        }

        /// <summary>按注册表内容恢复默认版面。</summary>
        public void ResetLayout()
        {
            Root = LayoutDefaults.Create(Registry);
            Refresh();
        }

        /// <summary>把当前版面序列化为 JSON。</summary>
        public string SaveLayout()
        {
            return Root == null ? string.Empty : LayoutSerializer.ToJson(Root);
        }

        /// <summary>从 JSON 载入版面。</summary>
        public void LoadLayout(string json)
        {
            Root = LayoutSerializer.FromJson(json);
            Refresh();
        }

        /// <summary>按当前布局树重建可视化树。</summary>
        public void Refresh()
        {
            Content = Root == null ? null : LayoutPanelBuilder.Build(Root, this);
            OnLayoutChanged();
        }

        private static Brush CreateFrozenBrush(byte r, byte g, byte b)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return brush;
        }

        private static void OnRootChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SplitLayoutControl)d).Refresh();
        }

        private static void OnLayoutJsonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitLayoutControl control = (SplitLayoutControl)d;
            string json = e.NewValue as string;
            if (string.IsNullOrWhiteSpace(json) || control._isApplyingJson)
            {
                return;
            }

            control._isApplyingJson = true;
            try
            {
                control.Root = LayoutSerializer.FromJson(json);
            }
            finally
            {
                control._isApplyingJson = false;
            }
        }

        private static void OnAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SplitLayoutControl)d).Refresh();
        }

        private void OnLayoutChanged()
        {
            if (_isApplyingJson)
            {
                return;
            }

            LayoutChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
