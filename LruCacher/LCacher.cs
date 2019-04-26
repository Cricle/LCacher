using LruCacher.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using LruCacher.Options;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LruCacher
{
    public abstract class LCacher<TModel, TEntity> : IEnumerable<TModel>, IEnumerable, IDisposable, ILCacher<TModel, TEntity> 
        where TModel : LCacheModel<TEntity>,new()
    {
        private readonly object Locker = new object();
        protected readonly LinkedList<TModel> models;
        protected readonly LCacherOptions cacherOptions;

        public IReadOnlyCollection<TModel> Models => models;
        public virtual int Count => models.Count;
        public virtual TModel First => models.First?.Value;
        public virtual TModel Larst => models.Last?.Value;
        public TEntity this[int index] => Get(e => true).Skip(index).FirstOrDefault();
        public LCacher(LCacherOptions cacherOptions)
        {
            this.cacherOptions = cacherOptions;
            models = new LinkedList<TModel>();
        }
        private T[] Get<T>(Func<TEntity, bool> condition, Func<TModel, T> selector, int max = 0)
        {
            var rl = new List<T>(max > 0 ? max : 5);
            var e1 = models.First;
            while (e1 != null && (max <= 0 || rl.Count < max))
            {
                if (condition(e1.Value.Entity))
                {
                    rl.Add(selector(e1.Value));
                }
                e1 = e1.Next;
            }
            return rl.ToArray();
        }
        public virtual TEntity[] Get(Func<TEntity, bool> condition, int max = 0)
        {
            return Get(condition, m => m.Entity, max);
        }
        public virtual TEntity GetOne(Func<TEntity, bool> condition)
        {
            return Get(condition, e => e.Entity).FirstOrDefault();
        }
        protected virtual TModel CreateModel(TEntity entity)
        {
            return new TModel
            {
                Entity=entity
            };
        }
        public virtual bool Remove(Func<TEntity, bool> condition)
        {
            try
            {
                var s = models.First;
                if (s != null)
                {
                    do
                    {
                        if (condition(s.Value.Entity))
                        {
                            break;
                        }
                        s = s.Next;
                    } while (s != null);
                    if (s != null)
                    {
                        models.Remove(s);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public virtual bool Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var m = CreateModel(entity);
            lock (Locker)
            {
                if (models.Count < cacherOptions.MaxSize)
                {
                    models.AddLast(m);
                }
                else
                {
                    models.RemoveFirst();
                    models.AddLast(m);
                }
            }
            return true;
        }
        public virtual bool Clear()
        {
            try
            {
                lock (Locker)
                {
                    models.Clear();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public virtual TModel[] ToArray()
        {
            try
            {
                TModel[] res = null;
                lock (Locker)
                {
                    res = models.ToArray();
                }
                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public IEnumerator<TModel> GetEnumerator()
        {
            return models.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {

        }
    }
}
