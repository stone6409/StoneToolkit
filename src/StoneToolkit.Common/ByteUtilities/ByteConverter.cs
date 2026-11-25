using System;

namespace CommonLibrary.ByteUtils
{
    public class ByteConverter
    {
        public static byte[] ToArray(byte[] bytes, int index, int length, bool isLittleEndian = true)
        {
            if (index < 0 || index >= bytes.Length || length < 0 || index + length > bytes.Length)
            {
                throw new ArgumentException();
            }

            byte[] resultBytes = new byte[length];
            Array.Copy(bytes, index, resultBytes, 0, length);

            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                Array.Reverse(resultBytes);
            }

            return resultBytes;
        }

        public static void SwapBytes(byte[] bytes, int index, int length, bool isLittleEndian = true)
        {
            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                byte[] result = new byte[length];
                Array.Copy(bytes, index, result, 0, length);

                Array.Reverse(result);
                Array.Copy(result, 0, bytes, index, length);
            }
        }

        public static string ToString(byte[] bytes)
        {
            string result = null;

            if (bytes.Length > 0)
            {
                result = "0x" + BitConverter.ToString(bytes, 0).Replace("-", ", 0x");
            }

            return result;
        }
    }
}
