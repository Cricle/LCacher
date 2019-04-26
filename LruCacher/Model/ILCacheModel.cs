namespace LruCacher.Model
{
    public interface ILCacheModel<T>
    {
        long CreateTime { get; }
        T Entity { get; }
        bool IsVisited { get; }
        long VisitTime { get; }
    }
}