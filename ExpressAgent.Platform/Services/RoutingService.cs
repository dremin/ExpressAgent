using ExpressAgent.Platform.Abstracts;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ExpressAgent.Platform.Services
{
    public class RoutingService : PlatformService<RoutingApi>
    {
        public ObservableCollection<Queue> Queues { get; set; }

        public RoutingService(RoutingApi apiInstance, Session session) : base(apiInstance, session)
        {
            Queues = new ObservableCollection<Queue>();
        }

        private List<Queue> GetQueues(int pageNumber = 0)
        {
            try
            {
                Debug.WriteLine($"RoutingService: Calling GetRoutingQueues");

                QueueEntityListing result = ApiInstance.GetRoutingQueues(pageNumber);
                List<Queue> queues = new List<Queue>();

                foreach (var entity in result.Entities)
                {
                    queues.Add(entity);
                }

                if (result.PageNumber != null)
                {
                    if (result.PageCount > result.PageNumber)
                    {
                        queues.AddRange(GetQueues((int)result.PageNumber + 1));
                    }
                }

                return queues;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new List<Queue>();
        }

        public ObservableCollection<Queue> SetQueueCollection()
        {
            try
            {
                Queues = new ObservableCollection<Queue>(GetQueues());

                return Queues;
            }
            catch (ApiException e)
            {
                Session.HandleException(e);
            }

            return new ObservableCollection<Queue>();
        }
    }
}
