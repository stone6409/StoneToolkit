using System;

namespace StoneToolkit.Common.Utilities
{
    /// <summary>
    /// 唯一ID生成器
    /// </summary>
    public static class UniqueIdGenerator
    {
        /// <summary>
        /// 短ID（时间戳hex + GUID前8位）
        /// <para>长度约19字符，按时间有序，适合文件名、聊天会话ID等一般场景</para>
        /// </summary>
        public static class ShortId
        {
            public static string New()
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("x");
                var guid = Guid.NewGuid().ToString("N").Substring(0, 8);
                return $"{timestamp}{guid}";
            }
        }

        /// <summary>
        /// 雪花数字ID（64位整数）
        /// <para>长度约19位数字，趋势递增，适合数据库主键、订单号等需要排序的场景</para>
        /// </summary>
        public static class Snowflake
        {
            public static string New()
            {
                return Engine.NextId().ToString();
            }
        }

        /// <summary>
        /// 紧凑ID（雪花ID的Base62编码）
        /// <para>长度约11字符，最短，适合URL参数、短链接、Token等对长度敏感的场景</para>
        /// </summary>
        public static class Compact
        {
            public static string New()
            {
                return ToBase62(Engine.NextId());
            }
        }

        #region 内部共享引擎

        private static class Engine
        {
            private const long Epoch = 1700000000000L;
            private const int WorkerIdBits = 10;
            private const int SequenceBits = 12;

            private const long MaxWorkerId = (1L << WorkerIdBits) - 1;
            private const long MaxSequence = (1L << SequenceBits) - 1;

            private const int WorkerIdShift = SequenceBits;
            private const int TimestampShift = SequenceBits + WorkerIdBits;

            private static readonly long _workerId;
            private static long _lastTimestamp = -1L;
            private static long _sequence = 0L;
            private static readonly object _lock = new object();

            static Engine()
            {
                int hash = Environment.MachineName.GetHashCode();
                _workerId = (hash & long.MaxValue) % (MaxWorkerId + 1);
            }

            public static long NextId()
            {
                lock (_lock)
                {
                    var timestamp = GetCurrentTimestamp();

                    if (timestamp < _lastTimestamp)
                        throw new InvalidOperationException("时钟回拨，无法生成唯一ID");

                    if (timestamp == _lastTimestamp)
                    {
                        _sequence = (_sequence + 1) & MaxSequence;
                        if (_sequence == 0)
                            timestamp = WaitNextMillis(_lastTimestamp);
                    }
                    else
                    {
                        _sequence = 0L;
                    }

                    _lastTimestamp = timestamp;

                    return (timestamp << TimestampShift)
                         | (_workerId << WorkerIdShift)
                         | _sequence;
                }
            }

            private static long GetCurrentTimestamp()
            {
                return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Epoch;
            }

            private static long WaitNextMillis(long lastTimestamp)
            {
                var timestamp = GetCurrentTimestamp();
                while (timestamp <= lastTimestamp)
                    timestamp = GetCurrentTimestamp();
                return timestamp;
            }
        }

        private static string ToBase62(long value)
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (value == 0) return "0";

            var buffer = new char[11];
            int pos = buffer.Length;

            var num = value < 0 ? (value ^ long.MinValue) : value;

            while (num > 0)
            {
                buffer[--pos] = chars[(int)(num % 62)];
                num /= 62;
            }

            return new string(buffer, pos, buffer.Length - pos);
        }

        #endregion
    }
}
