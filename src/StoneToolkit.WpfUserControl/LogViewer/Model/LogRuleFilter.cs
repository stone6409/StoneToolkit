using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoneToolkit.WpfUserControl.LogViewer.Model
{
    public class LogRuleFilter : ICloneable
    {
        private bool _enabled;

        public string _expression;

        public string _description;

        public event EventHandler<EventArgs> EnabledChanged;

        public LogRuleFilter()
        {

        }

        public LogRuleFilter(string name, bool enabled, string expression)
        {
            Name = name;
            Enabled = enabled;
            Expression = expression;
        }

        public string Name { get; set; }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    if (EnabledChanged != null)
                        EnabledChanged(this, null);
                }
            }
        }

        public string Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                _expression = value;
                _regularExpression = null;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        Regex _regularExpression;

        private Regex RegularExpression
        {
            get
            {
                if (_regularExpression == null)
                {
                    if (!string.IsNullOrEmpty(Expression))
                    {
                        _regularExpression = new Regex(Expression);
                    }
                }

                return _regularExpression;
            }
        }

        public bool? IsMatch(string message)
        {
            if (Enabled && RegularExpression != null)
            {
                return RegularExpression.IsMatch(message);
            }

            return null;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
