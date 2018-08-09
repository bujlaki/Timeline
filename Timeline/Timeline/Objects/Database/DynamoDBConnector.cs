﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

using Timeline.Models;
using Timeline.Objects.Auth;

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
        public async Task<MUser> CreateUser(LoginData login)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineUsers");
                MUser user = new MUser();
                user.UserId = login.UserId;
                user.UserName = login.UserName;
                user.Email = login.Email;
                user.PhotoUrl = login.Picture;
                await table.PutItemAsync(DynamoDBAdapter.User2DynamoDoc(user));
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateUser ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task<MUser> GetUserById(string id)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineUsers");
                Document doc = await table.GetItemAsync(id);
                return DynamoDBAdapter.DynamoDoc2User(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetUserById ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task UpdateUser(MUser user)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineUsers");
                await table.UpdateItemAsync(DynamoDBAdapter.User2DynamoDoc(user));
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateUser ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task DeleteUser(MUser user)
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
        public async Task StoreEvent(MTimelineEvent tlevent)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineEvents");
                await table.PutItemAsync(DynamoDBAdapter.TimelineEvent2DynamoDoc(tlevent));
            }
            catch (Exception ex)
            {
                Console.WriteLine("StoreEvent ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task UpdateEvent(MTimelineEvent tlevent)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineEvents");
                await table.UpdateItemAsync(DynamoDBAdapter.TimelineEvent2DynamoDoc(tlevent));
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateEvent ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task StoreEvents(List<MTimelineEvent> timelineEvents)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineEvents");
                DocumentBatchWrite batchWrite = table.CreateBatchWrite();
                foreach (MTimelineEvent tlevent in timelineEvents) batchWrite.AddDocumentToPut( DynamoDBAdapter.TimelineEvent2DynamoDoc(tlevent) );

                await batchWrite.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("StoreEvents ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task<List<MTimelineEvent>> GetEvents(string timelineId)
        {
            try
            {
                List<MTimelineEvent> results = new List<MTimelineEvent>();

                Table table = Table.LoadTable(client, "TimelineEvents");
                QueryFilter filter = new QueryFilter("timelineid", QueryOperator.Equal, timelineId);
                Search search = table.Query(filter);

                do
                {
                    var docSet = await search.GetNextSetAsync();
                    foreach (Document doc in docSet) results.Add( DynamoDBAdapter.DynamoDoc2TimelineEvent(doc) );

                } while (!search.IsDone);

                return results;
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
