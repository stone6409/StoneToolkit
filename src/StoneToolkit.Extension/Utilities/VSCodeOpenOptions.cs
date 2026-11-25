/// <summary>
/// VSCode 打开配置选项
/// </summary>
namespace StoneToolkit.Extension.Utilities
{
    /// <summary>
    /// VSCode 打开配置选项
    /// </summary>
    public class VSCodeOpenOptions
    {
        public bool OpenInNewWindow { get; set; } = false;
        public bool ReuseWindow { get; set; } = false;
        public bool SkipAddToRecentlyOpened { get; set; } = true;
        public bool DisableExtensions { get; set; } = false;
        public bool DisableGpu { get; set; } = false;
        public bool RunAsAdministrator { get; set; } = false;
        public bool UseTemporaryUserData { get; set; } = false;
        public bool DisableWorkspaceTrust { get; set; } = true; // 新增：禁用工作区信任
        public int? GotoLine { get; set; }
        public int? GotoColumn { get; set; }
        public int TimeoutMilliseconds { get; set; } = 5000;
        public string CustomArguments { get; set; } = string.Empty;
        public bool DisableWelcomePage { get; set; } = true;

        public static VSCodeOpenOptions Default
        {
            get { return new VSCodeOpenOptions(); }
        }

        public static VSCodeOpenOptions SaveToRecent
        {
            get { return new VSCodeOpenOptions { SkipAddToRecentlyOpened = false }; }
        }

        public static VSCodeOpenOptions PerformanceOptimized
        {
            get { return new VSCodeOpenOptions { DisableExtensions = true, DisableGpu = true }; }
        }

        // 修改：完全隔离的选项
        public static VSCodeOpenOptions Isolated
        {
            get
            {
                return new VSCodeOpenOptions
                {
                    SkipAddToRecentlyOpened = true,
                    UseTemporaryUserData = true,
                    DisableExtensions = true,
                    DisableWorkspaceTrust = true  // 默认禁用信任检查
                };
            }
        }
    }
}