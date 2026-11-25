using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoneToolkit.WpfUserControl.ToolBox
{
    /// <summary>
    /// Interaction logic for ToolBoxControl.xaml
    /// </summary>
    public partial class ToolBoxControl : UserControl
    {
        public event EventHandler<ToolItemEventArgs> ToolItemSelected;

        public ToolBoxControl()
        {
            InitializeComponent();
        }

        private ObservableCollection<ToolItem> _toolItems;

        private ObservableCollection<ToolItem> ToolItems
        {
            get
            {
                if (_toolItems == null)
                {
                    _toolItems = new ObservableCollection<ToolItem>();
                }

                return _toolItems;
            }
        }

        public void AddToolItem(ToolItem toolItem)
        {
            ToolItems.Add(toolItem);
            toolItem.PropertyChanged += ToolItem_PropertyChanged;
        }

        public ToolItem FindToolItem(string name)
        {
            foreach (ToolItem toolItem in ToolItems)
            {
                if (toolItem.Name == name)
                {
                    return toolItem;
                }
            }

            return null;
        }

        private void ToolItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ToolItem toolItem = sender as ToolItem;

            if (e.PropertyName == "IsSelected")
            {
                if (SelectedToolItem != toolItem)
                {
                    if (SelectedToolItem != null)
                    {
                        SelectedToolItem.IsSelected = false;
                    }

                    SelectedToolItem = toolItem;
                }
            }
        }

        public ToolItem SelectionTool { get; set; }

        public ToolItem SelectedToolItem { get; set; }

        private bool _hasLoaded = false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_hasLoaded)
            {
                return;
            }
            _hasLoaded = true;

            itemsControl.ItemsSource = ToolItems;
            AddGrouping();
        }

        private void AddGrouping()
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(itemsControl.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Category");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            ToolItem toolItem = frameworkElement.DataContext as ToolItem;

            toolItem.IsSelected = true;

            if (ToolItemSelected != null)
            {
                ToolItemSelected(this, new ToolItemEventArgs(toolItem));
            }
        }
    }
}
