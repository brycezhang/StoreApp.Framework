namespace StoreApp.Framework
{
    /// <summary>
    /// 用于导航时传递复杂对象参数的辅助类
    /// </summary>
    public sealed class NavigationParamHelper
    {
        private static NavigationParamHelper _navigationParamHelper;

        private dynamic _item;

        private NavigationParamHelper() { }
        
        public static NavigationParamHelper Instance
        {
            get
            {
                return _navigationParamHelper ?? (_navigationParamHelper = new NavigationParamHelper());
            }
        }
        /// <summary>
        /// 设置导航参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="navItem"></param>
        public void SetNavigationParam(dynamic navItem)
        {
            _item = navItem;
        }
        /// <summary>
        /// 获取导航参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public dynamic GetNavigationParam()
        {
            return _item;
        }
    }
}
