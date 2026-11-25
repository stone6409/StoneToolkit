using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using SplitLayoutDemo.Contents;
using StoneToolkit.WpfControl.SplitLayoutControl;

namespace SplitLayoutDemo
{
    /// <summary>
    /// 主窗口视图模型：装配内容工厂、承载版面宿主并处理保存 / 加载 / 重置。
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly SplitLayoutControl _layout;
        private LayoutNode _layoutRoot = null!;
        private string _layoutJson = string.Empty;
        private string _status = string.Empty;

        public MainWindowViewModel(SplitLayoutControl layout)
        {
            _layout = layout;

            layout.Registry.Register(new DeviceListContentFactory());
            layout.Registry.Register(new TrendChartContentFactory());
            layout.Registry.Register(new LoadCurveContentFactory());
            layout.Registry.Register(new AlarmListContentFactory());
            layout.Registry.Register(new EventLogContentFactory());

            layout.LayoutChanged += OnLayoutChanged;

            SaveCommand = new RelayCommand(SaveLayout);
            LoadCommand = new RelayCommand(LoadLayout);
            ResetCommand = new RelayCommand(ResetLayout);

            ResetLayout();
        }

        public ICommand SaveCommand { get; }

        public ICommand LoadCommand { get; }

        public ICommand ResetCommand { get; }

        /// <summary>版面根节点，双向绑定到控件。</summary>
        public LayoutNode LayoutRoot
        {
            get => _layoutRoot;
            set
            {
                if (ReferenceEquals(_layoutRoot, value))
                {
                    return;
                }

                _layoutRoot = value;
                OnPropertyChanged();
            }
        }

        /// <summary>当前版面的 JSON 文本，仅用于展示。</summary>
        public string LayoutJson
        {
            get => _layoutJson;
            set
            {
                if (_layoutJson == value)
                {
                    return;
                }

                _layoutJson = value;
                OnPropertyChanged();
            }
        }

        /// <summary>状态栏文字。</summary>
        public string Status
        {
            get => _status;
            set
            {
                if (_status == value)
                {
                    return;
                }

                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnLayoutChanged(object? sender, EventArgs e)
        {
            LayoutRoot = _layout.Root;
            LayoutJson = _layout.SaveLayout();
        }

        private void SaveLayout()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "布局文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                FileName = "layout.json"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            File.WriteAllText(dialog.FileName, _layout.SaveLayout());
            Status = $"版面已保存：{dialog.FileName}";
        }

        private void LoadLayout()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "布局文件 (*.json)|*.json|所有文件 (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            _layout.LoadLayout(File.ReadAllText(dialog.FileName));
            Status = $"版面已加载：{dialog.FileName}";
        }

        private void ResetLayout()
        {
            LayoutRoot = LayoutDefaults.Create(_layout.Registry);
            LayoutJson = _layout.SaveLayout();
            Status = "右键任意格子可进行分栏、关闭或切换内容。";
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
