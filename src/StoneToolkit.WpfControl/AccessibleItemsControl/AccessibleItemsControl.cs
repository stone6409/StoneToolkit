using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace StoneToolkit.WpfControl
{
    public sealed class AccessibleItemsControl : ItemsControl
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AccessibleItemsControlAutomationPeer(this);
        }

        private sealed class AccessibleItemsControlAutomationPeer : ItemsControlAutomationPeer
        {
            public AccessibleItemsControlAutomationPeer(ItemsControl owner)
                : base(owner)
            {
            }

            protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
            {
                return new AccessibleItemAutomationPeer(item, this);
            }

            protected override string GetClassNameCore() => "AccessibleItemsControl";

            protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.List;
        }

        private sealed class AccessibleItemAutomationPeer : ItemAutomationPeer
        {
            public AccessibleItemAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer)
                : base(item, itemsControlAutomationPeer)
            {
            }

            protected override string GetClassNameCore() => "AccessibleItemsControlItem";

            protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.DataItem;
        }
    }
}
