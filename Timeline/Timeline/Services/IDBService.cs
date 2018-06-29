using Amazon.Runtime;
using System;
using System.Threading.Tasks;
using Timeline.Models.DynamoDBModels;

namespace Timeline.Services
{
    public interface IDBService
    {
        void Connect(AWSCredentials credential);
        Task SaveUser(MDBUser user);
        Task<MDBUser> GetUser(string userId);
        void CreateTimeline(string userId);
    }
}
