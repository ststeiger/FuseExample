
namespace FuseExample
{


    class MainClass
    {


        public static void Main(string[] args)
        {
			System.Diagnostics.ConsoleTraceListener consoleTracer;
			consoleTracer = new System.Diagnostics.ConsoleTraceListener(true);
			consoleTracer.Name = "mainConsoleTracer";
			System.Diagnostics.Trace.Listeners.Add(consoleTracer);

            string[] argsMountPoint = new string[] { "/mnt/fuse" };
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
