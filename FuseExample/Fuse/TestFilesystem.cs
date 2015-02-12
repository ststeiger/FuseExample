//
// Mono.Fuse/FileSystem.cs
//
// Authors:
//   Jonathan Pryor (jonpryor@vt.edu)
//
// (C) 2006-2007 Jonathan Pryor
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Unix;
using Mono.Unix.Native;


namespace Mono.Fuse
{



    public abstract class TestFileSystem : FileSystem
    {
        protected TestFileSystem() : base()
        {}

        protected TestFileSystem(string mountPoint)
            : base(mountPoint)
        {}

        protected TestFileSystem(string[] args) 
            : base(args)
        {}


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        protected string GetCurrentMethod()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame sf = st.GetFrame(2);

            return sf.GetMethod().Name;
        }

        protected void Log()
        {
            System.Console.WriteLine("{0} called... ", GetCurrentMethod());
        }


        protected override void OnInit(ConnectionInformation connection)
        {
            Log();
            // base.OnInit(connection);
        }


        protected override Errno OnCreateDirectory(string directory, FilePermissions mode)
        {
            Log();
            return base.OnCreateDirectory(directory, mode);
        }


        protected override Errno OnReadDirectory(string directory, OpenedPathInfo info, out IEnumerable<DirectoryEntry> paths)
        {
            Log();
            return base.OnReadDirectory(directory, info, out paths);
        }


        protected override Errno OnAccessPath(string path, AccessModes mode)
        {
            Log();
            return base.OnAccessPath(path, mode);
        }

        protected override Errno OnChangePathOwner(string path, long owner, long group)
        {
            Log();
            return base.OnChangePathOwner(path, owner, group);
        }

        protected override Errno OnChangePathPermissions(string path, FilePermissions mode)
        {
            Log();
            return base.OnChangePathPermissions(path, mode);
        }

        protected override Errno OnChangePathTimes(string path, ref Utimbuf buf)
        {
            Log();
            return base.OnChangePathTimes(path, ref buf);
        }

        protected override Errno OnCreateHandle(string file, OpenedPathInfo info, FilePermissions mode)
        {
            Log();
            return base.OnCreateHandle(file, info, mode);
        }


        protected override Errno OnCreateHardLink(string oldpath, string link)
        {
            Log();
            return base.OnCreateHardLink(oldpath, link);
        }


        protected override Errno OnCreateSpecialFile(string file, FilePermissions perms, ulong dev)
        {
            Log();
            return base.OnCreateSpecialFile(file, perms, dev);
        }

        protected override Errno OnCreateSymbolicLink(string target, string link)
        {
            Log();
            return base.OnCreateSymbolicLink(target, link);
        }

        protected override Errno OnFlushHandle(string file, OpenedPathInfo info)
        {
            Log();
            return base.OnFlushHandle(file, info);
        }


        protected override Errno OnGetFileSystemStatus(string path, out Statvfs buf)
        {
            Log();
            return base.OnGetFileSystemStatus(path, out buf);
        }

        protected override Errno OnGetHandleStatus(string file, OpenedPathInfo info, out Stat buf)
        {
            Log();
            return base.OnGetHandleStatus(file, info, out buf);
        }

        protected override Errno OnGetPathExtendedAttribute(string path, string name, byte[] value, out int bytesWritten)
        {
            Log();
            return base.OnGetPathExtendedAttribute(path, name, value, out bytesWritten);
        }

        protected override Errno OnGetPathStatus(string path, out Stat stat)
        {
            Log();
            return base.OnGetPathStatus(path, out stat);
        }


        protected override Errno OnListPathExtendedAttributes(string path, out string[] names)
        {
            Log();
            return base.OnListPathExtendedAttributes(path, out names);
        }


        protected override Errno OnLockHandle(string file, OpenedPathInfo info, FcntlCommand cmd, ref Flock @lock)
        {
            Log();
            return base.OnLockHandle(file, info, cmd, ref @lock);
        }


        protected override Errno OnMapPathLogicalToPhysicalIndex(string path, ulong logical, out ulong physical)
        {
            Log();
            return base.OnMapPathLogicalToPhysicalIndex(path, logical, out physical);
        }


        protected override Errno OnOpenDirectory(string directory, OpenedPathInfo info)
        {
            Log();
            return base.OnOpenDirectory(directory, info);
        }


        protected override Errno OnOpenHandle(string file, OpenedPathInfo info)
        {
            Log();
            return base.OnOpenHandle(file, info);
        }


        protected override Errno OnReadHandle(string file, OpenedPathInfo info, byte[] buf, long offset, out int bytesWritten)
        {
            Log();
            return base.OnReadHandle(file, info, buf, offset, out bytesWritten);
        }

        protected override Errno OnReadSymbolicLink(string link, out string target)
        {
            Log();
            return base.OnReadSymbolicLink(link, out target);
        }

        protected override Errno OnReleaseDirectory(string directory, OpenedPathInfo info)
        {
            Log();
            return base.OnReleaseDirectory(directory, info);
        }

        protected override Errno OnReleaseHandle(string file, OpenedPathInfo info)
        {
            Log();
            return base.OnReleaseHandle(file, info);
        }

        protected override Errno OnRemoveDirectory(string directory)
        {
            Log();
            return base.OnRemoveDirectory(directory);
        }

        protected override Errno OnRemoveFile(string file)
        {
            Log();
            return base.OnRemoveFile(file);
        }

        protected override Errno OnRemovePathExtendedAttribute(string path, string name)
        {
            Log();
            return base.OnRemovePathExtendedAttribute(path, name);
        }

        protected override Errno OnRenamePath(string oldpath, string newpath)
        {
            Log();
            return base.OnRenamePath(oldpath, newpath);
        }

        protected override Errno OnSetPathExtendedAttribute(string path, string name, byte[] value, XattrFlags flags)
        {
            Log();
            return base.OnSetPathExtendedAttribute(path, name, value, flags);
        }

        protected override Errno OnSynchronizeDirectory(string directory, OpenedPathInfo info, bool onlyUserData)
        {
            Log();
            return base.OnSynchronizeDirectory(directory, info, onlyUserData);
        }

        protected override Errno OnSynchronizeHandle(string file, OpenedPathInfo info, bool onlyUserData)
        {
            Log();
            return base.OnSynchronizeHandle(file, info, onlyUserData);
        }


        protected override Errno OnTruncateFile(string file, long length)
        {
            Log();
            return base.OnTruncateFile(file, length);
        }


        protected override Errno OnTruncateHandle(string file, OpenedPathInfo info, long length)
        {
            Log();
            return base.OnTruncateHandle(file, info, length);
        }

        protected override Errno OnWriteHandle(string file, OpenedPathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            Log();
            return base.OnWriteHandle(file, info, buf, offset, out bytesRead);
        }

        protected override void Dispose(bool disposing)
        {
            Log();
            base.Dispose(disposing);
        }

        public override bool Equals(object obj)
        {
            Log();
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            Log();
            return base.GetHashCode();
        }

        public override string ToString()
        {
            Log();
            return base.ToString();
        }

    }


}
