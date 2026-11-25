using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Threading;

namespace StoneToolkit.WpfCustomControl.Button
{
    public class ButtonAutomationPeer : ButtonBaseAutomationPeer, IInvokeProvider
    {
        public ButtonAutomationPeer(Button owner) : base(owner)
        { 
        }

        override protected string GetClassNameCore()
        {
            return "Button";
        }

        override protected AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        /// 
        override public object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }
            else
            {
                return base.GetPattern(patternInterface);
            }
        }

        void IInvokeProvider.Invoke()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            // Async call of click event
            // In ClickHandler opens a dialog and suspend the execution we don't want to block this thread
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate (object param)
            {
                ((Button)Owner).AutomationButtonBaseClick();
                return null;
            }), null);
        }
    }
}
