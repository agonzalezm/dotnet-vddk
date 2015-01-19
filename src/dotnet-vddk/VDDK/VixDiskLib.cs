using System;
using System.Runtime.InteropServices;

namespace VDDK
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool VixDiskLibProgressFunc(IntPtr progressData, int percentCompleted);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void VixDiskLibLogFunc(string a, string b);

    [StructLayout(LayoutKind.Sequential)]
    public class VixDiskLibGenericLogFunc
    {
        public String fmt;
        public String va_list;
    }

    public enum VixDiskLibDiskType
    {
        VIXDISKLIB_DISK_MONOLITHIC_FLAT = 2,
        VIXDISKLIB_DISK_MONOLITHIC_SPARSE = 1,
        VIXDISKLIB_DISK_SPLIT_FLAT = 4,
        VIXDISKLIB_DISK_SPLIT_SPARSE = 3,
        VIXDISKLIB_DISK_STREAM_OPTIMIZED = 6,
        VIXDISKLIB_DISK_UNKNOWN = 0x100,
        VIXDISKLIB_DISK_VMFS_FLAT = 5
    }

    public struct VixDiskLibCreateParams
    {
        public VixDiskLibDiskType diskType;
        public VixDiskLibAdapterType adapterType;
        public ushort hwVersion;
        public uint capacity;
    }

    public enum VixDiskLibAdapterType
    {
        VIXDISKLIB_ADAPTER_IDE = 1,
        VIXDISKLIB_ADAPTER_SCSI_BUSLOGIC = 2,
        VIXDISKLIB_ADAPTER_SCSI_LSILOGIC = 3,
        VIXDISKLIB_ADAPTER_UNKNOWN = 0x100
    }

    public enum VixDiskLibCredType
    {
        VIXDISKLIB_CRED_UID = 1,     // use userid password
        VIXDISKLIB_CRED_SESSIONID = 2,  // http session id
        VIXDISKLIB_CRED_TICKETID = 3,   // vim ticket id
        VIXDISKLIB_CRED_UNKNOWN = 256
    }

    [StructLayout(LayoutKind.Sequential)]
    public class VixDiskLibUidPasswdCreds
    {
        public String userName;
        public String password;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class VixDiskLibCreds
    {
        public VixDiskLibUidPasswdCreds uid = new VixDiskLibUidPasswdCreds();
        public IntPtr ticketId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Ansi)]
    public class VixDiskLibConnectParams51x64
    {
        public string VmxSpec;      // URL like spec of the VM.
        public string ServerName;   // Name or IP address of VC / ESX.
        public string ThumbPrint;   // SSL Certificate thumb print.
        public Int64 PrivateUse;    // This value is ignored.
        public VixDiskLibCredType CredType;
        public string UserName;     // User id
        public string Password;     // Password
        private IntPtr Dummy;
        public UInt32 Port;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class VixDiskLibConnectParams
    {
        public String vmxSpec;
        public String serverName;
        public VixDiskLibCredType credType = new VixDiskLibCredType();
        public VixDiskLibCreds creds = new VixDiskLibCreds();
        public UInt32 port;
    }

    /// <summary>
    /// class is a C# wrapper for the vixDisk API's
    /// </summary>
    public class VixDiskLib
    {
        public const int VIXDISKLIB_SECTOR_SIZE = 512;
        public const int VIXDISKLIB_FLAG_OPEN_READ_ONLY = 4;
        public const int VIXDISKLIB_FLAG_OPEN_SINGLE_LINK = 2;
        public const int VIXDISKLIB_FLAG_OPEN_UNBUFFERED = 1;
        public const int VIXDISKLIB_HWVERSION_CURRENT = 6;
        public const int VIXDISKLIB_HWVERSION_ESX30 = 4;
        public const int VIXDISKLIB_HWVERSION_WORKSTATION_4 = 3;
        public const int VIXDISKLIB_HWVERSION_WORKSTATION_5 = 4;
        public const int VIXDISKLIB_HWVERSION_WORKSTATION_6 = 6;
        public const int VIXDISKLIB_VERSION_MAJOR = 1;
        public const int VIXDISKLIB_VERSION_MINOR = 2;


        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Init", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Init(UInt32 majorVersion, UInt32 minorVersion, VixDiskLibLogFunc log, VixDiskLibLogFunc warn, VixDiskLibLogFunc panic, String libDir);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_InitEx", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_InitEx(UInt32 majorVersion, UInt32 minorVersion, VixDiskLibLogFunc log, VixDiskLibLogFunc warn, VixDiskLibLogFunc panic, String libDir, String configFile);

        [DllImport("vixDiskLib", SetLastError = true, EntryPoint = "VixDiskLib_Connect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Connect([MarshalAs(UnmanagedType.AsAny)] object connectParams, ref IntPtr connection);

        //[DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Connect", CallingConvention = CallingConvention.Cdecl)]
        //public static extern VixError VixDiskLib_Connect(VixDiskLibConnectParams51x64 connectParams, ref IntPtr connection);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_ConnectEx", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_ConnectEx(VixDiskLibConnectParams connectParams, bool ReadOnly, string SnapshotMoRef, string transportMode, out IntPtr connection);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Disconnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Disconnect(IntPtr connectionHandle);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Cleanup", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Cleanup(VixDiskLibConnectParams connectParams, out UInt32 numCleanUp, out UInt32 numRemaing);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Open", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Open(IntPtr connection, String path, UInt32 flags, out IntPtr diskHandle);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Read", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Read(IntPtr diskHandle, UInt64 startSector, UInt64 numSectors, Byte[] readBuffer);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Write", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Write(IntPtr diskHandle, UInt64 startSector, UInt64 numSectors, Byte[] writeBuffer);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Close", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Close(IntPtr diskHandle);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Create", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError Create(IntPtr diskHandle);

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr VixDiskLib_GetErrorText(VixError vixErrorCode, IntPtr locale);

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void VixDiskLib_FreeErrorText(IntPtr vixErrorMsgPtr);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Clone", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Clone(IntPtr dstConnection, string dstPath, IntPtr srcConnection, string srcPath, out VixDiskLibCreateParams vixCreateParams, VixDiskLibProgressFunc progressFunc, IntPtr progressCallbackData, bool overWrite);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_Exit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void VixDiskLib_Exit();

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Create(IntPtr connection, [In, MarshalAs(UnmanagedType.LPStr)] string path, ref VixDiskLibCreateParams createParams, VixDiskLibProgressFunc progressFunc, IntPtr progressCallbackData);

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_CreateChild(IntPtr diskHandle, [In, MarshalAs(UnmanagedType.LPStr)] string childPath, VixDiskLibDiskType diskType, VixDiskLibProgressFunc progressFunc, IntPtr progressCallbackData);

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Defragment(IntPtr diskHandle, VixDiskLibProgressFunc progressFunc, IntPtr progressCallbackData);

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_GetInfo(IntPtr diskHandle, ref IntPtr info);

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_GetMetadataKeys(IntPtr diskHandle, IntPtr keysBuffer, uint bufLen, ref uint requiredLen);

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Grow();

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_ReadMetadata(IntPtr diskHandle, string key, IntPtr buf, uint bufLen, ref uint requiredLen);

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_WriteMetadata();

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Rename();

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_Shrink();

        [DllImport("vixDiskLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern string VixDiskLib_ListTransportModes(IntPtr diskHandle);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_PrepareForAccess", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_PrepareForAccess(VixDiskLibConnectParams connectParams, string VMname);

        [DllImport("vixDiskLib.dll", ExactSpelling = true, EntryPoint = "VixDiskLib_EndAccess", CallingConvention = CallingConvention.Cdecl)]
        public static extern VixError VixDiskLib_EndAccess(VixDiskLibConnectParams connectParams, string VMname);
    }

    public enum VixError : ulong
    {
        VIX_E_ALREADY_EXISTS = 0x17L,
        VIX_E_ANON_GUEST_OPERATIONS_PROHIBITED = 0xbcaL,
        VIX_E_BAD_VM_INDEX = 0x1f40L,
        VIX_E_BUFFER_TOOSMALL = 0x18L,
        VIX_E_CANCELLED = 10L,
        VIX_E_CANNOT_AUTHENTICATE_WITH_GUEST = 0xbd0L,
        VIX_E_CANNOT_CONNECT_TO_HOST = 0x4650L,
        VIX_E_CANNOT_CONNECT_TO_VM = 0xbc0L,
        VIX_E_CANNOT_READ_VM_CONFIG = 0xfa2L,
        VIX_E_CANNOT_START_READ_ONLY_VM = 0xbbdL,
        VIX_E_CONSOLE_GUEST_OPERATIONS_PROHIBITED = 0xbd2L,
        VIX_E_DISK_CANTSHRINK = 0x3e89L,
        VIX_E_DISK_CID_MISMATCH = 0x3e88L,
        VIX_E_DISK_FULL = 8L,
        VIX_E_DISK_INVAL = 0x3e80L,
        VIX_E_DISK_INVALID_CONNECTION = 0x3eb6L,
        VIX_E_DISK_INVALIDCHAIN = 0x3e9eL,
        VIX_E_DISK_INVALIDPARTITIONTABLE = 0x3e92L,
        VIX_E_DISK_KEY_NOTFOUND = 0x3eb4L,
        VIX_E_DISK_NEEDKEY = 0x3e8eL,
        VIX_E_DISK_NEEDSREPAIR = 0x3e86L,
        VIX_E_DISK_NEEDVMFS = 0x3e96L,
        VIX_E_DISK_NOINIT = 0x3e81L,
        VIX_E_DISK_NOIO = 0x3e82L,
        VIX_E_DISK_NOKEY = 0x3e91L,
        VIX_E_DISK_NOKEYOVERRIDE = 0x3e8fL,
        VIX_E_DISK_NOLICENSE = 0x3ec0L,
        VIX_E_DISK_NOTENCDESC = 0x3e94L,
        VIX_E_DISK_NOTENCRYPTED = 0x3e90L,
        VIX_E_DISK_NOTNORMAL = 0x3e93L,
        VIX_E_DISK_NOTSUPPORTED = 0x3e8dL,
        VIX_E_DISK_OPENPARENT = 0x3e8cL,
        VIX_E_DISK_OUTOFRANGE = 0x3e87L,
        VIX_E_DISK_PARTIALCHAIN = 0x3e83L,
        VIX_E_DISK_PARTMISMATCH = 0x3e8aL,
        VIX_E_DISK_RAWTOOBIG = 0x3e98L,
        VIX_E_DISK_RAWTOOSMALL = 0x3e9dL,
        VIX_E_DISK_SUBSYSTEM_INIT_FAIL = 0x3eb5L,
        VIX_E_DISK_TOOMANYOPENFILES = 0x3e9bL,
        VIX_E_DISK_TOOMANYREDO = 0x3e9cL,
        VIX_E_DISK_UNSUPPORTEDDISKVERSION = 0x3e8bL,
        VIX_E_FAIL = 1L,
        VIX_E_FILE_ACCESS_ERROR = 13L,
        VIX_E_FILE_ALREADY_EXISTS = 12L,
        VIX_E_FILE_ALREADY_LOCKED = 15L,
        VIX_E_FILE_ERROR = 7L,
        VIX_E_FILE_NAME_INVALID = 0x16L,
        VIX_E_FILE_NAME_TOO_LONG = 0x4e24L,
        VIX_E_FILE_NOT_FOUND = 4L,
        VIX_E_FILE_READ_ONLY = 11L,
        VIX_E_FILE_TOO_BIG = 0x15L,
        VIX_E_GUEST_OPERATIONS_PROHIBITED = 0xbc9L,
        VIX_E_GUEST_USER_PERMISSIONS = 0xbc7L,
        VIX_E_HOST_DISK_INVALID_VALUE = 0x36b3L,
        VIX_E_HOST_DISK_SECTORSIZE = 0x36b4L,
        VIX_E_HOST_FILE_ERROR_EOF = 0x36b5L,
        VIX_E_HOST_NBD_HASHFILE_INIT = 0x36bdL,
        VIX_E_HOST_NBD_HASHFILE_VOLUME = 0x36bcL,
        VIX_E_HOST_NETBLKDEV_HANDSHAKE = 0x36b6L,
        VIX_E_HOST_NETWORK_CONN_REFUSED = 0x36b9L,
        VIX_E_HOST_SERVER_NOT_FOUND = 0x36b8L,
        VIX_E_HOST_SOCKET_CREATION_ERROR = 0x36b7L,
        VIX_E_HOST_TCP_CONN_LOST = 0x36bbL,
        VIX_E_HOST_TCP_SOCKET_ERROR = 0x36baL,
        VIX_E_HOST_USER_PERMISSIONS = 0xbc6L,
        VIX_E_INCORRECT_FILE_TYPE = 9L,
        VIX_E_INVALID_ARG = 3L,
        VIX_E_INVALID_HANDLE = 0x3e8L,
        VIX_E_INVALID_PROPERTY_VALUE = 0x1771L,
        VIX_E_INVALID_XML = 0x7d2L,
        VIX_E_MISSING_ANON_GUEST_ACCOUNT = 0xbcfL,
        VIX_E_MISSING_REQUIRED_PROPERTY = 0x1773L,
        VIX_E_MUST_BE_CONSOLE_USER = 0xbd3L,
        VIX_E_NO_GUEST_OS_INSTALLED = 0xbc2L,
        VIX_E_NO_SUCH_PROCESS = 0x4e23L,
        VIX_E_NOT_A_DIRECTORY = 0x4e22L,
        VIX_E_NOT_A_FILE = 0x4e21L,
        VIX_E_NOT_ALLOWED_DURING_VM_RECORDING = 0xbd4L,
        VIX_E_NOT_ALLOWED_DURING_VM_REPLAY = 0xbd5L,
        VIX_E_NOT_FOR_REMOTE_HOST = 0x4651L,
        VIX_E_NOT_FOUND = 0x7d0L,
        VIX_E_NOT_SUPPORTED = 6L,
        VIX_E_NOT_SUPPORTED_FOR_VM_VERSION = 0xfa1L,
        VIX_E_NOT_SUPPORTED_ON_HANDLE_TYPE = 0x3e9L,
        VIX_E_NOT_SUPPORTED_ON_REMOTE_OBJECT = 20L,
        VIX_E_OBJECT_IS_BUSY = 5L,
        VIX_E_OP_NOT_SUPPORTED_ON_GUEST = 0xbbbL,
        VIX_E_OUT_OF_MEMORY = 2L,
        VIX_E_POWEROP_SCRIPTS_NOT_AVAILABLE = 0xbc1L,
        VIX_E_PROGRAM_NOT_STARTED = 0xbbcL,
        VIX_E_READ_ONLY_PROPERTY = 0x1772L,
        VIX_E_REQUIRES_LARGE_FILES = 14L,
        VIX_E_ROOT_GUEST_OPERATIONS_PROHIBITED = 0xbcbL,
        VIX_E_SNAPSHOT_CHECKPOINT = 0x32d1L,
        VIX_E_SNAPSHOT_CONFIG = 0x32cfL,
        VIX_E_SNAPSHOT_DISKLIB = 0x32caL,
        VIX_E_SNAPSHOT_DISKLOCKED = 0x32d6L,
        VIX_E_SNAPSHOT_DUMPER = 0x32c9L,
        VIX_E_SNAPSHOT_DUPLICATEDDISK = 0x32d7L,
        VIX_E_SNAPSHOT_EXISTS = 0x32ccL,
        VIX_E_SNAPSHOT_INCONSISTENT = 0x32d3L,
        VIX_E_SNAPSHOT_INDEPENDENTDISK = 0x32d8L,
        VIX_E_SNAPSHOT_INVAL = 0x32c8L,
        VIX_E_SNAPSHOT_LOCKED = 0x32d2L,
        VIX_E_SNAPSHOT_NAMETOOLONG = 0x32d4L,
        VIX_E_SNAPSHOT_NOCHANGE = 0x32d0L,
        VIX_E_SNAPSHOT_NONUNIQUE_NAME = 0x32d9L,
        VIX_E_SNAPSHOT_NOPERM = 0x32ceL,
        VIX_E_SNAPSHOT_NOTFOUND = 0x32cbL,
        VIX_E_SNAPSHOT_VERSION = 0x32cdL,
        VIX_E_SNAPSHOT_VIXFILE = 0x32d5L,
        VIX_E_SUSPEND_ERROR = 0xbc4L,
        VIX_E_TEMPLATE_VM = 0xfa3L,
        VIX_E_TIMEOUT_WAITING_FOR_TOOLS = 0xbb8L,
        VIX_E_TOO_MANY_HANDLES = 0x3eaL,
        VIX_E_TOOLS_NOT_RUNNING = 0xbc8L,
        VIX_E_TYPE_MISMATCH = 0x7d1L,
        VIX_E_UNRECOGNIZED_COMMAND = 0xbb9L,
        VIX_E_UNRECOGNIZED_COMMAND_IN_GUEST = 0xbd1L,
        VIX_E_UNRECOGNIZED_PROPERTY = 0x1770L,
        VIX_E_VM_ALREADY_LOADED = 0xfa4L,
        VIX_E_VM_ALREADY_UP_TO_DATE = 0xfa6L,
        VIX_E_VM_INSUFFICIENT_HOST_MEMORY = 0xbc3L,
        VIX_E_VM_IS_RUNNING = 0xbbfL,
        VIX_E_VM_NOT_ENOUGH_CPUS = 0xbc5L,
        VIX_E_VM_NOT_FOUND = 0xfa0L,
        VIX_E_VM_NOT_RUNNING = 0xbbeL,
        VIX_OK = 0L
    }


}
