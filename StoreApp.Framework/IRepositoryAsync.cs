using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StoreApp.Framework
{
    /// <summary>
    /// 仓储泛型接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepositoryAsync<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="item">Item to add to repository</param>
        Task Add(TEntity item);

        /// <summary>
        /// 添加实体列表
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task AddAll(IList<TEntity> items);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="item">Item to delete</param>
        Task Remove(TEntity item);

        /// <summary>
        /// 删除全部
        /// </summary>
        Task RemoveAll();

        /// <summary>
        /// 删除列表
        /// </summary>
        Task RemoveAll(IList<TEntity> items);

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="item">Item to modify</param>
        Task Modify(TEntity item);

        /// <summary>
        /// 通过Id获取实体
        /// </summary>
        /// <param name="id">Entity key value</param>
        /// <returns></returns>
        Task<TEntity> Get(string id);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>实体列表</returns>
        Task<IList<TEntity>> GetAll();

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <param name="pageIndex">页号</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>实体列表</returns>
        Task<IList<TEntity>> GetAll(int pageIndex, int pageSize);

        /// <summary>
        /// 获取匹配的实体列表
        /// </summary>
        /// <param name="filter">Filter that each element do match</param>
        /// <returns>List of selected elements</returns>
        Task<IList<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// 获取匹配的实体列表
        /// </summary>
        /// <param name="filter">Filter that each element do match</param>
        /// <param name="pageIndex">页号</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>List of selected elements</returns>
        Task<IList<TEntity>> GetFiltered(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageSize);
    }
}
