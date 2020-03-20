using System.Runtime.InteropServices;

namespace _9 {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct DataPacket {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Subject;

        [MarshalAs(UnmanagedType.I4)]
        public int Grade;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Memo;

        public byte[] Serialize() {
            var buffer = new byte[Marshal.SizeOf(typeof(DataPacket))];
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }

        public void Deserialize(ref byte[] data) {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (DataPacket)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(DataPacket));
            gch.Free();
        }
    }
}
