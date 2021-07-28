using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Evolution
{
    public class Program
    {
        public const int Width = 100;
        public const int Height = 100;

        #region Lowlevel
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }
        #endregion

        static void Main(string[] args)
        {
            SafeFileHandle h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            SmallRect rect = new SmallRect { Left = 0, Top = 0, Right = Width, Bottom = Height };

            if (!h.IsInvalid)
            {
                using var storageController = new StorageController();
                while (true)
                {
                    var processor = new FieldProcessor(Width, Height, storageController);
                    bool nextStep = true;
                    while (nextStep)
                    {
                        var field = processor.Field;

                        CharInfo[] buf = new CharInfo[Width * Height];

                        for (int i = 0; i < field.Length; i++)
                        {
                            switch (field[i])
                            {
                                case 1:
                                    buf[i].Char.AsciiChar = 254;
                                    buf[i].Attributes = 2;
                                    break;
                                case 2:
                                    buf[i].Char.AsciiChar = 215;
                                    buf[i].Attributes = 3;
                                    break;
                                default:
                                    buf[i].Char.AsciiChar = 46;
                                    buf[i].Attributes = 5;
                                    break;
                            }
                        }

                        WriteConsoleOutput(h, buf,
                            new Coord(Width, Height),
                            new Coord(0, 0),
                            ref rect);
                        nextStep = processor.Step();
                    }

                }
            }
        }
    }
}
