using System.ComponentModel;
using System.Windows;
using Microsoft.Phone.Controls;

namespace StoreApp.Framework
{
    /// <summary>
    /// 页面基类 
    /// </summary>
    /// <remarks>
    /// 实现View Model调用页面进入、离开相关事件
    /// </remarks>
    public class PhoneApplicationPageBase : PhoneApplicationPage
    {
        protected PhoneApplicationPageBase()
        {
            Loaded += PageBaseLoaded;
        }

        private void PageBaseLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as NavigationViewModelBase;
            if (viewModel != null)
            {
                viewModel.NavigationService = new NavigationHelper(NavigationService, NavigationParamHelper.Instance);
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            var viewModel = DataContext as NavigationViewModelBase;
            if (viewModel != null)
            {
                viewModel.OnBackKeyPress(e);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var viewModel = DataContext as NavigationViewModelBase;
            if (viewModel != null)
            {
                viewModel.NavigationContext = NavigationContext;
                viewModel.OnNavigatedTo(e);
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            var viewModel = DataContext as NavigationViewModelBase;
            if (viewModel != null)
            {
                viewModel.NavigationContext = NavigationContext;
                viewModel.OnNavigatingFrom(e);
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            var viewModel = DataContext as NavigationViewModelBase;
            if (viewModel != null)
            {
                viewModel.NavigationContext = NavigationContext;
                viewModel.OnNavigatedFrom(e);
            }
        }
    }
}
