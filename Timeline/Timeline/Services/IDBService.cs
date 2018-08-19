using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Timeline.Models;
using Timeline.Objects.Auth;

namespace Timeline.Services
{
    public interface IDBService
    {
        void Connect(AWSCredentials credential);

        Task CreateUser(LoginData login);
        Task UpdateUser(MUser user);
        Task<MUser> GetUser(string userId);
        Task<MUser> GetUserOrCreate(LoginData login);
        Task StoreEvent(MTimelineEvent tlevent);
        Task DeleteEvent(MTimelineEvent tlevent);
        Task UpdateEvent(MTimelineEvent tlevent);
        Task StoreEvents(List<MTimelineEvent> timelineEvents);
        Task<List<MTimelineEvent>> GetEvents(string timelineId);
    }
}
