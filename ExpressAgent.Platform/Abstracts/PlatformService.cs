namespace ExpressAgent.Platform.Abstracts
{
    public abstract class PlatformService<T>
    {
        internal T ApiInstance;
        internal Session Session;

        public PlatformService(T apiInstance, Session session)
        {
            ApiInstance = apiInstance;
            Session = session;
        }
    }
}
