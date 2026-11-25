/// <summary>
/// 打开操作结果
/// </summary>
namespace StoneToolkit.Extension.Utilities
{
    public class OpenResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? ExitCode { get; set; }
        public string StandardOutput { get; set; }
        public string StandardError { get; set; }
    }
}