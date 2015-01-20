using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VDDK;

namespace dotnetvddk
{
    class Program
    {
        static void Main(string[] args)
        {            
            // define this basic info to your enviroment and VM you want to connect to
            const string server = "vcenter01";
            const string user = "vddkuser@virtualsharp";
            const string password = "vddkuser";
            const string vmdkpath = "[UNITRENDS_QA_01] Stress_TCL.01/Stress_TCL.01.vmdk";
            const string moref = "vm-324307";
            //


            int offset = 0; // start reading at the beggining of disk
            int length = 1; // try to read 1 block
            var buffer = new Byte[length * 512];

            // Mono + VDDK64
            var connParams64 = new VixDiskLibConnectParams51x64();
            connParams64.ServerName = server;
            connParams64.CredType = VixDiskLibCredType.VIXDISKLIB_CRED_UID;
            connParams64.UserName = user;
            connParams64.Password = password;
            connParams64.Port = 902;
            connParams64.VmxSpec = "moref=" + moref; //"moref=2";

            // Windows + VDDK64
            var connParams32 = new VixDiskLibConnectParams51x32();
            connParams32.ServerName = server;
            connParams32.CredType = VixDiskLibCredType.VIXDISKLIB_CRED_UID;
            connParams32.UserName = user;
            connParams32.Password = password;
            connParams32.Port = 902;
            connParams32.VmxSpec = "moref=" + moref; //"moref=2";


            var libdir = "/usr/lib/vmware-vix-disklib/lib64";
            var configfile = "./vddk.conf";


            if (Environment.OSVersion.Platform != PlatformID.Unix) {
                var vddkPath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "vddk_x64"); //vddk64 lib must be in app base directoy \vddk_x64
                Environment.SetEnvironmentVariable("PATH", vddkPath);
                libdir = null;
                configfile = null;
            }

            var status = VixDiskLib.VixDiskLib_InitEx(1, 0,
                                      null,
                                     (a, b) => Console.WriteLine(string.Format("Warning VixDiskLib_InitEx:  {0}", a)),
                                     (a, b) => Console.WriteLine(string.Format("Warning VixDiskLib_InitEx:  {0}", a)),
                                     libdir, configfile);
            Console.WriteLine("64-bit - VixDiskLib_InitEx() - result: " + status + Environment.NewLine);

            IntPtr vixConnHandle = IntPtr.Zero;
            IntPtr vixDiskHandle;


            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                status = VixDiskLib.VixDiskLib_Connect(connParams64, ref vixConnHandle);
            }
            else
            {
                status = VixDiskLib.VixDiskLib_Connect(connParams32, ref vixConnHandle);
            }

            Console.WriteLine("64-bit - VixDiskLib_Connect() - result: " + status + " - handle: " + vixConnHandle + Environment.NewLine);
            status = VixDiskLib.VixDiskLib_Open(vixConnHandle, vmdkpath, 4, out vixDiskHandle);
            Console.WriteLine("64-bit - VixDiskLib_Open() - result: " + status + " - handle: " + vixDiskHandle +  Environment.NewLine);
            status = VixDiskLib.VixDiskLib_Read(vixDiskHandle, Convert.ToUInt64(offset), Convert.ToUInt64(length), buffer);
            Console.WriteLine("64-bit - VixDiskLib_Read() - result: " + status + Environment.NewLine);
            status = VixDiskLib.VixDiskLib_Disconnect(vixConnHandle);
            Console.WriteLine("64-bit - VixDiskLib_Disconnect() - result: " + status + Environment.NewLine);
            status = VixDiskLib.VixDiskLib_Close(vixDiskHandle);
            Console.WriteLine("64-bit - VixDiskLib_Close() - result: " + status + Environment.NewLine);
            VixDiskLib.VixDiskLib_Exit();
            Console.WriteLine("64-bit - VixDiskLib_Exit() - result: " +  Environment.NewLine);
            Console.ReadLine();
        }
    }
}
