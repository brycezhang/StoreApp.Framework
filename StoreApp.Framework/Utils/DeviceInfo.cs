using System;
using StoreApp.Framework.Cryptography;
using StoreApp.Framework.Storage;

namespace StoreApp.Framework.Utils
{
    public class DeviceInfo
    {
        /// <summary>
        /// 获取设备ID(32位)
        /// </summary>
        /// <returns>设备ID的MD5值</returns>
        public static string GetDeviceUniqueId()
        {
            var id = AppDataService.Read<string>("DeviceIdKey");

            if (string.IsNullOrEmpty(id) || id.Length != 32)
            {
                string longStr = string.Empty;

#if WINDOWS_PHONE
                longStr = Windows.Phone.System.Analytics.HostInformation.PublisherHostId;
#endif
#if WINDOWS_8
                HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
                var buffer = token.Id;
                using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                {
                    var bytes = new byte[buffer.Length];
                    dataReader.ReadBytes(bytes);
                    longStr = BitConverter.ToString(bytes);
                }
#endif
                id = CryptHepler.Md5(longStr);
                AppDataService.Save("DeviceIdKey", id);
            }

            System.Diagnostics.Debug.WriteLine("设备ID：" + id);
            return id;
        }

        /// <summary>
        /// 网络状态是否可用
        /// </summary>
        public static bool NetworkIsAvailable
        {
            get { return IsConnectedToInternet(); }
        }

        private static bool IsConnectedToInternet()
        {

#if WINDOWS_PHONE

            return Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
#endif

#if WINDOWS_8
            bool connected = false;

            ConnectionProfile cp = NetworkInformation.GetInternetConnectionProfile();

            if (cp != null)
            {
                NetworkConnectivityLevel cl = cp.GetNetworkConnectivityLevel();

                connected = cl == NetworkConnectivityLevel.InternetAccess;
            }

            return NetworkInterface.GetIsNetworkAvailable() && connected;
#endif
        }

        /// <summary>
        /// 分辨率
        /// </summary>
        public static string Resolution
        {
            get
            {
                try
                {

#if WINDOWS_PHONE
                    var content = System.Windows.Application.Current.Host.Content;
                    var scale = (double)content.ScaleFactor / 100;
                    var h = (int)Math.Ceiling(content.ActualHeight * scale);
                    var w = (int)Math.Ceiling(content.ActualWidth * scale);
                    return w + "*" + h;
#endif
#if WINDOWS_8
                    var ret = Window.Current.CoreWindow.Bounds;
                    return ret.Width + "*" + ret.Height;
#endif

                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 获取网络名称（WIFI/LAN/3G/Other）
        /// </summary>
        /// <returns></returns>
        public static string GetNetworkName()
        {
#if WINDOWS_PHONE
            var info = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;

            switch (info)
            {
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandCdma:
                    return "CDMA";
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandGsm:
                    return "GSM";
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Wireless80211:
                    return "WiFi";
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet:
                    return "Ethernet";
                case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None:
                    return "None";
                default:
                    return "Other";
            }
#endif
#if WINDOWS_8
            var profile = System.Net.NetworkInformation.GetInternetConnectionProfile();
            var interfaceType = profile.NetworkAdapter.IanaInterfaceType;

            string networkName;
            switch (interfaceType)
            {
                case 71:
                    networkName = "WiFi";
                    break;
                case 6:
                    networkName = "LAN";
                    break;
                case 243:
                case 244:
                    networkName = "3G";
                    break;
                default:
                    networkName = "Other";
                    break;
            }
            return networkName;
#endif
        }

#if WINDOWS_PHONE
        /// <summary>
        /// 操作系统
        /// </summary>
        public static string Version
        {
            get
            {
                var version = System.Environment.OSVersion.Version;
                Version targetedVersion = new Version(8, 0);
                return version >= targetedVersion ? "WP8" : "WP7";
            }
        }
#endif

#if WINDOWS_8
        
        /// <summary>
        /// 获取当前系统版本号
        /// 摘自：http://attackpattern.com/2013/03/device-information-in-windows-8-store-apps/
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetWindowsVersionAsync()
        {
            var hal = await GetHalDevice(DeviceDriverVersionKey);
            if (hal == null || !hal.Properties.ContainsKey(DeviceDriverVersionKey))
                return null;

            var versionParts = hal.Properties[DeviceDriverVersionKey].ToString().Split('.');
            return string.Join(".", versionParts.Take(2).ToArray());
        }
        private static async Task<PnpObject> GetHalDevice(params string[] properties)
        {
            var actualProperties = properties.Concat(new[] { DeviceClassKey });
            var rootDevices = await PnpObject.FindAllAsync(PnpObjectType.Device,
                actualProperties, RootQuery);

            foreach (var rootDevice in rootDevices.Where(d => d.Properties != null && d.Properties.Any()))
            {
                var lastProperty = rootDevice.Properties.Last();
                if (lastProperty.Value != null)
                    if (lastProperty.Value.ToString().Equals(HalDeviceClass))
                        return rootDevice;
            }
            return null;
        }
        const string DeviceClassKey = "{A45C254E-DF1C-4EFD-8020-67D146A850E0},10";
        const string DeviceDriverVersionKey = "{A8B865DD-2E3D-4094-AD97-E593A70C75D6},3";
        const string RootContainer = "{00000000-0000-0000-FFFF-FFFFFFFFFFFF}";
        const string RootQuery = "System.Devices.ContainerId:=\"" + RootContainer + "\"";
        const string HalDeviceClass = "4d36e966-e325-11ce-bfc1-08002be10318";
        
#endif
    }
}