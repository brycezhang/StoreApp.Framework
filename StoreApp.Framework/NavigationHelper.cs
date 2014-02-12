using System;
using System.Linq;
using System.Windows.Navigation;

namespace StoreApp.Framework
{
    /// <summary>
    /// NavigationService的门面类，增加传参的方法
    /// </summary>
    public class NavigationHelper
    {
        public NavigationService NavigationService { get; private set; }
        public NavigationParamHelper NavigationParamHelper { get; private set; }

        public NavigationHelper(NavigationService navigationService, NavigationParamHelper navigationParamHelper)
        {
            NavigationService = navigationService;
            NavigationParamHelper = navigationParamHelper;
        }

        public bool CanGoForward { get { return NavigationService.CanGoForward; } }

        public bool CanGoBack { get { return NavigationService.CanGoBack; } }

        public bool Navigate(Uri source)
        {
            return NavigationService.Navigate(source);
        }

        public bool Navigate(string source)
        {
            return NavigationService.Navigate(new Uri(source, UriKind.RelativeOrAbsolute));
        }

        public bool Navigate<T>(Uri source, T param)
        {
            NavigationParamHelper.SetNavigationParam(param);
            return NavigationService.Navigate(source);
        }

        public bool Navigate<T>(string source, T param)
        {
            NavigationParamHelper.SetNavigationParam(param);
            return NavigationService.Navigate(new Uri(source, UriKind.RelativeOrAbsolute));
        }

        public void GoForward()
        {
            NavigationService.GoForward();
        }

        public void GoBack()
        {
            NavigationService.GoBack();
        }

        public JournalEntry RemoveBackEntry()
        {
            return NavigationService.RemoveBackEntry();
        }

        public void ClearBackStack()
        {
            var count = NavigationService.BackStack.Count();
            for (int i = 0; i < count; i++)
            {
                RemoveBackEntry();
            }
        }
    }
}
