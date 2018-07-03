using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timeline.Models.DynamoDBModels;

namespace Timeline.Services
{
    public interface IDBService
    {
        void Connect(AWSCredentials credential);

        Task CreateUser(MDBUser user);
        Task UpdateUser(MDBUser user);
        Task<MDBUser> GetUser(string userId);

        Task StoreEvents(List<MDBTimelineEvent> timelineEvents);
        Task<List<MDBTimelineEvent>> GetEvents(string timelineId);
    }
}
