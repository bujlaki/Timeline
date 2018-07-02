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

        public void Initialize(Amazon.Runtime.AWSCredentials credential)
        {
            client = new AmazonDynamoDBClient(credential, Amazon.RegionEndpoint.EUCentral1);
        }

        #region "MDBUser"
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

        public async Task UpdateUser(MDBUser user)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineUsers");
                await table.UpdateItemAsync(user.AsDynamoDocument());
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateUser ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task DeleteUser(MDBUser user)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineUsers");
                await table.DeleteItemAsync(user.UserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteUser ERROR: " + ex.Message);
                throw ex;
            }
        }
        #endregion

        #region "MDBTimelineEvent"
        public async Task CreateEvent(MDBTimelineEvent timelineEvent)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineEvents");
                //await table.PutItemAsync(timelineEvent.AsDynamoDocument());
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateEvent ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task<List<MDBTimelineEvent>> GetEvents(string id)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineEvents");
                Document doc = await table.GetItemAsync(id);
                //return new MDBUser(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetEvents ERROR: " + ex.Message);
                throw ex;
            }
        }
        #endregion
    }
}
