
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Mono.Unix.Native;
using Mono.Fuse;


namespace FuseExample
{

	// http://tirania.org/blog/archive/2006/Sep-01.html
	// Refurbished from http://tirania.org/tmp/fuse.cs
	public class TypeNavigatorFS : FileSystem
	{
		Dictionary<string,Assembly> assemblies;
		//Dictionary<string,string[]> assembly_results = new Dictionary<string,string[]> ();
		Dictionary<string, List<DirectoryEntry>> assembly_results = new Dictionary<string,List<DirectoryEntry>> ();


		public TypeNavigatorFS()
		{
			assemblies = new Dictionary<string,Assembly> ();

			Assembly a = Assembly.Load ("mscorlib");
			assemblies [a.GetName ().Name] = a;
		}


		//protected override Errno OnReadDirectory (string path, [Out] out string[] paths, OpenedFileInfo fi)
		protected override Errno OnReadDirectory(string path, 
		                                          Mono.Fuse.OpenedPathInfo fi,
		                                          out IEnumerable<Mono.Fuse.DirectoryEntry> paths)
		{
			List<Mono.Fuse.DirectoryEntry> ls = new List<DirectoryEntry> ();

			if (path == "/")
			{
				ls.Add (new DirectoryEntry ("."));
				ls.Add (new DirectoryEntry (".."));
							
				foreach (string val in assemblies.Keys)
				{
					ls.Add (new DirectoryEntry (val));
					Console.WriteLine ("  {0}", val);
				} // Next val

				paths = ls;
				return 0;
			} // End if (path == "/")

			path = path.Substring (1);
			string[] elements = path.Split (new char [] { '/' });

			try
			{
				if (elements.Length == 1)
				{
					string key = elements [0];

					Assembly ass = null;

					if (assemblies.ContainsKey (key))
						ass = assemblies [key];

					if (ass == null)
					{
						paths = ls;
						return Errno.ENOENT;
					}

					if (assembly_results.ContainsKey (key))
					{
						ls.AddRange(assembly_results [key]);
					}
					else
					{
						Type[] types = ass.GetTypes ();

						ls.Add (new DirectoryEntry ("."));
						ls.Add (new DirectoryEntry (".."));

						foreach (Type t in types)
						{
							ls.Add (new DirectoryEntry (t.FullName));
						}

						assembly_results [key] = ls;
					} // End else of if (assembly_results.ContainsKey (key))

					paths = ls;
					return 0;
				}

				if (elements.Length == 2)
				{
					Assembly ass = null; 
					if (assemblies.ContainsKey (elements [0]))
						ass = assemblies [elements [0]];

					if (ass != null)
					{
						Type t = ass.GetType (elements [1]);
						if (t != null)
						{
							MemberInfo[] mi = t.GetMembers ();

							ls.Add (new DirectoryEntry ("."));
							ls.Add (new DirectoryEntry (".."));


							foreach (MemberInfo m in mi)
							{
								ls.Add (new DirectoryEntry (m.Name));
							} // Next m

							paths = ls;
							return 0;
						} // End if (t != null)

					} // End if (ass != null)

				} // End if (elements.Length == 2)

			}
			catch (Exception e)
			{
				Console.WriteLine ("Exception: {0}", e.ToString ());
			}

			paths = ls;
			return Errno.ENOENT;
		} // End Function OnReadDirectory


		// protected override Errno OnGetFileAttributes (string path, ref Stat stbuf)
		protected override Errno OnGetPathStatus(string path, out Stat stbuf)
		{
			int sep = 0;

			foreach (char c in path)
			{
				if (c == '/')
					sep++;
			}

			stbuf = new Stat ();

			if (sep < 3)
			{
				stbuf.st_mode = FilePermissions.S_IFDIR | NativeConvert.FromOctalPermissionString ("0755");
				stbuf.st_nlink = 1;
			}
			else
			{
				stbuf.st_mode = FilePermissions.S_IFREG | NativeConvert.FromOctalPermissionString ("0444");
				stbuf.st_nlink = 1;
			}
			stbuf.st_size = 0;

			return 0;
		} // End Function OnGetPathStatus


		public static void TestFS(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine ("Error: must specify a directory to mount at");
				return;
			}

			using (TypeNavigatorFS t = new TypeNavigatorFS ())
			{
				t.MountPoint = args [0];
				t.Start ();
			}

		} // End Sub TestFS


	}


}
