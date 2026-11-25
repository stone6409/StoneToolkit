using System;

namespace StoneToolkit.WpfControl.SplitLayoutControl
{
    /// <summary>
    /// 分割容器节点，把区域一分为二。
    /// </summary>
    public class SplitNode : LayoutNode
    {
        private SplitOrientation _orientation;
        private double _ratio = 0.5;
        private LayoutNode _first;
        private LayoutNode _second;

        /// <summary>
        /// 分割方向。<see cref="SplitOrientation.Horizontal"/> 为左右分栏；<see cref="SplitOrientation.Vertical"/> 为上下分栏。
        /// </summary>
        public SplitOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation == value)
                {
                    return;
                }

                _orientation = value;
                OnPropertyChanged();
            }
        }

        /// <summary>第一子节点（左 / 上）所占比例，取值 (0,1)。</summary>
        public double Ratio
        {
            get => _ratio;
            set
            {
                double clamped = value < 0.05 ? 0.05 : value > 0.95 ? 0.95 : value;
                if (Math.Abs(_ratio - clamped) < 0.0001)
                {
                    return;
                }

                _ratio = clamped;
                OnPropertyChanged();
            }
        }

        /// <summary>左 / 上子节点。</summary>
        public LayoutNode First
        {
            get => _first;
            set
            {
                if (ReferenceEquals(_first, value))
                {
                    return;
                }

                _first = value;
                if (value != null)
                {
                    value.Parent = this;
                }

                OnPropertyChanged();
            }
        }

        /// <summary>右 / 下子节点。</summary>
        public LayoutNode Second
        {
            get => _second;
            set
            {
                if (ReferenceEquals(_second, value))
                {
                    return;
                }

                _second = value;
                if (value != null)
                {
                    value.Parent = this;
                }

                OnPropertyChanged();
            }
        }
    }
}
