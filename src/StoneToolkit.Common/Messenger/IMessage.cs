namespace StoneToolkit.Common.Messenger
{
    /// <summary>
    /// 消息标记接口。
    /// 所有通过 Messenger 传递的消息类型建议实现该接口，作为一种约定，
    /// 以明确区分"消息"与普通对象，提升可读性。
    /// </summary>
    public interface IMessage
    {
    }
}
