using Amazon.Runtime;
using System;
using System.Threading.Tasks;
using Timeline.Models.DynamoDBModels;
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

        public async Task SaveUser(MDBUser user)
        {
            await ddb.SaveUser(user);
        }

        public async Task<MDBUser> GetUser(string userId)
        {
            return await ddb.GetUserById(userId);
        }

        public void CreateTimeline(string userId)
        {
            throw new NotImplementedException();
        }


    }
}
