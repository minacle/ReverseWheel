using Microsoft.Win32;

#pragma warning disable CS0659

namespace ReverseWheel {
    public class Mouse {
        private RegistryKey home;
        private RegistryKey deviceParameters;
        
        public string Caption {
            get;
            private set;
        }

        public string DeviceID {
            get;
            private set;
        }

        public int DeviceIcon {
            get;
            private set;
        }

        public bool IsFlipFlopWheelAvailable {
            get {
                return FlipFlopWheel != null;
            }
        }

        public bool IsFlipFlopHScrollAvailable {
            get {
                return FlipFlopHScroll != null;
            }
        }

        public bool? FlipFlopWheel {
            get {
                var value = deviceParameters.GetValue("FlipFlopWheel");
                if (value == null)
                    return null;
                else if ((int)value == 0)
                    return false;
                return true;
            }
            set {
                int result = -1;
                if (value == true)
                    result = 1;
                else if (value == false)
                    result = 0;
                if (result >= 0)
                    deviceParameters.SetValue("FlipFlopWheel", result, RegistryValueKind.DWord);
                deviceParameters.Flush();
            }
        }

        public bool? FlipFlopHScroll {
            get {
                var value = deviceParameters.GetValue("FlipFlopHScroll");
                if (value == null)
                    return null;
                else if ((int)value == 0)
                    return false;
                return true;
            }
            set {
                int result = -1;
                if (value == true)
                    result = 1;
                else if (value == false)
                    result = 0;
                if (result >= 0)
                    deviceParameters.SetValue("FlipFlopHScroll", result, RegistryValueKind.DWord);
                deviceParameters.Flush();
            }
        }

        public Mouse(string caption, string deviceID) {
            Caption = caption;
            DeviceID = deviceID;
            DeviceIcon = -2212;
            home = Registry.LocalMachine.OpenSubKey("SYSTEM").OpenSubKey("CurrentControlSet").OpenSubKey("Enum");
            var path = deviceID.Split('\\');
            foreach (var keyName in path)
                home = home.OpenSubKey(keyName);
            deviceParameters = home.OpenSubKey("Device Parameters", RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.WriteKey | System.Security.AccessControl.RegistryRights.ReadKey);
        }

        public override bool Equals(object obj) {
            if (GetType() == obj.GetType())
                return DeviceID == (obj as Mouse).DeviceID;
            return base.Equals(obj);
        }

        public override string ToString() {
            return string.Format("\"{0}\" {1}", Caption, DeviceID);
        }
    }
}
