using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 布局树与 JSON 之间的序列化。
    /// </summary>
    public static class LayoutSerializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>把布局树转换为可序列化的数据对象。</summary>
        public static LayoutNodeData ToData(LayoutNode node)
        {
            SplitNode split = node as SplitNode;
            if (split != null)
            {
                return new LayoutNodeData
                {
                    NodeType = "Split",
                    Orientation = split.Orientation,
                    Ratio = split.Ratio,
                    First = split.First == null ? null : ToData(split.First),
                    Second = split.Second == null ? null : ToData(split.Second)
                };
            }

            ContentNode content = node as ContentNode;
            if (content != null)
            {
                return new LayoutNodeData
                {
                    NodeType = "Content",
                    ContentKey = content.ContentKey,
                    Title = content.Title
                };
            }

            throw new NotSupportedException($"未知的节点类型: {node.GetType().Name}");
        }

        /// <summary>把数据对象还原为布局树。</summary>
        public static LayoutNode FromData(LayoutNodeData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.NodeType == "Split")
            {
                SplitNode split = new SplitNode
                {
                    Orientation = data.Orientation,
                    Ratio = data.Ratio
                };

                if (data.First != null)
                {
                    split.First = FromData(data.First);
                }

                if (data.Second != null)
                {
                    split.Second = FromData(data.Second);
                }

                return split;
            }

            return new ContentNode
            {
                ContentKey = data.ContentKey,
                Title = data.Title
            };
        }

        /// <summary>序列化布局树为 JSON 文本。</summary>
        public static string ToJson(LayoutNode node)
        {
            return JsonSerializer.Serialize(ToData(node), Options);
        }

        /// <summary>从 JSON 文本反序列化布局树。</summary>
        public static LayoutNode FromJson(string json)
        {
            LayoutNodeData data = JsonSerializer.Deserialize<LayoutNodeData>(json, Options);
            if (data == null)
            {
                throw new InvalidOperationException("布局 JSON 内容为空。");
            }

            return FromData(data);
        }
    }
}
