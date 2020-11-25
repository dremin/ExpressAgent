using PureCloudPlatform.Client.V2.Api;

namespace ExpressAgent.Platform.Services
{
    public class RoutingService
    {
        private RoutingApi RoutingApi = new RoutingApi();
        private Session Session;

        public RoutingService(Session session)
        {
            Session = session;
        }
    }
}
