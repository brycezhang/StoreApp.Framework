using System.Collections.Generic;
using System.Linq;

namespace StoreApp.Framework.Utils
{
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {        
        private readonly IGrouping<TKey, TElement> _grouping;// 分组数据

        public Grouping(IGrouping<TKey, TElement> unit)
        {
            _grouping = unit;
        }

        /// <summary>
        /// 唯一的键值
        /// </summary>
        public TKey Key
        {
            get { return _grouping.Key; }
        }

        /// <summary>
        /// 重载判断相等方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Grouping<TKey, TElement> that = obj as Grouping<TKey, TElement>;
            return (that != null) && (this.Key.Equals(that.Key));
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _grouping.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _grouping.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
