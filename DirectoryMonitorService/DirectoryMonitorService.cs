using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace DirectoryMonitorService
{
    public partial class DirectoryMonitorService : ServiceBase
    {
        // Listens to the file system change notifications and raises events when a directory, or file in a directory, changes.
        FileSystemWatcher fileSystemWatcher;
        System.Timers.Timer oTimer;
        string strLogFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        string strFolderPath = ConfigurationManager.AppSettings["folderPath"];

        public DirectoryMonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            fileSystemWatcher = new FileSystemWatcher(strFolderPath)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };
            oTimer = new System.Timers.Timer();
            oTimer.AutoReset = true;
            oTimer.Elapsed += new System.Timers.ElapsedEventHandler(oTimer_Elapsed);
            oTimer.Enabled = true;


            fileSystemWatcher.Created += DirectoryChanged; // Event Handler Method called when folder Created
            fileSystemWatcher.Deleted += DirectoryChanged; // Event Handler Method called when folder Deleted
            fileSystemWatcher.Changed += DirectoryChanged; // Event Handler Method called when folder Changed
            fileSystemWatcher.Renamed += DirectoryChanged; // Event Handler Method called when folder Renamed

            EventLog.WriteEntry("Directory Monitor Service Started.");

        }

        private void oTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //logic to do something
            File.AppendAllText(strLogFolderPath + "log1.txt", DateTime.Now.ToString()+ " put your logic here.\n");
        }

        private void DirectoryChanged(object sender, FileSystemEventArgs e)
        {
            // log event when folder Created, Deleted, Changed, or Renamed
            var msg = $"{e.ChangeType} at {DateTime.Now}\t{e.FullPath}\n";
            File.AppendAllText(strLogFolderPath + "log.txt", msg);
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Directory Monitor Service Stopped.");
            oTimer.Enabled= false;
        }
    }
}
