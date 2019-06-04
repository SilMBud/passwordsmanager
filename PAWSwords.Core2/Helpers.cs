using System;
using System.Runtime.InteropServices;
using System.Security;

namespace PAWSwords.Core
{
    public static class Helpers
    {
        public unsafe static void ClearData(byte[] data)
        {
            byte[] clear = new byte[data.Length];
            fixed (byte* ptr = &data[0])
            {
                Marshal.Copy(clear, 0, new IntPtr(ptr), data.Length);
            }
        }

        public static string GetStringFromSecureString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocAnsi(value);
                return Marshal.PtrToStringAnsi(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(valuePtr);
            }
        }
    }
}

