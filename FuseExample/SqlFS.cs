
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


		protected override Errno OnGetHandleStatus (string path, Mono.Fuse.OpenedPathInfo info, out Stat buf)
		{
			System.Console.WriteLine ("OnGetHandleStatus (\"{0}\")", path);
			buf = new Stat ();

			string strSQL =  string.Format( @"ui
SELECT 
	 FS_Id AS FileHandle 
	,0 AS Size
	--,FS_UnixPermissions
	,'-r--r--r--' AS PermissionString 
	,FS_OwnerId AS OwnerId
	,FS_OwnerGroupId AS OwnerPrimaryGroupId
	--,FS_IsReadOnly 
	--FS_IsFolder 
FROM T_Filesystem 
WHERE FS_LowerCasePath = '{0}'
;
", olc(path).Replace("'","''")
);

			cHandleStatusInfo HandleStatusInfo = SQL.GetClass<cHandleStatusInfo> (strSQL);

			// only the st_uid, st_gid, st_size, and st_mode

			buf.st_uid = HandleStatusInfo.OwnerId;
			buf.st_gid = HandleStatusInfo.OwnerPrimaryGroupId;

			buf.st_ino = HandleStatusInfo.FileHandle;
			buf.st_size = HandleStatusInfo.Size;
			buf.st_mode = NativeConvert.FromUnixPermissionString(HandleStatusInfo.PermissionString);


			// int r = Syscall.fstat ((int)info.Handle, out buf);
			// if (r == -1) return Stdlib.GetLastError ();

			return 0;
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


		protected cEntityInfo GetEntityInfo(string strPath)
        {
            //lock (objLock)
            //{
			cEntityInfo EntityInfo = null;

			if (string.IsNullOrEmpty (strPath))
				return EntityInfo;

            try
            {
				// object obj = SQL.ExecuteScalar(strSQL);
				// if (obj != null) EntityInfo.IsFolder = System.Convert.ToBoolean(obj);
				// else EntityInfo.IsInvalid = true;

				string strSQL = @"
SELECT 
	 FS_isFolder AS IsFolder 
	,CASE WHEN FS_Id <> FS_Target_FS_Id THEN 1 ELSE 0 END AS isLink 
FROM T_Filesystem WHERE FS_LowerCasePath = '{0}';";
				strSQL = string.Format(strSQL, olc(strPath).Replace("'", "''"));
				EntityInfo = SQL.GetClass<cEntityInfo>(strSQL);

				if (EntityInfo == null)
					EntityInfo = new cEntityInfo(true);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
				EntityInfo = new cEntityInfo(true);
            }

			return EntityInfo;
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
			string strSQL = string.Format(@"SELECT fs_id FROM T_Filesystem 
WHERE (1=1) AND FS_LowerCasePath = '{0}' "
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


		protected string mountPoint = "/mnt/fuse";

		// ln -s /path/to/file-name link-name
		protected override Errno OnCreateSymbolicLink (string to, string from)
		{
			// int r = Syscall.symlink (from, basedir + to);
			// if (r == -1) return Stdlib.GetLastError ();
			System.Console.WriteLine ("Symlink from {0} to {1}", from, to);


			if (to.StartsWith (mountPoint, System.StringComparison.InvariantCultureIgnoreCase))
				to = to.Substring (mountPoint.Length);

			to = to.TrimEnd (System.IO.Path.DirectorySeparatorChar);
			from = from.TrimEnd (System.IO.Path.DirectorySeparatorChar);

			string parentPath = System.IO.Path.GetDirectoryName (from);
			string dir = System.IO.Path.GetFileName(from);

			string strSQL = @"
INSERT INTO T_Filesystem(FS_Id, FS_Target_FS_Id, FS_Parent_FS_Id, FS_Text, FS_Path, FS_LowerCasePath, FS_isFolder) 
SELECT 
	 (SELECT COALESCE(MAX(FS_Id), 0) + 1 AS newid FROM T_Filesystem) AS FS_Id 
    ,(SELECT FS_Id AS newid FROM T_Filesystem WHERE FS_LowerCasePath = '{4}' ) AS FS_Target_FS_Id
	,(SELECT FS_Id FROM T_Filesystem WHERE FS_LowerCasePath = '{2}') AS FS_Parent_FS_Id 
	,'{0}' AS FS_Text 
	,'{1}/{0}' AS FS_Path 
	,'{3}' AS FS_LowerCasePath 
     -- Assuming target is on the same filesystem
	,(SELECT FS_isFolder AS newid FROM T_Filesystem WHERE FS_LowerCasePath = '{4}' ) AS FS_isFolder
";

			strSQL = string.Format(strSQL
				,dir.Replace("'","''") // FS_Text
				,parentPath.Replace("'","''") // FS_Path
				,olc(parentPath).Replace("'","''") // FS_Path
				,olc(parentPath).Replace("'","''") + "/" +  olc(dir).Replace("'","''")// FS_LowerCasePath
				,olc(to).Replace("'","''")
			);

			System.Console.WriteLine(strSQL);
			SQL.ExecuteNonQuery(strSQL);

			return 0;
		}

		protected override Errno OnReadSymbolicLink (string path, out string target)
		{
			System.Console.WriteLine ("OnReadSymbolicLink for \"{0}\"", path);
			path = olc (path);

			string strSQL = string.Format (@"SELECT FS_Path FROM T_Filesystem 
WHERE FS_ID = (SELECT FS_Target_FS_Id FROM T_Filesystem WHERE FS_LowerCasePath = '{0}') 
", path.Replace("'","''")
);

			object obj = SQL.ExecuteScalar (strSQL);

				// mountPoint may not end on /
			target = mountPoint + System.Convert.ToString(obj);
			System.Console.WriteLine (target);

			return 0;
		}


		protected override Errno OnTruncateFile (string path, long size)
		{
			System.Console.WriteLine ("OnTruncateFile for \"{0}\"", path);
			// int r = Syscall.truncate (basedir + path, size);
			// if (r == -1) return Stdlib.GetLastError ();

			return 0;
		}

		protected override Errno OnTruncateHandle (string path, Mono.Fuse.OpenedPathInfo info, long size)
		{
			System.Console.WriteLine ("OnTruncateHandle for \"{0}\"", path);

			// int r = Syscall.ftruncate ((int)info.Handle, size);
			// if (r == -1) return Stdlib.GetLastError ();

			return 0;
		}


		protected override Errno OnRemoveFile (string path)
		{
			System.Console.WriteLine ("OnRemoveFile for \"{0}\"", path);

			string strSQL = @"DELETE FROM T_Filesystem WHERE FS_LowerCasePath = '{0}';";
			strSQL = string.Format(strSQL, olc(path).Replace("'","''"));
			SQL.ExecuteNonQuery (strSQL);

			// int r = Syscall.unlink (basedir + path);
			// if (r == -1) return Stdlib.GetLastError ();
			return 0;
		}


		protected override Errno OnRemoveDirectory (string path)
		{
			System.Console.WriteLine ("OnRemoveDirectory for \"{0}\"", path);

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
			System.Console.WriteLine ("OnWriteHandle for \"{0}\"", path);

            Errno e = 0;

            using (System.IO.FileStream fs = new System.IO.FileStream("/mnt/repo/lol.txt", System.IO.FileMode.Append))
            {
                fs.Write(buf, (int)offset, buf.Length);
            }

            bytesWritten = buf.Length;
            return e;
        }


		protected override Errno OnOpenDirectory (string path, Mono.Fuse.OpenedPathInfo info)
		{
			System.Console.WriteLine ("OnOpenDirectory (\"{0}\")", path);

			// System.IntPtr dp = Syscall.opendir (basedir + path);

			// if (dp == System.IntPtr.Zero) return Stdlib.GetLastError ();
			// info.Handle = dp;

			return 0;
		}

		protected override Errno OnReleaseDirectory (string path, Mono.Fuse.OpenedPathInfo info)
		{
			System.Console.WriteLine ("OnReleaseDirectory (\"{0}\")", path);

			// System.IntPtr dp = (System.IntPtr)info.Handle;
			// Syscall.closedir (dp);
			return 0;
		}

        protected override Errno OnReadDirectory(string directory,
                                                  Mono.Fuse.OpenedPathInfo info,
                                                  out IEnumerable<Mono.Fuse.DirectoryEntry> names)
        {
			System.Console.WriteLine("OnReadDirectory (\"{0}\")", directory);
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


		protected override Errno OnGetFileSystemStatus (string path, out Statvfs stbuf)
		{
			System.Console.WriteLine("OnGetFileSystemStatus (\"{0}\")", path);
				// int r = Syscall.statvfs (basedir + path, out stbuf);
			// if (r == -1) return Stdlib.GetLastError ();

			stbuf = new Statvfs ();

			// stbuf.f_bavail stbuf.f_bfree
			stbuf.f_namemax = 4000;
			stbuf.f_flag = MountFlags.ST_RDONLY|MountFlags.ST_NODEV|MountFlags.ST_NOATIME|MountFlags.ST_NODIRATIME;

			return 0;
		}


        protected override Errno OnGetPathStatus(string path, out Stat stbuf)
        {
			System.Console.WriteLine("OnGetPathStatus for path \"{0}\"", path);

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
                
			cEntityInfo ei = GetEntityInfo (path);
			if (!ei.IsInvalid)
			{
				if (ei.IsLink)
				{
					stbuf.st_mode = NativeConvert.FromUnixPermissionString("lrwxrwxrwx");
					stbuf.st_nlink = 1;
				}
				else if (ei.IsFolder)
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
                // stbuf.st_mode = NativeConvert.FromUnixPermissionString("dr-xr-xr-x");
				// stbuf.st_nlink = 1;
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
