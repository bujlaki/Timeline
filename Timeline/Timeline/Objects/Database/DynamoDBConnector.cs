using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Timeline.Models.DynamoDBModels;

namespace Timeline.Objects.Database
{
    class DynamoDBConnector
    {
        private AmazonDynamoDBClient client;
        //private DynamoDBContext context;        

        public void Initialize(Amazon.Runtime.AWSCredentials credential)
        {
            client = new AmazonDynamoDBClient(credential, Amazon.RegionEndpoint.EUCentral1);
            //context = new DynamoDBContext(client);
        }

        #region "MDBUser"
        public async Task<MDBUser> GetUserById(string id)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineUsers");
                Document doc = await table.GetItemAsync(id);
                return new MDBUser(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetUserById ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task CreateUser(MDBUser user)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineUsers");
                await table.PutItemAsync(user.AsDynamoDocument());
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateUser ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task UpdateUser(MDBUser user)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineUsers");
                await table.UpdateItemAsync(user.AsDynamoDocument());
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateUser ERROR: " + ex.Message);
                throw ex;
            }
        }
        #endregion
    }
}
