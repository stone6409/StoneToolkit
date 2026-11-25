using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoneToolkit.WpfCommon.Misc
{
    public static class DataGridComands
    {
        private static object _synchronize = new object();
        private static RoutedUICommand _add;
        private static RoutedUICommand _insert;
        private static RoutedUICommand _edit;
        private static RoutedUICommand _delete;
        private static RoutedUICommand _import;

        public static RoutedUICommand Add
        {
            get { return DataGridComands.EnsureCommand(ref DataGridComands._add, "Add"); }
        }

        public static RoutedUICommand Insert
        {
            get { return DataGridComands.EnsureCommand(ref DataGridComands._insert, "Insert"); }
        }

        public static RoutedUICommand Edit
        {
            get { return DataGridComands.EnsureCommand(ref DataGridComands._edit, "Edit"); }
        }

        public static RoutedUICommand Delete
        {
            get { return DataGridComands.EnsureCommand(ref DataGridComands._delete, "Delete"); }
        }

        public static RoutedUICommand Import
        {
            get { return DataGridComands.EnsureCommand(ref DataGridComands._import, "Import"); }
        }

        private static RoutedUICommand EnsureCommand(ref RoutedUICommand command, string commandPropertyName)
        {
            lock (DataGridComands._synchronize)
            {
                if (command == null)
                {
                    command = new RoutedUICommand(commandPropertyName, commandPropertyName, typeof(DataGridComands));
                }
            }
            return command;
        }
    }
}
