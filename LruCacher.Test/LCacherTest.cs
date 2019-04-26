using LruCacher.Model;
using LruCacher.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LruCacher.Test
{
    public class Entity
    {
        static int id = 1;
        public Entity()
        {
            Id = id++;
        }

        public int Id { get; }
    }
    public class LCacherTest
    {
        private ILCacher<DefaultLCacheModel<Entity>,Entity> lcacher;

        void Reset(int count=2)
        {
            var opt = new LCacherOptions { MaxSize = count };
            lcacher = new DefaultLCacher<Entity>(opt);
            //lcacher = new SLCacher<DefaultLCacheModel<Entity>, Entity>(opt);
        }
        [Test]
        public void TestLCacher_NotFull()
        {
            Reset();
            lcacher.Add(new Entity());
            Assert.True(lcacher.Count == 1);
        }
        [Test]
        public void TestLCacher_Full()
        {
            Reset();
            lcacher.Add(new Entity());
            lcacher.Add(new Entity());
            Assert.True(lcacher.Count == 2);
        }
        [Test]
        public void TestLCacher_LRU_Replate()
        {
            Reset();
            lcacher.Add(new Entity());
            lcacher.Add(new Entity());
            lcacher.Add(new Entity());
            Assert.True(lcacher.Count == 2);
            Assert.True(lcacher[1].Id == 3);//LRU替换了1
            Assert.True(lcacher[0].Id == 2);
        }
        [Theory]
        [TestCase(10000, 10000)]
        [TestCase(10000, 10100)]
        [TestCase(10000, 10001)]
        [TestCase(10000, 20000)]
        public void TestLCacher_MuliteTask(int total,int testcount)
        {
            Reset(total);
            var tasks = new Task[4];
            for (int i = 0; i < 4; i++)
            {
                tasks[i] = new Task(() =>
                  {
                      for (int j = 0; j < testcount/4; j++)
                      {
                          lcacher.Add(new Entity());
                      }
                  });
            }
            foreach (var item in tasks)
            {
                item.Start();
            }
            Task.WaitAll(tasks);
            Assert.True(lcacher.Count == total);
        }
        [Test]
        public void TestLCacher_Clear()
        {
            Reset();
            lcacher.Add(new Entity());
            lcacher.Clear();
            Assert.True(lcacher.Count == 0);
        }
        [Test]
        public void TestLCacher_BigCollection()
        {

            TestLCacher_MuliteTask(10000, 1000000);//0.336
        }
        [Test]
        public void TestLCacher_VeryBigCollection()
        {
            TestLCacher_MuliteTask(10000, 10000000);//3.333
        }
        [Test]
        public void TestLCacher_Remove()
        {
            Reset();
            lcacher.Add(new Entity());
            lcacher.Add(new Entity());
            Assert.True(lcacher.Remove(e => e.Id == 1));
            Assert.True(lcacher.Count == 1);
            Assert.True(lcacher[0].Id == 2);
        }
        [Test]
        public void TestLCacher_RemoveNoEntity()
        {
            Reset();
            lcacher.Add(new Entity());
            lcacher.Add(new Entity());
            Assert.True(!lcacher.Remove(e => e.Id == 3));
            Assert.True(lcacher.Count == 2);
        }
    }
}
