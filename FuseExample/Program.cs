



using System.Linq;
using System.Collections.Generic;


using FuseExample;

namespace FuseExample
{


    class MainClass
    {

		public void roflmao()
		{}


		public void rofl()
		{
			var tsk0 = new System.Threading.Tasks.Task (roflmao);

			System.Threading.Tasks.TaskFactory tf = new System.Threading.Tasks.TaskFactory ();

			System.Threading.Tasks.Task tsk1 = tf.StartNew (roflmao);
			// tsk1.ContinueWith (roflmao); // Execute another Async task when the current task is done:

			System.Threading.Tasks.Task.WaitAll (tsk0, tsk1);
			// System.Threading.Tasks.Parallel.For (1, 10, roflmao);
		}



		public class DebugTraceListener : System.Diagnostics.TraceListener
		{

			private static void MyWrite(object data)
			{
				if (data != null)
					System.Console.Write (data.ToString ());
				else
					System.Console.Write ("data is NULL");
			}


			private static void MyWriteLine(object data)
			{
				if (data != null)
					System.Console.WriteLine (data.ToString ());
				else
					System.Console.WriteLine ("data is NULL");
			}


			public override void Write(object o)
			{
				MyWrite (o);
			}


			public override void Write(string message)
			{
				MyWrite (message);
			}

			public override void WriteLine(object o)
			{
				MyWriteLine(o);
			}

			public override void WriteLine(string message)
			{
				MyWriteLine(message);
			}


			public override void Write(object o, string category)
			{
				MyWrite(category + ":");
				MyWrite(o);
			}

			public override void Write(string message, string category)
			{
				MyWrite(category + ":");
				MyWrite(message);
			}

			public override void WriteLine(object o, string category)
			{
				MyWriteLine(category + ":");
				MyWriteLine(o);
			}

			public override void WriteLine(string message, string category)
			{
				MyWriteLine(category + ":");
				MyWriteLine(message);
			}


		}


        public static void Main(string[] args)
		{
			System.Diagnostics.ConsoleTraceListener consoleTracer;
			consoleTracer = new System.Diagnostics.ConsoleTraceListener(true);
			consoleTracer.Name = "Mono.Fuse.DebugInfo";
			// System.Diagnostics.Trace.Listeners.Add(consoleTracer);

			System.Diagnostics.Trace.Listeners.Add(new DebugTraceListener());




			 
			string[] argsMountPoint = new string[] { "/mnt/fuse" }; //, "-o", "use_ino" };
            string[] argsRedirectFS = new string[] { "/mnt/fuse", "/root/Downloads/fuse-tutorial-2014-06-12/html" };

            System.Console.WriteLine("Attempting to mount filesystem ...");
            // SimpleFS.TestFS(argsMountPoint);
            // HelloFS.TestFS(argsMountPoint);
            // RedirectFS.TestFS(argsRedirectFS);
            // RedirectFHFS.TestFS(argsRedirectFS);
            // TypeNavigatorFS.TestFS(argsMountPoint);

            SqlFS.TestFS(argsMountPoint);


            // Calm warnings
            System.Console.WriteLine(System.Environment.NewLine, argsMountPoint, argsRedirectFS);
            System.Console.WriteLine("Unmounted !");
        }


    }


}
