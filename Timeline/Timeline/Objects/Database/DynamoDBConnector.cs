using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Timeline.Models.DynamoDBModels;

namespace Timeline.Objects.Database
{
    class DynamoDBConnector
    {
        private DynamoDBContext context;        

        public void Initialize(Amazon.Runtime.AWSCredentials credential)
        {
            var client = new AmazonDynamoDBClient(credential, Amazon.RegionEndpoint.EUCentral1);
            context = new DynamoDBContext(client);
        }

        public async Task<MDBUser> GetUserById(string id)
        {
            try
            {
                return await context.LoadAsync<MDBUser>(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetUserById ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task SaveUser(MDBUser user)
        {
            try
            {
                await context.SaveAsync<MDBUser>(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateUser ERROR: " + ex.Message);
                throw ex;
            }
        }
    }
}
