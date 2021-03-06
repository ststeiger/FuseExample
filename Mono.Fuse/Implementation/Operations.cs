﻿//
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



    [Map]
    [StructLayout(LayoutKind.Sequential)]
    class Operations
    {
        public GetPathStatusCb getattr;
        public ReadSymbolicLinkCb readlink;
        public CreateSpecialFileCb mknod;
        public CreateDirectoryCb mkdir;
        public RemoveFileCb unlink;
        public RemoveDirectoryCb rmdir;
        public CreateSymbolicLinkCb symlink;
        public RenamePathCb rename;
        public CreateHardLinkCb link;
        public ChangePathPermissionsCb chmod;
        public ChangePathOwnerCb chown;
        public TruncateFileb truncate;
        public ChangePathTimesCb utime;
        public OpenHandleCb open;
        public ReadHandleCb read;
        public WriteHandleCb write;
        public GetFileSystemStatusCb statfs;
        public FlushHandleCb flush;
        public ReleaseHandleCb release;
        public SynchronizeHandleCb fsync;
        public SetPathExtendedAttributeCb setxattr;
        public GetPathExtendedAttributeCb getxattr;
        public ListPathExtendedAttributesCb listxattr;
        public RemovePathExtendedAttributeCb removexattr;
        public OpenDirectoryCb opendir;
        public ReadDirectoryCb readdir;
        public ReleaseDirectoryCb releasedir;
        public SynchronizeDirectoryCb fsyncdir;
        public InitCb init;
        public DestroyCb destroy;
        public AccessPathCb access;
        public CreateHandleCb create;
        public TruncateHandleCb ftruncate;
        public GetHandleStatusCb fgetattr;
        public LockHandleCb @lock;
        public MapPathLogicalToPhysicalIndexCb bmap;
    }


}
