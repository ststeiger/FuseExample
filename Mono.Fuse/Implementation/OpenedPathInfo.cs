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


    [Map]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class OpenedPathInfo
    {
        internal OpenFlags flags;
        private int write_page;
        private bool direct_io;
        private bool keep_cache;
        private ulong file_handle;

        internal OpenedPathInfo()
        {
        }

        public OpenFlags OpenFlags
        {
            get { return flags; }
            set { flags = value; }
        }

        private const OpenFlags accessMask =
            OpenFlags.O_RDONLY | OpenFlags.O_WRONLY | OpenFlags.O_RDWR;

        public OpenFlags OpenAccess
        {
            get { return flags & accessMask; }
        }

        public int WritePage
        {
            get { return write_page; }
            set { write_page = value; }
        }

        public bool DirectIO
        {
            get { return direct_io; }
            set { direct_io = value; }
        }

        public bool KeepCache
        {
            get { return keep_cache; }
            set { keep_cache = value; }
        }

        public IntPtr Handle
        {
            get { return (IntPtr)(long)file_handle; }
            set { file_handle = (ulong)(long)value; }
        }
    }


}
