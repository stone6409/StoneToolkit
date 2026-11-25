using System;
using System.Collections.Generic;

namespace CommonLibrary.ByteUtils
{
    public static class ObjectValueConverter
    {
        private delegate string ConvertToStringMethod(object value, out string valueType);

        private static Dictionary<Type, ConvertToStringMethod> _convertToStringMethod;

        private static Dictionary<Type, ConvertToStringMethod> ConvertToStringMethods
        {
            get
            {
                if (_convertToStringMethod == null)
                {
                    _convertToStringMethod = new Dictionary<Type, ConvertToStringMethod>
                    {
                        { typeof(bool), ConvertBoolToString },
                        { typeof(byte), ConvertByteToString },
                        { typeof(sbyte), ConvertSByteToString },
                        { typeof(Int16), ConvertInt16ToString },
                        { typeof(UInt16), ConvertUInt16ToString },
                        { typeof(Int32), ConvertInt32ToString },
                        { typeof(UInt32), ConvertUInt32ToString },
                        { typeof(float), ConvertFloatToString },
                        { typeof(Int64), ConvertInt64ToString },
                        { typeof(UInt64), ConvertUInt64ToString },
                        { typeof(double), ConvertDoubleToString },
                        { typeof(string), ConvertStringToString },
                        { typeof(DateTime), ConvertDateTimeToString },
                    };
                }

                return _convertToStringMethod;
            }
        }

        private delegate object ConvertToObjectMethod(string value);

        private static Dictionary<string, ConvertToObjectMethod> _convertToObjectMethods;

        private static Dictionary<string, ConvertToObjectMethod> ConvertToObjectMethods
        {
            get
            {
                if (_convertToObjectMethods == null)
                {
                    _convertToObjectMethods = new Dictionary<string, ConvertToObjectMethod>
                    {
                        { "bool", ConvertStringToBool },
                        { "byte", ConvertStringToByte },
                        { "sbyte", ConvertStringToSByte },
                        { "int16", ConvertStringToInt16 },
                        { "uint16", ConvertStringToUInt16 },
                        { "int32", ConvertStringToInt32 },
                        { "uint32", ConvertStringToUInt32 },
                        { "float", ConvertStringToFloat },
                        { "int64", ConvertStringToInt64 },
                        { "uint64", ConvertStringToUInt64 },
                        { "double", ConvertStringToDouble },
                        { "string", ConvertStringToString },
                        { "datetime", ConvertStringToDateTime },
                    };
                }

                return _convertToObjectMethods;
            }
        }

        public static string ConvertToString(object value, out string valueType)
        {
            if (value == null)
            {
                valueType = "null";
                return null;
            }

            Type type = value.GetType();

            ConvertToStringMethod convertToStringMethod;
            if (ConvertToStringMethods.TryGetValue(type, out convertToStringMethod))
            {
                return convertToStringMethod(value, out valueType);
            }

            throw new NotImplementedException();
        }

        public static object ConvertToObject(string value, string valueType)
        {
            if (value == null)
            {
                return null;
            }

            ConvertToObjectMethod convertToObjectMethod;
            if (ConvertToObjectMethods.TryGetValue(valueType, out convertToObjectMethod))
            {
                return convertToObjectMethod(value);
            }

            throw new NotImplementedException();
        }

        public static string ConvertBoolToString(object value, out string valueType)
        {
            valueType = "bool";
            return Convert.ToString(value);
        }

        public static object ConvertStringToBool(string value)
        {
            return bool.Parse(value);
        }

        public static string ConvertByteToString(object value, out string valueType)
        {
            valueType = "byte";
            return Convert.ToString(value);
        }

        public static object ConvertStringToByte(string value)
        {
            return byte.Parse(value);
        }

        public static string ConvertSByteToString(object value, out string valueType)
        {
            valueType = "sbyte";
            return Convert.ToString(value);
        }

        public static object ConvertStringToSByte(string value)
        {
            return sbyte.Parse(value);
        }

        public static string ConvertInt16ToString(object value, out string valueType)
        {
            valueType = "int16";
            return Convert.ToString(value);
        }

        public static object ConvertStringToInt16(string value)
        {
            return Int16.Parse(value);
        }

        public static string ConvertUInt16ToString(object value, out string valueType)
        {
            valueType = "uint16";
            return Convert.ToString(value);
        }

        public static object ConvertStringToUInt16(string value)
        {
            return UInt16.Parse(value);
        }

        public static string ConvertInt32ToString(object value, out string valueType)
        {
            valueType = "int32";
            return Convert.ToString(value);
        }

        public static object ConvertStringToInt32(string value)
        {
            return Int32.Parse(value);
        }

        public static string ConvertUInt32ToString(object value, out string valueType)
        {
            valueType = "uint32";
            return Convert.ToString(value);
        }

        public static object ConvertStringToUInt32(string value)
        {
            return UInt32.Parse(value);
        }

        public static string ConvertFloatToString(object value, out string valueType)
        {
            valueType = "float";
            return Convert.ToString(value);
        }

        public static object ConvertStringToFloat(string value)
        {
            return float.Parse(value);
        }

        public static string ConvertInt64ToString(object value, out string valueType)
        {
            valueType = "int64";
            return Convert.ToString(value);
        }

        public static object ConvertStringToInt64(string value)
        {
            return Int64.Parse(value);
        }

        public static string ConvertUInt64ToString(object value, out string valueType)
        {
            valueType = "uint64";
            return Convert.ToString(value);
        }

        public static object ConvertStringToUInt64(string value)
        {
            return UInt64.Parse(value);
        }

        public static string ConvertDoubleToString(object value, out string valueType)
        {
            valueType = "double";
            return Convert.ToString(value);
        }

        public static object ConvertStringToDouble(string value)
        {
            return double.Parse(value);
        }

        public static string ConvertStringToString(object value, out string valueType)
        {
            valueType = "string";
            return Convert.ToString(value);
        }

        public static object ConvertStringToString(string value)
        {
            return value;
        }

        public static string ConvertDateTimeToString(object value, out string valueType)
        {
            valueType = "datetime";
            return Convert.ToString(value);
        }

        public static object ConvertStringToDateTime(string value)
        {
            return DateTime.Parse(value);
        }
    }
}
