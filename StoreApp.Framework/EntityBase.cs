using GalaSoft.MvvmLight;

namespace StoreApp.Framework
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class EntityBase : ObservableObject
    {
        public virtual string Id { get; set; }
    }
}
