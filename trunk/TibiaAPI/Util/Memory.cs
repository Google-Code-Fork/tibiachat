using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tibia
{
    /// <summary>
    /// Helper methods for reading memory.
    /// </summary>
    public static class Memory
    {
        public static System.Text.ASCIIEncoding encoding = null;

        /// <summary>
        /// Read a specified number of bytes from a process.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="bytesToRead"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(IntPtr handle, long address, uint bytesToRead)
        {
            IntPtr ptrBytesRead;
            byte[] buffer = new byte[bytesToRead];

            Util.WinApi.ReadProcessMemory(handle, new IntPtr(address), buffer, bytesToRead, out ptrBytesRead);

            return buffer;
        }

        /// <summary>
        /// Read a byte from memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte ReadByte(IntPtr handle, long address)
        {
            return ReadBytes(handle, address, 1)[0];
        }

        /// <summary>
        /// Read a short from memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static short ReadShort(IntPtr handle, long address)
        {
            return BitConverter.ToInt16(ReadBytes(handle, address, 2), 0);
        }

        /// <summary>
        /// Read an integer from the process (actually a short because it is only 4 bytes)
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static int ReadInt(IntPtr handle, long address)
        {
            return BitConverter.ToInt32(ReadBytes(handle, address, 4), 0);
        }

        /// <summary>
        /// Read a 32-bit double from the process
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static double ReadDouble(IntPtr handle, long address)
        {
            return BitConverter.ToDouble(ReadBytes(handle, address, 8), 0);
        }

        /// <summary>
        /// Read a string from memmory. Splits at 00 and returns first section to avoid junk. Uses default length of 255. Use ReadString(IntPtr handle, long address, int length) to read longer strings, such as the RSA key.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string ReadString(IntPtr handle, long address)
        {
            return ReadString(handle, address, 255);
        }

        /// <summary>
        /// Read a string from memmory. Splits at 00 and returns first section to avoid junk.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="length">the length of the bytes to read</param>
        /// <returns></returns>
        public static string ReadString(IntPtr handle, long address, uint length)
        {
            if (encoding == null) encoding = new System.Text.ASCIIEncoding();
            String str = encoding.GetString(ReadBytes(handle, address, length)).Split(new Char())[0];
            return str;
        }

        /// <summary>
        /// Write a specified number of bytes to a process.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool WriteBytes(IntPtr handle, long address, byte[] bytes, uint length)
        {
            IntPtr bytesWritten;
            int result;

            // Write to memory
            result = Util.WinApi.WriteProcessMemory(handle, new IntPtr(address), bytes, length, out bytesWritten);

            return (result != 0);
        }

        /// <summary>
        /// Write an integer to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteInt(IntPtr handle, long address, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes, 4);
        }

        /// <summary>
        /// Write a double value to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteDouble(IntPtr handle, long address, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes, 8);
        }

        /// <summary>
        /// Write a byte to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteByte(IntPtr handle, long address, byte value)
        {
            byte[] bytes = { value };
            return WriteBytes(handle, address, bytes, 1);
        }

        /// <summary>
        /// Write a string to memory without using econding.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool WriteStringNoEncoding(IntPtr handle, long address, string str)
        {
            str += '\0';
            byte[] bytes = str.ToByteArray();
            return WriteBytes(handle, address, bytes, (uint)bytes.Length);
        }

        /// <summary>
        /// Write a string to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool WriteString(IntPtr handle, long address, string str)
        {
            str += '\0';
            if (encoding == null) encoding = new System.Text.ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(str);
            return WriteBytes(handle, address, bytes, (uint)bytes.Length);
        }

        /// <summary>
        /// Set the RSA key. Different from WriteString because must overcome protection.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="newKey"></param>
        /// <returns></returns>
        public static bool WriteRSA(IntPtr handle, long address, string newKey)
        {
            IntPtr bytesWritten;
            int result;
            uint oldProtection = 0;

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] bytes = enc.GetBytes(newKey);

            // Make it so we can write to the memory block
            Util.WinApi.VirtualProtectEx(handle, new IntPtr(address), new IntPtr(bytes.Length), Util.WinApi.PAGE_EXECUTE_READWRITE, ref oldProtection);

            // Write to memory
            result = Util.WinApi.WriteProcessMemory(handle, new IntPtr(address), bytes, (uint)bytes.Length, out bytesWritten);

            // Put the protection back on the memory block
            Util.WinApi.VirtualProtectEx(handle, new IntPtr(address), new IntPtr(bytes.Length), oldProtection, ref oldProtection);

            return (result != 0);
        }
    }
}
