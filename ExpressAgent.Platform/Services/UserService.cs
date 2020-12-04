using ExpressAgent.Platform.Abstracts;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Model;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExpressAgent.Platform.Services
{
    public class UserService : PlatformService<UsersApi>
    {
        public UserService(UsersApi apiInstance, Session session) : base(apiInstance, session)
        {
        }

        public UserMe GetCurrentUser()
        {
            try
            {
                Debug.WriteLine($"UserService: Calling GetUsersMe");

                return ApiInstance.GetUsersMe(new List<string> { "presence", "routingStatus", "station" });
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new UserMe();
        }
    }
}
