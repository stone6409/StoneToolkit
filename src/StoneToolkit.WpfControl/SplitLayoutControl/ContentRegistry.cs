using System.Collections.Generic;

namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 内容工厂注册表。右键菜单据此生成可选项。
    /// </summary>
    public class ContentRegistry
    {
        private readonly Dictionary<string, IContentFactory> _factories
            = new Dictionary<string, IContentFactory>();

        private readonly List<IContentFactory> _ordered = new List<IContentFactory>();

        /// <summary>按注册顺序返回全部工厂。</summary>
        public IReadOnlyList<IContentFactory> All => _ordered;

        /// <summary>注册工厂；相同 Key 时覆盖原工厂。</summary>
        public void Register(IContentFactory factory)
        {
            if (factory == null)
            {
                return;
            }

            IContentFactory existing;
            if (_factories.TryGetValue(factory.Key, out existing))
            {
                int index = _ordered.IndexOf(existing);
                _ordered[index] = factory;
                _factories[factory.Key] = factory;
            }
            else
            {
                _factories.Add(factory.Key, factory);
                _ordered.Add(factory);
            }
        }

        /// <summary>按 Key 查找工厂，未找到返回 null。</summary>
        public IContentFactory Resolve(string key)
        {
            IContentFactory factory;
            if (key != null && _factories.TryGetValue(key, out factory))
            {
                return factory;
            }

            return null;
        }

        /// <summary>清空注册表。</summary>
        public void Clear()
        {
            _factories.Clear();
            _ordered.Clear();
        }
    }
}
