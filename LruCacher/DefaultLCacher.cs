using LruCacher.Model;
using LruCacher.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LruCacher
{
    public class DefaultLCacher<T> : LCacher<DefaultLCacheModel<T>, T>
    {
        public DefaultLCacher(LCacherOptions cacherOptions) 
            : base(cacherOptions)
        {
        }
    }
}
