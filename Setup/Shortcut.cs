using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Setup
{
    static internal class NativeClasses
    {
        [Flags]
        internal enum SLR_MODE : uint
        {
            /// <summary>
            /// Call the Microsoft Windows Installer.
            /// </summary>
            SLR_INVOKE_MSI = 0x80,

            /// <summary>
            /// Disable distributed link tracking. By default, distributed link tracking tracks removable media across multiple devices based on the volume name. It also uses the Universal Naming Convention (UNC) path to track remote file systems whose drive letter has changed. Setting SLR_NOLINKINFO disables both types of tracking.
            /// </summary>
            SLR_NOLINKINFO = 0x40,

            /// <summary>
            /// Do not display a dialog box if the link cannot be resolved. When SLR_NO_UI is set, the high-order word of fFlags can be set to a time-out value that specifies the maximum amount of time to be spent resolving the link. The function returns if the link cannot be resolved within the time-out duration. If the high-order word is set to zero, the time-out duration will be set to the default value of 3,000 milliseconds (3 seconds). To specify a value, set the high word of fFlags to the desired time-out duration, in milliseconds.
            /// </summary>
            SLR_NO_UI = 0x1,

            /// <summary>
            /// Do not update the link information.
            /// </summary>
            SLR_NOUPDATE = 0x8,

            /// <summary>
            /// Do not execute the search heuristics.
            /// </summary>
            SLR_NOSEARCH = 0x10,

            /// <summary>
            /// Do not use distributed link tracking.
            /// </summary>
            SLR_NOTRACK = 0x20,

            /// <summary>
            /// If the link object has changed, update its path and list of identifiers. If SLR_UPDATE is set, you do not need to call IPersistFile::IsDirty to determine whether or not the link object has changed.
            /// </summary>
            SLR_UPDATE = 0x4,

            SLR_NO_UI_WITH_MSG_PUMP = 0x101
        }

        [Flags]
        internal enum STGM_ACCESS : uint
        {
            /// <summary>
            /// Indicates that the object is read-only, meaning that modifications cannot be made. 
            /// For example, if a stream object is opened with STGM_READ, the ISequentialStream::Read method 
            ///     may be called, but the ISequentialStream::Write method may not. 
            /// Similarly, if a storage object opened with STGM_READ, the IStorage::OpenStream and 
            ///     IStorage::OpenStorage methods may be called, but the IStorage::CreateStream and 
            ///     IStorage::CreateStorage methods may not.
            /// </summary>
            STGM_READ = 0x00000000,

            /// <summary>
            /// Enables you to save changes to the object, but does not permit access to its data. 
            /// The provided implementations of the IPropertyStorage and IPropertySetStorage interfaces 
            ///     do not support this write-only mode.
            /// </summary>
            STGM_WRITE = 0x00000001,

            /// <summary>
            /// Enables access and modification of object data. 
            /// For example, if a stream object is created or opened in this mode, it is possible 
            ///     to call both IStream::Read and IStream::Write. 
            /// Be aware that this constant is not a simple binary OR operation of the STGM_WRITE and 
            ///     STGM_READ elements.
            /// </summary>
            STGM_READWRITE = 0x00000002,

            /// <summary>
            /// Specifies that subsequent openings of the object are not denied read or write access. 
            /// If no flag from the sharing group is specified, this flag is assumed.
            /// </summary>
            STGM_SHARE_DENY_NONE = 0x00000040,

            /// <summary>
            /// Prevents others from subsequently opening the object in STGM_READ mode. 
            /// It is typically used on a root storage object.
            /// </summary>
            STGM_SHARE_DENY_READ = 0x00000030,

            /// <summary>
            /// Prevents others from subsequently opening the object for STGM_WRITE or STGM_READWRITE access. 
            /// In transacted mode, sharing of STGM_SHARE_DENY_WRITE or STGM_SHARE_EXCLUSIVE can 
            ///     significantly improve performance because they do not require snapshots. 
            /// </summary>
            STGM_SHARE_DENY_WRITE = 0x00000020,

            /// <summary>
            /// Prevents others from subsequently opening the object in any mode. 
            /// Be aware that this value is not a simple bitwise OR operation of the STGM_SHARE_DENY_READ 
            ///     and STGM_SHARE_DENY_WRITE values. 
            /// In transacted mode, sharing of STGM_SHARE_DENY_WRITE or STGM_SHARE_EXCLUSIVE can 
            ///     significantly improve performance because they do not require snapshots.
            /// </summary>
            STGM_SHARE_EXCLUSIVE = 0x00000010,

            /// <summary>
            /// Opens the storage object with exclusive access to the most recently committed version. 
            /// Thus, other users cannot commit changes to the object while you have it open in priority mode. 
            /// You gain performance benefits for copy operations, but you prevent others from committing changes. 
            /// Limit the time that objects are open in priority mode. 
            /// You must specify STGM_DIRECT and STGM_READ with priority mode, and you 
            ///     cannot specify STGM_DELETEONRELEASE. 
            /// STGM_DELETEONRELEASE is only valid when creating a root object, such as with StgCreateStorageEx. 
            /// It is not valid when opening an existing root object, such as with StgOpenStorageEx. 
            /// It is also not valid when creating or opening a subelement, such as with IStorage::OpenStorage.
            /// </summary>
            STGM_PRIORITY = 0x00040000,

            /// <summary>
            /// Indicates that an existing storage object or stream should be removed before the new object replaces it. 
            /// A new object is created when this flag is specified only if the existing object has been 
            ///     successfully removed.
            ///     
            /// This flag is used when attempting to create:
            ///     A storage object on a disk, but a file of that name exists. 
            ///     An object inside a storage object, but a object with the specified name exists. 
            ///     A byte array object, but one with the specified name exists. 
            /// This flag cannot be used with open operations, such as StgOpenStorageEx or IStorage::OpenStream.
            /// </summary>
            STGM_CREATE = 0x00001000,

            /// <summary>
            /// Creates the new object while preserving existing data in a stream named "Contents". In the case of a storage object or a byte array, the old data is formatted into a stream regardless of whether the existing file or byte array currently contains a layered storage object. This flag can only be used when creating a root storage object. It cannot be used within a storage object; for example, in IStorage::CreateStream. It is also not valid to use this flag and the STGM_DELETEONRELEASE flag simultaneously.
            /// </summary>
            STGM_CONVERT = 0x00020000,

            /// <summary>
            /// Causes the create operation to fail if an existing object with the specified name exists. In this case, STG_E_FILEALREADYEXISTS is returned. This is the default creation mode; that is, if no other create flag is specified, STGM_FAILIFTHERE is implied.
            /// </summary>
            STGM_FAILIFTHERE = 0x00000000,

            /// <summary>
            /// Indicates that, in direct mode, each change to a storage or stream element is written as it occurs. This is the default if neither STGM_DIRECT nor STGM_TRANSACTED is specified.
            /// </summary>
            STGM_DIRECT = 0x00000000,

            /// <summary>
            /// Indicates that, in transacted mode, changes are buffered and written only if an explicit commit operation is called. To ignore the changes, call the Revert method in the IStream, IStorage, or IPropertyStorage interface. The COM compound file implementation of IStorage does not support transacted streams, which means that streams can be opened only in direct mode, and you cannot revert changes to them, however transacted storages are supported. The compound file, stand-alone, and NTFS file system implementations of IPropertySetStorage similarly do not support transacted, simple property sets because these property sets are stored in streams. However, transactioning of nonsimple property sets, which can be created by specifying the PROPSETFLAG_NONSIMPLE flag in the grfFlags parameter of IPropertySetStorage::Create, are supported.
            /// </summary>
            STGM_TRANSACTED = 0x00010000,

            /// <summary>
            /// Indicates that, in transacted mode, a temporary scratch file is usually used to save modifications until the Commit method is called. Specifying STGM_NOSCRATCH permits the unused portion of the original file to be used as work space instead of creating a new file for that purpose. This does not affect the data in the original file, and in certain cases can result in improved performance. It is not valid to specify this flag without also specifying STGM_TRANSACTED, and this flag may only be used in a root open. For more information about NoScratch mode, see the Remarks section.
            /// </summary>
            STGM_NOSCRATCH = 0x00100000,

            /// <summary>
            /// This flag is used when opening a storage object with STGM_TRANSACTED and without STGM_SHARE_EXCLUSIVE or STGM_SHARE_DENY_WRITE. In this case, specifying STGM_NOSNAPSHOT prevents the system-provided implementation from creating a snapshot copy of the file. Instead, changes to the file are written to the end of the file. Unused space is not reclaimed unless consolidation is performed during the commit, and there is only one current writer on the file. When the file is opened in no snapshot mode, another open operation cannot be performed without specifying STGM_NOSNAPSHOT. This flag may only be used in a root open operation. For more information about NoSnapshot mode, see the Remarks section.
            /// </summary>
            STGM_NOSNAPSHOT = 0x00200000,

            /// <summary>
            /// Provides a faster implementation of a compound file in a limited, but frequently used, case. For more information, see the Remarks section.
            /// </summary>
            STGM_SIMPLE = 0x08000000,

            /// <summary>
            /// Supports direct mode for single-writer, multireader file operations. For more information, see the Remarks section.
            /// </summary>
            STGM_DIRECT_SWMR = 0x00400000,

            /// <summary>
            /// Indicates that the underlying file is to be automatically destroyed when the root storage object is released. This feature is most useful for creating temporary files. This flag can only be used when creating a root object, such as with StgCreateStorageEx. It is not valid when opening a root object, such as with StgOpenStorageEx, or when creating or opening a subelement, such as with IStorage::CreateStream. It is also not valid to use this flag and the STGM_CONVERT flag simultaneously.
            /// </summary>
            STGM_DELETEONRELEASE = 0x04000000
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0)]
        internal struct _FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0, CharSet = CharSet.Unicode)]
        internal struct _WIN32_FIND_DATAW
        {
            public uint dwFileAttributes;
            public _FILETIME ftCreationTime;
            public _FILETIME ftLastAccessTime;
            public _FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        internal const uint SLGP_SHORTPATH = 0x01;     // Retrieves the standard short (8.3 format) file name. 
        internal const uint SLGP_UNCPRIORITY = 0x02;   // Retrieves the Universal Naming Convention (UNC) path name of the file. 
        internal const uint SLGP_RAWPATH = 0x04;       // Retrieves the raw path name. A raw path is something that might not exist and may include environment variables that need to be expanded.

        [ComImport()]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellLinkW
        {
            /// <summary>
            /// Retrieves the path and filename of a shell link object
            /// </summary> 
            [PreserveSig()]
            int GetPath(
               [Out(), MarshalAs(UnmanagedType.LPWStr)]
               StringBuilder pszFile,
               int cchMaxPath,
               ref _WIN32_FIND_DATAW pfd,
               uint fFlags);

            /// <summary>
            /// Retrieves the list of shell linkitem identifiers
            /// </summary>
            [PreserveSig()]
            int GetIDList(out IntPtr ppidl);

            /// <summary>
            /// Sets the list of shell link item identifiers
            /// </summary>
            [PreserveSig()]
            int SetIDList(IntPtr pidl);

            /// <summary>
            /// Retrieves the shell link description string
            /// </summary>
            [PreserveSig()]
            int GetDescription(
               [Out(), MarshalAs(UnmanagedType.LPWStr)]
               StringBuilder pszFile,
               int cchMaxName);

            /// <summary>
            /// Sets the shell link description string
            /// </summary>
            [PreserveSig()]
            int SetDescription(
               [MarshalAs(UnmanagedType.LPWStr)] string pszName);

            /// <summary>
            /// Retrieves the name of the shell link working directory
            /// </summary>
            [PreserveSig()]
            int GetWorkingDirectory(
               [Out(), MarshalAs(UnmanagedType.LPWStr)]
               StringBuilder pszDir,
               int cchMaxPath);

            /// <summary>
            /// Sets the name of the shell link working directory
            /// </summary>
            [PreserveSig()]
            int SetWorkingDirectory(
               [MarshalAs(UnmanagedType.LPWStr)] string pszDir);

            /// <summary>
            /// Retrieves the shell link command-line arguments
            /// </summary>
            [PreserveSig()]
            int GetArguments(
               [Out(), MarshalAs(UnmanagedType.LPWStr)]
               StringBuilder pszArgs,
               int cchMaxPath);

            /// <summary>
            /// Sets the shell link command-line arguments
            /// </summary>
            [PreserveSig()]
            int SetArguments(
               [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

            /// <summary>
            /// Retrieves or sets the shell link hot key
            /// </summary>
            [PreserveSig()]
            int GetHotkey(out ushort pwHotkey);

            /// <summary>
            /// Retrieves or sets the shell link hot key
            /// </summary>
            [PreserveSig()]
            int SetHotkey(ushort pwHotkey);

            /// <summary>
            /// Retrieves or sets the shell link show command
            /// </summary>
            [PreserveSig()]
            int GetShowCmd(out uint piShowCmd);

            /// <summary>
            /// Retrieves or sets the shell link show command
            /// </summary>
            [PreserveSig()]
            int SetShowCmd(uint piShowCmd);

            /// <summary>
            /// Retrieves the location (path and index) of the shell link icon
            /// </summary>
            [PreserveSig()]
            int GetIconLocation(
               [Out(), MarshalAs(UnmanagedType.LPWStr)] 
               StringBuilder pszIconPath,
               int cchIconPath,
               out int piIcon);

            /// <summary>
            /// Sets the location (path and index) of the shell link icon
            /// </summary>
            [PreserveSig()]
            int SetIconLocation(
               [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
               int iIcon);

            /// <summary>
            /// Sets the shell link relative path
            /// </summary>
            [PreserveSig()]
            int SetRelativePath(
               [MarshalAs(UnmanagedType.LPWStr)] 
               string pszPathRel,
               uint dwReserved);

            /// <summary>
            /// Resolves a shell link. The system
            /// searches for the shell link object and updates 
            /// the shell link path and its list of 
            /// identifiers (if necessary)
            /// </summary>
            [PreserveSig()]
            int Resolve(
               IntPtr hWnd,
               uint fFlags);

            /// <summary>
            /// Sets the shell link path and filename
            /// </summary>
            [PreserveSig()]
            int SetPath(
               [MarshalAs(UnmanagedType.LPWStr)]
               string pszFile);
        }

        [ComImport()]
        [Guid("0000010B-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IPersistFile
        {
            [PreserveSig()]
            int GetClassID(out Guid pClassID);

            /// <summary>Checks for changes since last file write
            /// </summary>
            [PreserveSig()]
            int IsDirty();

            /// <summary>
            /// Opens the specified file and initializes the object from its contents
            /// </summary>
            [PreserveSig()]
            int Load(
               [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
               uint dwMode);

            /// <summary>
            /// Saves the object into the specified file
            /// </summary>
            [PreserveSig()]
            int Save(
               [MarshalAs(UnmanagedType.LPWStr)] 
               string pszFileName,
               [MarshalAs(UnmanagedType.Bool)] 
               bool fRemember);

            /// <summary>
            /// Notifies the object that save is completed
            /// </summary>
            [PreserveSig()]
            int SaveCompleted(
               [MarshalAs(UnmanagedType.LPWStr)]
               string pszFileName);

            /// <summary>
            /// Gets the current name of the file associated with the object
            /// </summary>
            [PreserveSig()]
            int GetCurFile(
               [Out(), MarshalAs(UnmanagedType.LPWStr)] 
               StringBuilder pszIconPath);
        }

        [Guid("00021401-0000-0000-C000-000000000046")]
        [ClassInterface(ClassInterfaceType.None)]
        [ComImport()]
        private class CShellLink { }

        internal static NativeClasses.IShellLinkW CreateShellLink()
        {
            return (NativeClasses.IShellLinkW)new NativeClasses.CShellLink();
        }
    }

    public class Shortcut
    {
        private const int MAX_DESCRIPTION_LENGTH = 512;
        private const int MAX_PATH = 512;

        private NativeClasses.IShellLinkW _link;

        public Shortcut()
        {
            this._link = NativeClasses.CreateShellLink();
        }

        public Shortcut(string path)
            : this()
        {
            Marshal.ThrowExceptionForHR(this._link.SetPath(path));
        }

        public string Path
        {
            get
            {
                NativeClasses._WIN32_FIND_DATAW fdata = new NativeClasses._WIN32_FIND_DATAW();
                StringBuilder path = new StringBuilder(MAX_PATH, MAX_PATH);

                Marshal.ThrowExceptionForHR(
                    this._link.GetPath(path, path.MaxCapacity, ref fdata, NativeClasses.SLGP_UNCPRIORITY)
                    );

                return path.ToString();
            }

            set
            {
                Marshal.ThrowExceptionForHR(
                    this._link.SetPath(value)
                    );
            }
        }

        public string Description
        {
            get
            {
                StringBuilder desc = new StringBuilder(MAX_DESCRIPTION_LENGTH, MAX_DESCRIPTION_LENGTH);
                Marshal.ThrowExceptionForHR(
                    this._link.GetDescription(desc, desc.MaxCapacity)
                    );

                return desc.ToString();
            }

            set
            {
                Marshal.ThrowExceptionForHR(
                    this._link.SetDescription(value)
                    );
            }
        }

        public string RelativePath
        {
            set
            {
                Marshal.ThrowExceptionForHR(
                    this._link.SetRelativePath(value, 0)
                    );
            }
        }

        public string WorkingDirectory
        {
            get
            {
                StringBuilder dir = new StringBuilder(MAX_PATH, MAX_PATH);
                Marshal.ThrowExceptionForHR(
                    this._link.GetWorkingDirectory(dir, dir.MaxCapacity)
                    );

                return dir.ToString();
            }

            set
            {
                Marshal.ThrowExceptionForHR(
                    this._link.SetWorkingDirectory(value)
                    );
            }
        }
        public void SetIcon(String path, int index)
        {
            Marshal.ThrowExceptionForHR(this._link.SetIconLocation(path, index));
        }
        public string Arguments
        {
            get
            {
                StringBuilder args = new StringBuilder(MAX_PATH, MAX_PATH);
                Marshal.ThrowExceptionForHR(
                    this._link.GetArguments(args, args.MaxCapacity)
                    );

                return args.ToString();
            }

            set
            {
                Marshal.ThrowExceptionForHR(
                    this._link.SetArguments(value)
                    );
            }
        }

        public ushort HotKey
        {
            get
            {
                ushort key = 0;
                Marshal.ThrowExceptionForHR(
                    this._link.GetHotkey(out key)
                    );

                return key;
            }

            set
            {
                Marshal.ThrowExceptionForHR(
                    this._link.SetHotkey(value)
                    );
            }
        }

        public void Resolve(IntPtr hwnd, uint flags)
        {
            Marshal.ThrowExceptionForHR(
                this._link.Resolve(hwnd, flags)
                );
        }

        public void Resolve(IWin32Window window)
        {
            this.Resolve(window.Handle, 0);
        }

        public void Resolve()
        {
            this.Resolve(IntPtr.Zero, (uint)NativeClasses.SLR_MODE.SLR_NO_UI);
        }

        private NativeClasses.IPersistFile AsPersist
        {
            get { return ((NativeClasses.IPersistFile)this._link); }
        }

        public void Save(string fileName)
        {
            int hres = this.AsPersist.Save(fileName, true);

            Marshal.ThrowExceptionForHR(hres);
        }

        public void Load(string fileName)
        {
            int hres = this.AsPersist.Load(fileName, (uint)NativeClasses.STGM_ACCESS.STGM_READ);

            Marshal.ThrowExceptionForHR(hres);
        }
    }
}
