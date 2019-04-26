using System;
using System.Collections.Generic;
using LruCacher.Model;

namespace LruCacher
{
    public interface ILCacher<TModel, TEntity> 
        where TModel : LCacheModel<TEntity>
    {
        TEntity this[int index] { get; }

        int Count { get; }
        TModel First { get; }
        TModel Larst { get; }
        IReadOnlyCollection<TModel> Models { get; }

        bool Add(TEntity entity);
        bool Clear();
        void Dispose();
        IEnumerator<TModel> GetEnumerator();
        TEntity GetOne(Func<TEntity, bool> condition);
        bool Remove(Func<TEntity, bool> condition);
        TModel[] ToArray();
    }
}