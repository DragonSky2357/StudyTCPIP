using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _6 {
    class Program {
        static void Main(string[] args) {
            try {
                string Name = string.Empty;
                string Subject = string.Empty;
                Int32 Grade = 0;
                string Memo = string.Empty;

                do {
                    Console.Write("이름 : ");
                    Name = Console.ReadLine();

                    Console.Write("과목 : ");
                    Subject = Console.ReadLine();

                    Console.Write("점수 : ");
                    string tmpGrage = Console.ReadLine();
                    if (tmpGrage != "")
                        Grade = Convert.ToInt32(tmpGrage);
                    else
                        Grade = 0;

                    Console.Write("메모 : ");
                    Memo = Console.ReadLine();

                    if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Subject)) break;

                    DataPacket packet = new DataPacket();

                    packet.Name = Name;
                    packet.Subject = Subject;
                    packet.Grade = Grade;
                    packet.Memo = Memo;

                    byte[] buffer = new byte[Marshal.SizeOf(packet)];

                    unsafe {
                        fixed (byte* fixed_buffer = buffer) {
                            Marshal.StructureToPtr(packet, (IntPtr)fixed_buffer, false);
                        }
                    }

                    TcpClient client = new TcpClient();
                    client.Connect("192.168.254.1", 13000);
                    Console.WriteLine("Connected...");

                    NetworkStream stream = client.GetStream();

                    stream.Write(buffer, 0, Marshal.SizeOf(packet));

                    stream.Close();
                    client.Close();
                } while (Name != "" && Subject != "");
            } catch (SocketException se) {
                Console.WriteLine(se.Message.ToString());
            } catch (Exception ex) {
                Console.WriteLine(ex.Message.ToString());
            }
            Console.WriteLine("\nPress enter key to continue....");
            Console.ReadLine();
        }
    }
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
