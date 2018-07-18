using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Timeline.Models;
using Timeline.Objects.Database;

namespace Timeline.Services
{
	public class DBService : IDBService
    {
        private DynamoDBConnector ddb;

        public DBService()
        {
            ddb = new DynamoDBConnector();
        }

        public void Connect(AWSCredentials credential)
        {
            try
            {
                ddb.Initialize(credential);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Connect ERROR: " + ex.Message);
                throw ex;
            }   
        }

        public async Task CreateUser(MUser user)
        {
            await ddb.CreateUser(user);
        }

        public async Task UpdateUser(MUser user)
        {
            await ddb.UpdateUser(user);
        }

        public async Task<MUser> GetUser(string userId)
        {
            return await ddb.GetUserById(userId);
        }

        public async Task StoreEvents(List<MTimelineEvent> timelineEvents)
        {
            await ddb.StoreEvents(timelineEvents);
        }

        public async Task<List<MTimelineEvent>> GetEvents(string timelineId)
        {
            return await ddb.GetEvents(timelineId);
        }
    }
}
