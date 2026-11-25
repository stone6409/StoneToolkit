namespace SplitLayoutDemo.Models
{
    /// <summary>
    /// 报警条目。
    /// </summary>
    public class AlarmItem
    {
        public string Time { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
