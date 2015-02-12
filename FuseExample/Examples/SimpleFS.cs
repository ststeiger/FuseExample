
using System.Collections.Generic;


// using Mono.Fuse;
using Mono.Unix.Native;

using System.Linq;


// http://www.jprl.com/Projects/mono-fuse/docs/Mono.Fuse/FileSystem.html
namespace FuseExample
{


	// apt-get install libmono-fuse-cil
	// gmcs -r:Mono.Fuse.dll -r:Mono.Posix.dll SimpleFS.cs
	public class SimpleFS : Mono.Fuse.FileSystem
	{

		private List<string> files = new List<string> ();
		private List<string> folders = new List<string> ();


		public SimpleFS (string[] args) : base (args)
		{
			files.Add ("/file1");
			files.Add ("/file2");
			files.Add ("/file3");
			files.Add ("/файл4");

			folders.Add ("/folder1");
			folders.Add ("/folder2");
			folders.Add ("/folder3");
			folders.Add ("/фолдер4");
		}



		protected override Errno OnReadDirectory (string directory, 
		                                          Mono.Fuse.OpenedPathInfo info,
		                                          out IEnumerable<Mono.Fuse.DirectoryEntry> names)
		{

			// if (directory != "/")
			// {
			// 	names = null;
			// 	return Errno.ENOENT;
			// } // End if (directory != "/")

			names = ListNames (directory);
			return 0;
		}


		protected IEnumerable<Mono.Fuse.DirectoryEntry> ListNames (string directory)
		{
			List<Mono.Fuse.DirectoryEntry> ls = new List<Mono.Fuse.DirectoryEntry> ();

			// if (directory != "/") return ls;

			foreach (string name in files)
			{
				// Mono.Fuse.DirectoryEntry de = new Mono.Fuse.DirectoryEntry (name.Substring (1));
				ls.Add (new Mono.Fuse.DirectoryEntry (name.Substring (1)));
				// yield return new Mono.Fuse.DirectoryEntry (name.Substring (1));
			} // Next name

			foreach (string name in folders)
			{
				ls.Add (new Mono.Fuse.DirectoryEntry (name.Substring (1)));
			}

			return ls;
		}


		protected override Errno OnGetPathStatus (string path, out Stat stbuf)
		{
			stbuf = new Stat ();

			if (path == "/")
			{
				stbuf.st_mode = NativeConvert.FromUnixPermissionString ("dr-xr-xr-x");
				stbuf.st_nlink = 1;
				return 0;
			} // End if (path == "/")

			//if (!files.Contains (path) && !folders.Contains (path))			
			if(!files.Any(x => path.Contains(x)) && !folders.Any(x => path.Contains(x)))
				return Errno.ENOENT;

			//if (files.Contains (path))
			if(files.Any(x => path.Contains(x)))
				stbuf.st_mode = NativeConvert.FromUnixPermissionString ("-r--r--r--");
			else if(folders.Any(x => path.Contains(x))) //if (folders.Contains (path))
			{
				stbuf.st_mode = NativeConvert.FromUnixPermissionString ("dr-xr-xr-x");
				//stbuf.st_nlink = 1;
			} 
			else
			{
				stbuf.st_mode = NativeConvert.FromUnixPermissionString ("dr-xr-xr-x");
			}
			return 0;
		}

		public enum cputype_t : int
		{
			 x86
			,amd64
			,ia64
			,arm
			,sparc
			,alpha
			,mips
		}


		public class cpuinfo_t
		{
			public bool IsSet;
			private cputype_t m_cputype;

			public cpuinfo_t()
			{
				this.IsSet = false;
			}


			public cputype_t cputype
			{
				get{ return this.m_cputype;}
				set{ this.IsSet = true; this.m_cputype = value; }
			}

			public int bitness;
		}


		public static cpuinfo_t GetCpuKind()
		{
			// http://stackoverflow.com/questions/1542213/how-to-find-the-number-of-cpu-cores-via-net-coa

			cpuinfo_t cpuinfo = new cpuinfo_t ();

			if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
			{
				Mono.Unix.Native.Utsname results;
				int res = Mono.Unix.Native.Syscall.uname(out results);
				if(res == 0 && results != null && results.machine != null)
				{
					if (results.machine.StartsWith ("arm", System.StringComparison.InvariantCultureIgnoreCase))
						cpuinfo.cputype = cputype_t.arm;
				}

			}

			if (!cpuinfo.IsSet)
			{
				//lscpu
				// http://stackoverflow.com/questions/767613/identifying-the-cpu-architecture-type-using-c-sharp
				System.Reflection.PortableExecutableKinds peKind;
				System.Reflection.ImageFileMachine machine;
				typeof(object).Module.GetPEKind(out peKind, out machine);
				if (machine == System.Reflection.ImageFileMachine.AMD64)
					cpuinfo.cputype = cputype_t.amd64;

				if (machine == System.Reflection.ImageFileMachine.I386)
					cpuinfo.cputype = cputype_t.x86;

				if (machine == System.Reflection.ImageFileMachine.IA64)
					cpuinfo.cputype = cputype_t.ia64;

				cpuinfo.cputype = cputype_t.arm;
			}

			cpuinfo.bitness = System.IntPtr.Size * 8;

			return cpuinfo;
		}


		public static void TestFS (string[] args)
		{
			// int bla = System.IntPtr.Size *8;
			// System.Console.WriteLine (bla);

			if (args == null || args.Length == 0)
				throw new System.ArgumentException ("No argument passed to Sub Main");

			if (!System.IO.Directory.Exists (args [0]))
				throw new System.IO.DirectoryNotFoundException (args [0]);

			using (SimpleFS fs = new SimpleFS (args))
			{
				fs.Start ();
			} // End SimpleFS 

		} // End Sub Main


	} // End Class SimpleFS


} // End Namespace FuseExample
