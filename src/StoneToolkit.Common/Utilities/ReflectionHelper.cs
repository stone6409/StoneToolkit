using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace StoneToolkit.Common.Utilities
{
    public static class ReflectionHelper
    {
        internal static string? GetPropertyOrFieldName(MemberExpression expression)
        {
            string? propertyOrFieldName;
            if (!TryGetPropertyOrFieldName(expression, out propertyOrFieldName))
            {
                throw new InvalidOperationException("Unable to retrieve the property or field name.");
            }

            return propertyOrFieldName;
        }

        public static string? GetPropertyOrFieldName<TMember>(Expression<Func<TMember>> expression)
        {
            string? propertyOrFieldName;
            if (!TryGetPropertyOrFieldName(expression, out propertyOrFieldName))
                throw new InvalidOperationException("Unable to retrieve the property or field name.");

            return propertyOrFieldName;
        }

        [DebuggerStepThrough]
        internal static bool TryGetPropertyOrFieldName<TMember>(Expression<Func<TMember>> expression, out string? propertyOrFieldName)
        {
            propertyOrFieldName = null;

            if (expression == null)
                return false;

            return TryGetPropertyOrFieldName(expression.Body as MemberExpression, out propertyOrFieldName);
        }

        [DebuggerStepThrough]
        internal static bool TryGetPropertyOrFieldName(MemberExpression? expression, out string? propertyOrFieldName)
        {
            propertyOrFieldName = null;

            if (expression == null)
                return false;

            propertyOrFieldName = expression.Member.Name;

            return true;
        }

        public static bool IsPublicInstanceProperty(Type type, string propertyName)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public;
            return type.GetProperty(propertyName, flags) != null;
        }
    }
}
