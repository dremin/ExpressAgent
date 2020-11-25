using PureCloudPlatform.Client.V2.Api;

namespace ExpressAgent.Platform
{
    public class RoutingHelper
    {
        private RoutingApi RoutingApi = new RoutingApi();
        private Session Session;

        public RoutingHelper(Session session)
        {
            Session = session;
        }
    }
}
