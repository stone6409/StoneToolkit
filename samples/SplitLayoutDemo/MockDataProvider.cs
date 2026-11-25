using System;
using System.Collections.Generic;
using SplitLayoutDemo.Models;

namespace SplitLayoutDemo
{
    /// <summary>
    /// 演示用模拟数据。数据在首次访问时生成并缓存，保证每次重建视图时内容一致。
    /// </summary>
    public static class MockDataProvider
    {
        private static IReadOnlyList<Device>? _devices;
        private static IReadOnlyList<AlarmItem>? _alarms;
        private static IReadOnlyList<LogItem>? _logs;
        private static IReadOnlyList<double>? _trend;
        private static IReadOnlyList<double>? _load;

        public static IReadOnlyList<Device> CreateDevices()
        {
            return _devices ??= BuildDevices();
        }

        public static IReadOnlyList<AlarmItem> CreateAlarms()
        {
            return _alarms ??= BuildAlarms();
        }

        public static IReadOnlyList<LogItem> CreateLogs()
        {
            return _logs ??= BuildLogs();
        }

        public static IReadOnlyList<double> CreateTrendSamples()
        {
            return _trend ??= BuildWalk(120, 75.0, 3.0, 7);
        }

        public static IReadOnlyList<double> CreateLoadSamples()
        {
            return _load ??= BuildWalk(240, 45.0, 2.2, 13);
        }

        private static IReadOnlyList<Device> BuildDevices()
        {
            Random random = new Random(2024);
            string[] names =
            {
                "1#注塑机", "2#注塑机", "3#数控机床", "4#装配线", "5#焊接机器人",
                "6#喷涂线", "7#包装机", "8#检测台", "9#传送带", "10#空压机"
            };
            string[] states = { "运行", "运行", "待机", "故障", "运行", "停机", "运行", "待机", "运行", "运行" };

            List<Device> list = new List<Device>();
            for (int i = 0; i < names.Length; i++)
            {
                list.Add(new Device
                {
                    Name = names[i],
                    State = states[i],
                    Runtime = $"{random.Next(1, 72):00}:{random.Next(0, 60):00}:{random.Next(0, 60):00}",
                    Output = random.Next(100, 9999)
                });
            }

            return list;
        }

        private static IReadOnlyList<AlarmItem> BuildAlarms()
        {
            Random random = new Random(21);
            string[] levels = { "故障", "警告", "提示", "警告", "提示", "故障" };
            string[] messages =
            {
                "4#装配线 电机过载",
                "2#注塑机 料筒温度偏高",
                "6#喷涂线 气压不足",
                "5#焊接机器人 关节限位",
                "8#检测台 视觉相机离线",
                "7#包装机 膜料用尽"
            };

            List<AlarmItem> list = new List<AlarmItem>();
            for (int i = 0; i < messages.Length; i++)
            {
                list.Add(new AlarmItem
                {
                    Time = $"{8 + i / 6:00}:{i * 7 % 60:00}:{random.Next(0, 60):00}",
                    Level = levels[i],
                    Message = messages[i]
                });
            }

            return list;
        }

        private static IReadOnlyList<LogItem> BuildLogs()
        {
            string[] messages =
            {
                "系统启动，进入自动运行模式",
                "1#注塑机 开始加工批次 B20240916",
                "2#注塑机 料筒温度到达设定值",
                "4#装配线 完成工单 WO-10231",
                "5#焊接机器人 程序切换至 P-07",
                "6#喷涂线 气压恢复至 0.62 MPa",
                "8#检测台 相机重新连接成功",
                "系统已连续运行 12 小时"
            };

            List<LogItem> list = new List<LogItem>();
            for (int i = 0; i < messages.Length; i++)
            {
                list.Add(new LogItem
                {
                    Time = $"{9 + i / 4:00}:{i * 11 % 60:00}",
                    Message = messages[i]
                });
            }

            return list;
        }

        private static IReadOnlyList<double> BuildWalk(int count, double start, double volatility, int seed)
        {
            Random random = new Random(seed);
            List<double> list = new List<double>(count);
            double value = start;

            for (int i = 0; i < count; i++)
            {
                value += (random.NextDouble() - 0.48) * volatility;
                list.Add(Math.Round(value, 2));
            }

            return list;
        }
    }
}
