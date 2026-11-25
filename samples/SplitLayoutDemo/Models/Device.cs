namespace SplitLayoutDemo.Models
{
    /// <summary>
    /// 设备状态项。
    /// </summary>
    public class Device
    {
        public string Name { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Runtime { get; set; } = string.Empty;

        public int Output { get; set; }
    }
}
