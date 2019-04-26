using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LruCacher
{
    public class LinkNode<T>
    {
        public LinkNode<T> Previous { get; internal set; }
        public LinkNode<T> Next { get; internal set; }
        public T Value { get; set; }
        public LinkNode(LinkNode<T> previous, LinkNode<T> next, T value)
        {
            this.Previous = previous;
            this.Next = next;
            this.Value = value;
        }
    }
}
