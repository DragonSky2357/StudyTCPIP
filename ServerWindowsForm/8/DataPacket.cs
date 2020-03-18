using System.Runtime.InteropServices;

namespace _8 {
    [StructLayout(LayoutKind.Sequential)]
    class DataPacket {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Subject;

        [MarshalAs(UnmanagedType.I4)]
        public int Grade;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Memo;
    }
}
