using System;
using System.Collections.Generic;
using System.Text;

namespace LruCacher.Options
{
    public class LCacherOptions
    {
        public static readonly TimeSpan DefaultWaitOutTime = TimeSpan.FromMilliseconds(100);

        public static readonly long DefaultMaxSize = 100;
        public LCacherOptions()
        {
            MaxSize = DefaultMaxSize;
            WaitOutTime = DefaultWaitOutTime;
            if (MaxSize<=0)
            {
                throw new RankException("最大容量只能>0");
            }
        }

        /// <summary>
        /// 最大数量,默认<see cref="DefaultMaxSize"/>
        /// </summary>
        public long MaxSize { get; set; }
        /// <summary>
        /// 互斥锁超时时间,默认<see cref="DefaultWaitOutTime"/>
        /// </summary>
        public TimeSpan WaitOutTime { get; set; }
    }
}
