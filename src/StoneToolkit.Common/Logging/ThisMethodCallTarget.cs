using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StoneToolkit.Common.Logging
{
    [Target("ThisMethodCall")]
    public sealed class ThisMethodCallTarget : MethodCallTargetBase
    {
        /// <summary>
        /// Gets or sets the class name.
        /// </summary>
        /// <docgen category='Invocation Options' order='10' />
        public Object ObjectInstance { get; set; }

        /// <summary>
        /// Gets or sets the method name. The method must be public and static.
        /// </summary>
        /// <docgen category='Invocation Options' order='10' />
        public string MethodName { get; set; }

        private MethodInfo Method { get; set; }

        /// <summary>
        /// Initializes the target.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (this.ObjectInstance != null && this.MethodName != null)
            {
                Type targetType = this.ObjectInstance.GetType();
                this.Method = targetType.GetMethod(this.MethodName);
            }
            else
            {
                this.Method = null;
            }
        }

        /// <summary>
        /// Calls the specified Method.
        /// </summary>
        /// <param name="parameters">Method parameters.</param>
        protected override void DoInvoke(object[] parameters)
        {
            if (this.Method != null)
            {
                this.Method.Invoke(ObjectInstance, parameters);
            }
        }
    }
}
