using Windows.Storage;

namespace StoreApp.Framework.Storage
{
    public class AppDataService
    {
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void Save<T>(string key, T value)
        {
#if WINDOWS_PHONE
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings[key] = value;
#endif
#if WINDOWS_8
            ApplicationData.Current.LocalSettings.Values[key] = value;
#endif
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static T Read<T>(string key)
        {
#if WINDOWS_PHONE
            var settings = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(key))
            {
                return (T)settings[key];
            }
#endif
#if WINDOWS_8
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                return (T)ApplicationData.Current.LocalSettings.Values[key];
            }
#endif
            return default(T);
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>成功true/失败false</returns>
        public static bool Remove(string key)
        {
#if WINDOWS_PHONE
            return System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Remove(key);
#endif
#if WINDOWS_8
            return ApplicationData.Current.LocalSettings.Values.Remove(key);
#endif
        }
    }
}
