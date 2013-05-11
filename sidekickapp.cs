using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
 
namespace SyncDeploy
{
    class Program
    {
        static string path = null; 
        static FileSystemWatcher watcher;
 
        static void Main(string[] args)
        {
            path = Directory.GetCurrentDirectory();
            watcher = new FileSystemWatcher(path, "*.*");
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.Created += new FileSystemEventHandler(watcher_Changed);
            Console.WriteLine("Watching " + path + " for changes, press Enter to stop...");
            Shell("tutorial");
            Console.ReadLine();
            
        }
 
        static void Shell(params string[] args)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("ruby", "dotnet.watchr.rb " + string.Join(" ", args));
            processStartInfo.UseShellExecute = false;
            processStartInfo.ErrorDialog = false;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            Process process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo = processStartInfo;
            process.OutputDataReceived += (sender, args1) => System.Console.WriteLine(args1.Data);
            process.ErrorDataReceived += (sender, args2) => System.Console.WriteLine(args2.Data);
 
            bool processStarted = process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
 
            process.WaitForExit();
            System.Console.WriteLine("---");
        }
 
        static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            watcher.EnableRaisingEvents = false;
            var relativeFile = e.FullPath.Replace(Directory.GetCurrentDirectory(), "");
            System.Console.WriteLine("Changed: " + relativeFile);
            Shell("file_changed", relativeFile);
            watcher.EnableRaisingEvents = true;
        }
    }
}
