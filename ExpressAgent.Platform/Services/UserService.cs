using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Model;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExpressAgent.Platform.Services
{
    public class UserService
    {
        private UsersApi UsersApi = new UsersApi();
        private Session Session;

        public UserService(Session session)
        {
            Session = session;
        }

        public UserMe GetCurrentUser()
        {
            try
            {
                Debug.WriteLine($"Users: Calling GetUsersMe");

                return UsersApi.GetUsersMe(new List<string> { "presence", "routingStatus", "station" });
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new UserMe();
        }
    }
}
