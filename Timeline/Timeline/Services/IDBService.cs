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
        Task DeleteUser(MUser user);
        Task<MUser> GetUser(string userId);
        Task<MUser> GetUserOrCreate(LoginData login);
        Task ShareTimeline(MTimelineInfo tlinfo, MUser user);
        Task UpdateSharedTimeline(MTimelineInfo tlinfo, MUser user);
        Task UpdateSharedTimelineTags(MTimelineInfo tlinfo);
        Task DeleteSharedTimelineTags(MTimelineInfo tlinfo);
        Task<List<MTimelineInfo>> SearchSharedTimeline(string tag);
        Task<MTimelineInfo> GetSharedTimelineByID(string id);
        Task<List<MTimelineInfo>> GetSharedTimelinesByIDs(List<string> idList);
        Task DeleteSharedTimelinesByIDs(List<string> idList);
        Task StoreEvent(MTimelineEvent tlevent);
        Task DeleteEvent(MTimelineEvent tlevent);
        Task UpdateEvent(MTimelineEvent tlevent);
        Task StoreEvents(List<MTimelineEvent> timelineEvents);
        Task<List<MTimelineEvent>> GetEvents(string timelineId);
        Task DeleteEventsByTimelineId(string timelineId);
    }
}
