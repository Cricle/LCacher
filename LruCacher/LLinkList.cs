using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LruCacher
{
    public struct LLinkList<T>: IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
    {
        public int Count { get; private set; }

        public bool IsSynchronized { get; }

        public object SyncRoot { get; }

        public bool IsReadOnly { get; }

        public LinkNode<T> First { get; private set; }

        public LinkNode<T> Larst { get; private set; }

        public T this[int index]
        {
            get
            {
                var f = First;
                var sp = 0;
                while (f != null && sp++ < index)
                {
                    f = f.Next;
                }
                return f.Value;
            }
        }
        public LLinkList(bool isSynchronized)
        {
            IsSynchronized = isSynchronized;
            Count = 0;
            First = Larst = null;
            SyncRoot = new object();
            IsReadOnly = false;

        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                First = Larst = null;
                Count=0;
            }
        }
        public void AddFirst(T item)
        {
            lock (SyncRoot)
            {
                var old = First;
                var node = new LinkNode<T>(null, First, item);
                First = node;
                if (old!=null)
                {
                    old.Previous = node;
                }
                if (Larst==null)
                {
                    Larst = First;
                }
                Count++;
            }
        }
        public void AddLarst(T item)
        {
            lock (SyncRoot)
            {
                var old = Larst;
                var node = new LinkNode<T>(Larst, null, item);
                Larst = node;
                if (old!=null)
                {
                    old.Next = node;
                }
                if (First==null)
                {
                    First = Larst;
                }
                Count++;
            }
        }
        public void AddAfter(LinkNode<T> target,LinkNode<T> item)
        {
            lock (SyncRoot)
            {
                var old = target.Next;
                target.Next = item;
                item.Previous = target;
                item.Next = old;
                if (old != null)
                {
                    old.Previous = item;
                }
                Count++;
            }
        }
        public void Remove(LinkNode<T> item)
        {
            lock (SyncRoot)
            {
                var prev = item.Previous;
                var next = item.Next;
                item.Previous = item.Next = null;
                if (prev!=null)
                {
                    prev.Next = next;
                }
                if (next!=null)
                {
                    next.Previous = prev;
                }
                Count--;
            }
        }
        public void RemoveFirst()
        {
            lock (SyncRoot)
            {
                if (First != null)
                {
                    var old = First.Next;
                    First.Next = null;
                    if (old!=null)
                    {
                        old.Previous = null;
                        First = old;
                    }
                    Count--;
                }
            }
        }
        public void RemoveLarst()
        {
            lock (SyncRoot)
            {
                if (Larst!=null)
                {
                    var old = Larst.Previous;
                    Larst.Previous = null;
                    if (old != null)
                    {
                        old.Next = null;
                        Larst = old;
                    }
                    Count--;
                }
            }
        }
        public void CopyTo(Array array, int index)
        {
            lock (SyncRoot)
            {
                var datas = new T[Count];
                var i = 0;
                var f = First;
                while (f != null)
                {
                    datas[i++] = f.Value;
                    f = f.Next;
                }
                Array.Copy(datas, 0, array, index, Count);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator();
        }
        struct Enumerator : IEnumerator,IEnumerator<T>
        {
            private static readonly Func<LinkNode<T>, LinkNode<T>> FirstMaker =
                ll => new LinkNode<T>(null, ll, default(T));
            private readonly LinkNode<T> oldFirst;
            private LinkNode<T> first;
            private LinkNode<T> enuumerating;
            private bool complated;
            object IEnumerator.Current => Current;

            public T Current
            {
                get
                {
                    if (enuumerating!=null)
                    {
                        return enuumerating.Value;
                    }
                    return default(T);
                }
            }

            public Enumerator(LinkNode<T> f)
            {
                oldFirst = first = f;
                enuumerating = FirstMaker(f);
                complated = false;
            }

            public bool MoveNext()
            {
                if (complated)
                {
                    return false;
                }
                var hasNext = enuumerating.Next != null;
                enuumerating = enuumerating.Next;
                return (complated = hasNext);
            }

            public void Reset()
            {
                enuumerating = FirstMaker(oldFirst);
                complated = false;
            }

            public void Dispose()
            {
            }
        }
    }
}
