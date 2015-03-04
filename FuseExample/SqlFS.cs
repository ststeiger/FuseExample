
using System.Collections.Generic;


// using Mono.Fuse;
using Mono.Unix.Native;


using System.Linq;


// http://www.jprl.com/Projects/mono-fuse/docs/Mono.Fuse/FileSystem.html
namespace FuseExample
{


    // apt-get install libmono-fuse-cil
    // gmcs -r:Mono.Fuse.dll -r:Mono.Posix.dll SimpleFS.cs
	public class SqlFS : Mono.Fuse.FileSystem
	// public class SqlFS : Mono.Fuse.TestFileSystem
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

		public bool IgnoreCase = true;

		public string olc(string optionalLowerCase)
		{
			if (string.IsNullOrEmpty (optionalLowerCase))
				return optionalLowerCase;

			if (IgnoreCase)
				return optionalLowerCase.ToLowerInvariant ();

			return optionalLowerCase;
		}


		protected override Errno OnAccessPath (string path, AccessModes mask)
		{
			System.Console.WriteLine ("OnAccessPath (path: \"{0}\")", path);
			// return Errno.ENAMETOOLONG; // 
			// return Errno.EROFS; // No write rights
			/*
			// Check accessibility according to bit-pattern in mask
			int r = Syscall.access (basedir + path, mask);
			if (r == -1)
				return Stdlib.GetLastError ();
			*/
			return 0;
		}



        protected bool? IsFolder(string strPath)
        {
            //lock (objLock)
            //{
            bool? bIsFolder = null;

            if (string.IsNullOrEmpty(strPath))
                return bIsFolder;


			strPath = olc(strPath);

            strPath = strPath.Replace("'", "''");
            string strSQL = string.Format(
				@"SELECT FS_isFolder  FROM T_Filesystem WHERE FS_LowerCasePath = '{0}';"
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
				return "AND FS_Parent_FS_Id IS NULL ";

            strPath = strPath.Replace("'", "''");
			string strSQL = string.Format(@"SELECT fs_id FROM T_Filesystem WHERE (1=1) AND FS_LowerCasePath = '{0}'"
                                 , strPath
            );

            try
            {
                object obj = SQL.ExecuteScalar(strSQL);

                if (obj == null)
                    strRetVal = "AND (1 = 2) ";
                else
                {
					strRetVal = "AND FS_Parent_FS_Id = '" + System.Convert.ToString(obj) + "' ";
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
			System.Console.WriteLine("OnCreateHandle (path: \"{0}\")", path);

			string fn = System.IO.Path.GetFileName (path);
			string dn = System.IO.Path.GetDirectoryName (path);

			string strSQL = @"
INSERT INTO T_Filesystem(FS_Id, FS_Target_FS_Id, FS_Parent_FS_Id, FS_Text, FS_Path, FS_LowerCasePath, FS_isFolder) 
SELECT 
	 (SELECT COALESCE(MAX(FS_Id), 0) + 1 AS newid FROM T_Filesystem) AS FS_Id 
    ,(SELECT COALESCE(MAX(FS_Id), 0) + 1 AS newid FROM T_Filesystem) AS FS_Target_FS_Id 
	,(SELECT FS_Id FROM T_Filesystem WHERE FS_LowerCasePath = '{3}') AS FS_Parent_FS_Id 
	,'{0}' -- FS_Text 
	,'{1}' -- FS_Path 
	,'{2}' -- FS_LowerCasePath 
	,0::bit AS FS_isFolder 
";

			strSQL = string.Format(strSQL
				,fn.Replace("'","''") // 0
				,path.Replace("'","''") // 1 
				,olc(path).Replace("'","''") // 2 
				,olc(dn).Replace("'","''") // 3 
			);
			System.Console.WriteLine(strSQL);
			SQL.ExecuteNonQuery(strSQL);

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
			System.Console.WriteLine("OnOpenHandle (path: \"{0}\")", path);
            // Trace.WriteLine (string.Format ("(OnOpen {0} Flags={1})", path, fi.OpenFlags));

            //if (path != hello_path && path != data_path && path != data_im_path) return Errno.ENOENT;

            // if (path == data_im_path && !have_data_im) return Errno.ENOENT;

            if (fi.OpenAccess != OpenFlags.O_RDONLY)
                return Errno.EACCES;

            return 0;
        }


        protected override Errno OnReadHandle(string path, Mono.Fuse.OpenedPathInfo fi, byte[] buf, long offset, out int bytesWritten)
        {
			System.Console.WriteLine("OnReadHandle (path: \"{0}\")", path);

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


		// Move or rename dir/file
		protected override Errno OnRenamePath (string from, string to)
		{
			System.Console.WriteLine("OnRenamePath (from \"{0}\" to \"{1}\")", from, to);

			/*
			int r = Syscall.rename (basedir + from, basedir + to);
			if (r == -1)
				return Stdlib.GetLastError ();
			*/
			return 0;
		}


        protected override Errno OnCreateDirectory(string directory, FilePermissions mode)
        {
			System.Console.WriteLine("OnCreateDirectory (dir \"{0}\")", directory);

			string parentPath = System.IO.Path.GetDirectoryName (directory);
			string dir = System.IO.Path.GetFileName(directory);

			string strSQL = @"
INSERT INTO T_Filesystem(FS_Id, FS_Target_FS_Id, FS_Parent_FS_Id, FS_Text, FS_Path, FS_LowerCasePath, FS_isFolder) 
SELECT 
	 (SELECT COALESCE(MAX(FS_Id), 0) + 1 AS newid FROM T_Filesystem) AS FS_Id 
    ,(SELECT COALESCE(MAX(FS_Id), 0) + 1 AS newid FROM T_Filesystem) AS FS_Target_FS_Id
	,(SELECT FS_Id FROM T_Filesystem WHERE FS_LowerCasePath = '{2}') AS FS_Parent_FS_Id 
	,'{0}' -- FS_Text
	,'{1}/{0}' -- FS_Path
	,'{3}' -- FS_LowerCasePath
	,1::bit AS FS_isFolder

";

			strSQL = string.Format(strSQL
				,dir.Replace("'","''") // FS_Text
				,parentPath.Replace("'","''") // FS_Path
				,olc(parentPath).Replace("'","''") // FS_Path
				,olc(parentPath).Replace("'","''") + "/" +  olc(dir).Replace("'","''")// FS_LowerCasePath
			);
			System.Console.WriteLine(strSQL);
			SQL.ExecuteNonQuery(strSQL);

			// return Errno.ECONNABORTED;
			// mode = NativeConvert.FromUnixPermissionString("dr-xr-xr-x");

            // return base.OnCreateDirectory(directory, mode);
			return 0;
        }


		protected override Errno OnCreateSymbolicLink (string from, string to)
		{
			// int r = Syscall.symlink (from, basedir + to);
			// if (r == -1) return Stdlib.GetLastError ();
			return 0;
		}


		protected override Errno OnRemoveFile (string path)
		{
			string strSQL = @"DELETE FROM T_Filesystem WHERE FS_LowerCasePath = '{0}';";
			strSQL = string.Format(strSQL, olc(path).Replace("'","''"));
			SQL.ExecuteNonQuery (strSQL);

			// int r = Syscall.unlink (basedir + path);
			// if (r == -1) return Stdlib.GetLastError ();
			return 0;
		}


		protected override Errno OnRemoveDirectory (string path)
		{
			string strSQL = @"DELETE FROM T_Filesystem WHERE FS_LowerCasePath = '{0}'";
			strSQL = string.Format(strSQL, olc(path).Replace("'","''"));

			SQL.ExecuteNonQuery (strSQL);

			// int r = Syscall.rmdir (basedir + path);
			// if (r == -1)
			// 	return Stdlib.GetLastError ();
			return 0;
		}


        protected override unsafe Errno OnWriteHandle(string path, Mono.Fuse.OpenedPathInfo info,
            byte[] buf, long offset, out int bytesWritten)
        {
			System.Console.WriteLine("OnWriteHandle");


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
			System.Console.WriteLine("OnReadDirectory");
			directory = olc(directory);


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
SELECT 
	 FS_Id
	--,FS_Parent_FS_Id
	,FS_Text
	--,FS_Path
	-- ,FS_isFolder 
FROM T_Filesystem 
WHERE (1=1) 
{0} 
ORDER BY FS_Text 
";


            strSQL = string.Format(strSQL, GetParentCondition(directory));


            System.Data.DataTable dt = SQL.GetDataTable(strSQL);

            foreach (System.Data.DataRow dr in dt.Rows)
            {
				ulong id = System.Convert.ToUInt64(dr["FS_Id"]);
				string str = System.Convert.ToString(dr["FS_Text"]);
				// string str = System.Convert.ToString(dr["FS_Path"]);
				// str = System.IO.Path.GetFileName (str);


                if (str == null)
                {
                    LogDetails("skipping null entry");
                    continue;
                }

                int iPos = str.IndexOf("/");

                // LogDetails("adding " + str);
				if (iPos == -1)
				{
					Mono.Fuse.DirectoryEntry de = new Mono.Fuse.DirectoryEntry (str);
					de.Stat.st_ino = id;
					ls.Add(de);
				}
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
			// System.Console.WriteLine("OnGetPathStatus for path \"{0}\"", path);

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

            List<string> tra = new List<string>();

			// For move-to-trash
			tra.Add("/.Trash");
			tra.Add("/.Trash/0");
			tra.Add("/.Trash/0/info");
			tra.Add("/.Trash/0/files");
			tra.Add("/.Trash-0");
			tra.Add("/.Trash-0/info");
			tra.Add("/.Trash-0/files");

			// For Locate
			tra.Add("/info");


			if (tra.Exists ( fragment => 
				System.StringComparer.InvariantCultureIgnoreCase.Equals(path,fragment)
				// System.Globalization.CultureInfo.InvariantCulture.CompareInfo.IndexOf(path, fragment, System.Globalization.CompareOptions.IgnoreCase) != -1
			) ||
				path.EndsWith("/.hidden", System.StringComparison.InvariantCultureIgnoreCase)

			)
			{
				System.Console.WriteLine ("Virtual Trash dir: \"{0}\"", path);
				stbuf.st_mode = NativeConvert.FromUnixPermissionString("dr-xr-xr-x");
				stbuf.st_nlink = 1;
				return 0;
			}
                

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

				return Errno.ENOENT;
				// This is wrong...
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
