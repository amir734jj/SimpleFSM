namespace SimpleFSM.Utilities
{
    /// <summary>
    /// Base builder, needed so that fields of mappers are re-created during each mapping process
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseBuilder<T> where T : new()
    {
        public static T New()
        {
            return new T();
        }
    }
}