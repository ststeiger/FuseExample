
using System.Collections.Generic;


// using Mono.Fuse;
using Mono.Unix.Native;


using System.Linq;


// http://www.jprl.com/Projects/mono-fuse/docs/Mono.Fuse/FileSystem.html
namespace FuseExample
{


    // apt-get install libmono-fuse-cil
    // gmcs -r:Mono.Fuse.dll -r:Mono.Posix.dll SimpleFS.cs
	// public class SqlFS : Mono.Fuse.FileSystem
	public class SqlFS : Mono.Fuse.TestFileSystem
    {

        private List<string> files = new List<string>();
        private List<string> folders = new List<string>();


        public SqlFS(string[] args)
            : base(args)
        {
            files.Add("/file1");
            files.Add("/file2");
            files.Add("/file3");
            files.Add("/файл4");

            folders.Add("/folder1");
            folders.Add("/folder2");
            folders.Add("/folder3");
            folders.Add("/фолдер4");
        }


        protected static bool? IsFolder(string strPath)
        {
            //lock (objLock)
            //{
            bool? bIsFolder = null;

            if (string.IsNullOrEmpty(strPath))
                return bIsFolder;

            strPath = strPath.Replace("'", "''");
            string strSQL = string.Format(
                                  @"SELECT isdir FROM nav_overview WHERE (1=1) AND obj_path = '{0}'"
                                 , strPath
            );

            try
            {
                object obj = SQL.ExecuteScalar(strSQL);

                if (obj != null)
                    bIsFolder = System.Convert.ToBoolean(obj);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            return bIsFolder;
            // } // End Lock
        }


        protected static string GetParentCondition(string strPath)
        {
            //lock (objLock)
            //{
            string strRetVal = null;

            if (string.IsNullOrEmpty(strPath))
                return "AND (1 = 2) ";

            if (System.StringComparer.InvariantCultureIgnoreCase.Equals(strPath, "/"))
                return "AND obj_parent_uid IS NULL ";

            strPath = strPath.Replace("'", "''");
            string strSQL = string.Format(@"SELECT obj_uid FROM nav_overview WHERE (1=1) AND obj_path = '{0}'"
                                 , strPath
            );

            try
            {
                object obj = SQL.ExecuteScalar(strSQL);

                if (obj == null)
                    strRetVal = "AND (1 = 2) ";
                else
                {
                    strRetVal = "AND obj_parent_uid = '" + System.Convert.ToString(obj) + "' ";
                }

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            return strRetVal;
            // } // End Lock
        }


        protected override Errno OnCreateHandle(string path, Mono.Fuse.OpenedPathInfo info, FilePermissions mode)
        {
            System.Console.WriteLine("create");
            /*
            int fd = Syscall.open (basedir + path, info.OpenFlags, mode);
            if (fd == -1)
                return Stdlib.GetLastError ();

            info.Handle = (IntPtr)fd;
            */
            return 0;
        }

        // /mnt/fuse/1081 Montpreveyres/

        protected override Errno OnOpenHandle(string path, Mono.Fuse.OpenedPathInfo fi)
        {
            // Trace.WriteLine (string.Format ("(OnOpen {0} Flags={1})", path, fi.OpenFlags));

            //if (path != hello_path && path != data_path && path != data_im_path) return Errno.ENOENT;

            // if (path == data_im_path && !have_data_im) return Errno.ENOENT;

            if (fi.OpenAccess != OpenFlags.O_RDONLY)
                return Errno.EACCES;

            return 0;
        }


        protected override Errno OnReadHandle(string path, Mono.Fuse.OpenedPathInfo fi, byte[] buf, long offset, out int bytesWritten)
        {
            // Trace.WriteLine ("(OnRead {0})", path);
            bytesWritten = 0;
            long szeBufferSize = buf.LongLength;

            byte[] source = System.Text.Encoding.UTF8.GetBytes("Hello World!\n");

            if (offset < source.LongLength)
            {
                if (offset + szeBufferSize > source.LongLength)
                    szeBufferSize = source.LongLength - offset;

                // System.Buffer.BlockCopy(source, (int)offset, buf, 0, szeBufferSize);
                System.Array.Copy(source, offset, buf, 0, szeBufferSize);
            }
            else
                szeBufferSize = 0;

            bytesWritten = (int)szeBufferSize;
            return 0;
        }



        protected override Errno OnCreateDirectory(string directory, FilePermissions mode)
        {
            return base.OnCreateDirectory(directory, mode);
        }


        protected override unsafe Errno OnWriteHandle(string path, Mono.Fuse.OpenedPathInfo info,
            byte[] buf, long offset, out int bytesWritten)
        {
            Errno e = 0;

            using (System.IO.FileStream fs = new System.IO.FileStream("/mnt/repo/lol.txt", System.IO.FileMode.Append))
            {
                fs.Write(buf, (int)offset, buf.Length);
            }

            bytesWritten = buf.Length;
            return e;
        }


        protected override Errno OnReadDirectory(string directory,
                                                  Mono.Fuse.OpenedPathInfo info,
                                                  out IEnumerable<Mono.Fuse.DirectoryEntry> names)
        {

            // if (directory != "/")
            // {
            // 	names = null;
            // 	return Errno.ENOENT;
            // } // End if (directory != "/")

            names = ListNames(directory);
            return 0;
        }


        protected IEnumerable<Mono.Fuse.DirectoryEntry> ListNames(string directory)
        {
            List<Mono.Fuse.DirectoryEntry> ls = new List<Mono.Fuse.DirectoryEntry>();

            string strSQL = @"
SELECT -- obj_parent_uid, COUNT(*) AS cnt 
	 obj_uid
	,obj_parent_uid
	,obj_no
	,obj_text
	,obj_path
	,isdir 
FROM nav_overview 
WHERE (1=1) 
{0} 
-- AND obj_parent_uid IS NULL 
-- AND obj_path = '/Adelboden'
-- AND obj_parent_uid = 'f7c360ca-3da7-4e25-bbca-2ab3cb826ffc' 
-- AND obj_parent_uid = '7f58afb4-0e18-4086-834a-87e09ac6adf7' 
-- AND obj_text IS NULL 
-- AND obj_text ILIKE '%Le Rond%'
-- GROUP BY obj_parent_uid ORDER BY cnt DESC 
ORDER BY obj_text 
";


            strSQL = string.Format(strSQL, GetParentCondition(directory));


            System.Data.DataTable dt = SQL.GetDataTable(strSQL);

            foreach (System.Data.DataRow dr in dt.Rows)
            {
                string str = System.Convert.ToString(dr["obj_text"]);

                if (str == null)
                {
                    LogDetails("skipping null entry");
                    continue;
                }

                int iPos = str.IndexOf("/");

                // LogDetails("adding " + str);
                if (iPos == -1)
                    ls.Add(new Mono.Fuse.DirectoryEntry(str));
                else
                {
                    LogDetails(@"skipped entry containing ""/"" to prevent fatal error");
                    LogDetails("Entry text: \"{0}\"", str);
                }

                // LogDetails("added " + str);
            } // Next name

            return ls;
        }


        public static void LogDetails(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }


        protected override Errno OnGetPathStatus(string path, out Stat stbuf)
        {
            stbuf = new Stat();

            if (path == "/")
            {
                stbuf.st_mode = NativeConvert.FromUnixPermissionString("dr-xr-xr-x");
                stbuf.st_nlink = 1;
                return 0;
            } // End if (path == "/")

            //if (!files.Contains (path) && !folders.Contains (path))			
            // if(!files.Any(x => path.Contains(x)) && !folders.Any(x => path.Contains(x)))
            // 	return Errno.ENOENT;

            // /.Trash-0

            List<string> tra = new List<string>();
            tra.Add("/.Trash-0");
            tra.Add("/.Trash");
            tra.Add("/.hidden");

            if (tra.Contains(path, System.StringComparer.InvariantCultureIgnoreCase))
                return 0;

            bool? isf = IsFolder(path);
            if (isf.HasValue)
            {
                if (isf.Value)
                {
                    stbuf.st_mode = NativeConvert.FromUnixPermissionString("dr-xr-xr-x");
                    stbuf.st_nlink = 1;
                }
                else
                {
                    stbuf.st_mode = NativeConvert.FromUnixPermissionString("-r--r--r--");
                    byte[] source = System.Text.Encoding.UTF8.GetBytes("Hello World!\n");
                    stbuf.st_size = source.LongLength;
                }
            }
            else
            {
                System.Console.WriteLine("Rogue path: \"{0}\"", path);
                stbuf.st_mode = NativeConvert.FromUnixPermissionString("dr-xr-xr-x");
                stbuf.st_nlink = 1;
            }

            return 0;
        }


        public static void TestFS(string[] args)
        {
            // int bla = System.IntPtr.Size *8;
            // System.Console.WriteLine (bla);

            if (args == null || args.Length == 0)
                throw new System.ArgumentException("No argument passed to Sub Main");

            if (!System.IO.Directory.Exists(args[0]))
                throw new System.IO.DirectoryNotFoundException(args[0]);

            using (SqlFS fs = new SqlFS(args))
            {
                fs.Start();
            } // End SqlFS 

        } // End Sub Main


    } // End Class SimpleFS


} // End Namespace FuseExample
