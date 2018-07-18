using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Timeline.Models;

namespace Timeline.Services
{
    public interface IDBService
    {
        void Connect(AWSCredentials credential);

        Task CreateUser(MUser user);
        Task UpdateUser(MUser user);
        Task<MUser> GetUser(string userId);

        Task StoreEvents(List<MTimelineEvent> timelineEvents);
        Task<List<MTimelineEvent>> GetEvents(string timelineId);
    }
}
