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


    public class ConnectionInformation
    {
        private IntPtr conn;

        // fuse_conn_info member offsets
        const int
            ProtMajor = 0,
            ProtMinor = 1,
            AsyncRead = 2,
            MaxWrite = 3,
            MaxRead = 4;

        internal ConnectionInformation(IntPtr conn)
        {
            this.conn = conn;
        }

        public uint ProtocolMajorVersion
        {
            get { return (uint)Marshal.ReadInt32(conn, ProtMajor); }
        }

        public uint ProtocolMinorVersion
        {
            get { return (uint)Marshal.ReadInt32(conn, ProtMinor); }
        }

        public bool AsynchronousReadSupported
        {
            get { return Marshal.ReadInt32(conn, AsyncRead) != 0; }
            set { Marshal.WriteInt32(conn, AsyncRead, value ? 1 : 0); }
        }

        public uint MaxWriteBufferSize
        {
            get { return (uint)Marshal.ReadInt32(conn, MaxWrite); }
            set { Marshal.WriteInt32(conn, MaxWrite, (int)value); }
        }

        public uint MaxReadahead
        {
            get { return (uint)Marshal.ReadInt32(conn, MaxRead); }
            set { Marshal.WriteInt32(conn, MaxRead, (int)value); }
        }
    }


}
