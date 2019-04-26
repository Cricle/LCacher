using System;
using System.Collections.Generic;
using System.Text;

namespace LruCacher.Model
{
    public abstract class LCacheModel<T> : ILCacheModel<T>
    {
        private T entity;

        public LCacheModel()
        {
            CreateTime = DateTime.Now.Ticks;
            VisitTime = CreateTime;
        }
        /// <summary>
        /// 缓存实体
        /// </summary>
        public virtual T Entity
        {
            get
            {
                VisitTime = DateTime.Now.Ticks;
                return entity;
            }
            internal set
            {
                entity = value;
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual long CreateTime { get; private set; }
        /// <summary>
        /// 访问时间
        /// </summary>
        public virtual long VisitTime { get; private set; }
        /// <summary>
        /// 是否被访问过了
        /// </summary>
        public virtual bool IsVisited => VisitTime != CreateTime;
    }
}
