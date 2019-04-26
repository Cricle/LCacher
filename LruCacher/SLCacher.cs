using LruCacher.Model;
using LruCacher.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LruCacher
{
    public struct SLCacher<TModel, TEntity> : ILCacher<TModel, TEntity>
        where TModel : LCacheModel<TEntity>,new()
    {
        private readonly object AddLocker;
        private readonly LCacherOptions cacherOptions;
        private LLinkList<TModel> models;

        public TEntity this[int index]
        {
            get
            {
                var res = default(TEntity);
                lock (models.SyncRoot)
                {
                    var f = models.First;
                    var i = 0;
                    while (f != null && i++ < index)
                    {
                        f = f.Next;
                    }
                    if (f!=null)
                    {
                        res = f.Value.Entity;
                    }
                }
                return res;
            }
        }

        public int Count => models.Count;

        public TModel First => models.First?.Value;

        public TModel Larst => models.Larst?.Value;

        public IReadOnlyCollection<TModel> Models => models;
        public SLCacher(LCacherOptions co)
        {
            cacherOptions = co;
            models = new LLinkList<TModel>(true);
            AddLocker = new object();
        }
        public bool Add(TEntity entity)
        {
            lock (AddLocker)
            {
                var model = new TModel
                {
                    Entity = entity
                };
                if (models.Count < cacherOptions.MaxSize)
                {
                    models.AddLarst(model);
                }
                else
                {
                    models.RemoveFirst();
                    models.AddLarst(model);
                }
            }
            
            return true;
        }

        public bool Clear()
        {
            models.Clear();
            return true;
        }

        public void Dispose()
        {
            
        }

        public IEnumerator<TModel> GetEnumerator()
        {
            return models.GetEnumerator();
        }

        private LinkNode<TModel> GetOneNode(Func<TEntity, bool> condition)
        {
            LinkNode<TModel> model =null;
            lock (models.SyncRoot)
            {
                var f = models.First;
                while (f != null)
                {
                    if (condition(f.Value.Entity))
                    {
                        model = f;
                        break;
                    }
                    f = f.Next;
                }
                if (model != null)
                {
                    models.AddLarst(model.Value);
                    models.Remove(model);
                }
            }            
            return model;
        }
        public TEntity GetOne(Func<TEntity, bool> condition)
        {
            var node = GetOneNode(condition);
            if (node!=null)
            {
                return node.Value.Entity;
            }
            return default(TEntity);
        }
        public bool Remove(Func<TEntity, bool> condition)
        {
            var r = GetOneNode(condition);
            if (r != null)
            {
                models.Remove(r);
                return true;
            }
            return false;
        }

        public TModel[] ToArray()
        {
            var ms = new TModel[models.Count];
            models.CopyTo(ms, 0);
            return ms;
        }
    }
}
