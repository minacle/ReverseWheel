using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Windows;
using System.Windows.Data;

namespace ReverseWheel {
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private ManagementEventWatcher watcher;
        
        public IEnumerable<Mouse> Mice {
            get;
            private set;
        }

        public MainWindow() {
            watcher = new ManagementEventWatcher();
            watcher.Query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent");
            watcher.EventArrived += new EventArrivedEventHandler((object sender, EventArrivedEventArgs e) => Dispatcher.Invoke(() => Refresh()));
            watcher.Start();
            InitializeComponent();
        }

        private void MainWindow_Closed(object sender, EventArgs e) {
            if (MessageBox.Show("You must restart your computer to apply these changes.\nRestart now?", string.Empty, MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                ManagementClass os = new ManagementClass("Win32_OperatingSystem");
                os.Get();
                os.Scope.Options.EnablePrivileges = true;
                ManagementBaseObject mboShutdownParams = os.GetMethodParameters("Win32Shutdown");
                mboShutdownParams["Flags"] = "6";
                mboShutdownParams["Reserved"] = "0";
                ManagementBaseObject obj;
                foreach (ManagementObject instance in os.GetInstances())
                    obj = instance.InvokeMethod("Win32Shutdown", mboShutdownParams, null);

            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e) {
            if (watcher != null)
                watcher.Dispose();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            Refresh();
        }

        private void MiceList_SourceUpdated(object sender, DataTransferEventArgs e) {
            Debug.Print(e.OriginalSource.ToString());
        }

        private void Refresh() {
            var mice = new List<Mouse>();
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE ClassGuid = \"{4d36e96f-e325-11ce-bfc1-08002be10318}\""))
                foreach (var device in searcher.Get())
                    mice.Add(new Mouse(device.GetPropertyValue("Caption").ToString(), device.GetPropertyValue("DeviceID").ToString()));
            Mice = mice;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Mice"));
        }
    }
}
