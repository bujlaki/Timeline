﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
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

        #region "MUser"
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

        #region "MTimelineInfo"
        public async Task StoreSharedTimeline(MTimelineInfo tlinfo, MUser user)
        {
            try
            {
                Table table = Table.LoadTable(client, "SharedTimelines");
                tlinfo.OwnerID = user.UserId;
                tlinfo.OwnerName = user.UserName;
                await table.PutItemAsync(DynamoDBAdapter.TimelineInfo2DynamoDoc(tlinfo));

                table = Table.LoadTable(client, "SharedTimelineTags");
                DocumentBatchWrite batchWrite = table.CreateBatchWrite();
                foreach (string tag in tlinfo.Tags)
                {
                    Document tagDoc = new Document();
                    tagDoc.Add("tag", tag);
                    tagDoc.Add("timelineid", tlinfo.TimelineId);
                    batchWrite.AddDocumentToPut(tagDoc);
                }
                await batchWrite.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("StoreSharedTimeline ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task UpdateSharedTimeline(MTimelineInfo tlinfo, MUser user)
        {
            try
            {
                Table table = Table.LoadTable(client, "SharedTimelines");
                tlinfo.OwnerID = user.UserId;
                tlinfo.OwnerName = user.UserName;
                await table.UpdateItemAsync(DynamoDBAdapter.TimelineInfo2DynamoDoc(tlinfo));
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateSharedTimeline ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task UpdateSharedTimelineTags(MTimelineInfo tlinfo)
        {
            try
            {
                //first let's get the TAGs already in the table for this timeline
                List<string> existingTags = new List<string>();
                Dictionary<string, AttributeValue> lastKeyEvaluated = null;
                QueryRequest request = new QueryRequest("SharedTimelineTags");
                request.IndexName = "timelineid-tag-index";
                request.ExpressionAttributeValues.Add(":timelineIdValue", new AttributeValue(tlinfo.TimelineId));
                request.KeyConditionExpression = "timelineid=:timelineIdValue";

                do
                {
                    request.ExclusiveStartKey = lastKeyEvaluated;
                    var response = await client.QueryAsync(request);

                    foreach (Dictionary<string, AttributeValue> data in response.Items)
                    {
                        Document doc = Document.FromAttributeMap(data);
                        if(doc.ContainsKey("tag")) existingTags.Add(doc["tag"]);
                    }

                    lastKeyEvaluated = response.LastEvaluatedKey;
                } while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);

                //then build a list of the new TAGs
                List<string> newTags = new List<string>();
                foreach (string tag in tlinfo.Tags)
                    if (!existingTags.Contains(tag)) newTags.Add(tag);

                //then build a list of deleted TAGs
                List<string> deletedTags = new List<string>();
                foreach (string tag in existingTags)
                    if (!tlinfo.Tags.Contains(tag)) deletedTags.Add(tag);

                //then delete deleted TAGs
                Table table;
                table = Table.LoadTable(client, "SharedTimelineTags");
                DocumentBatchWrite batchDelete = table.CreateBatchWrite();
                foreach (string tag in deletedTags) batchDelete.AddKeyToDelete(tag, tlinfo.TimelineId);
                await batchDelete.ExecuteAsync();

                //then store new TAGs
                DocumentBatchWrite batchWrite = table.CreateBatchWrite();
                foreach (string tag in newTags)
                {
                    Document tagDoc = new Document();
                    tagDoc.Add("tag", tag);
                    tagDoc.Add("timelineid", tlinfo.TimelineId);
                    batchWrite.AddDocumentToPut(tagDoc);
                }
                await batchWrite.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateSharedTimeline ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task DeleteSharedTimelineTags(MTimelineInfo tlinfo)
        {
            try
            {
                if (tlinfo.Tags.Length == 0) return;
                Table table = Table.LoadTable(client, "SharedTimelineTags");
                DocumentBatchWrite batchDelete = table.CreateBatchWrite();
                foreach (string tag in tlinfo.Tags) batchDelete.AddKeyToDelete(tag, tlinfo.TimelineId);
                await batchDelete.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteSharedTimelineTags ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task<List<string>> SearchSharedTimelinesForTag(string tag)
        {
            try
            {
                List<string> resultIds = new List<string>();

                Dictionary<string, AttributeValue> lastKeyEvaluated = null;

                QueryRequest request = new QueryRequest("SharedTimelineTags");
                request.ExpressionAttributeValues.Add(":tagValue", new AttributeValue(tag));
                request.KeyConditionExpression = "tag=:tagValue";

                do {
                    request.ExclusiveStartKey = lastKeyEvaluated;
                    var response = await client.QueryAsync(request);
                    foreach (Dictionary<string, AttributeValue> data in response.Items) resultIds.Add(data["timelineid"].S);
                    lastKeyEvaluated = response.LastEvaluatedKey;

                } while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);

                return resultIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SearchTimelinesForTag ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task<MTimelineInfo> GetSharedTimelineForID(string id)
        {
            try
            {
                Table table = Table.LoadTable(client, "SharedTimelines");
                Document doc = await table.GetItemAsync(id);
                return DynamoDBAdapter.DynamoDoc2TimelineInfo(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetSharedTimelineForID ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task<List<MTimelineInfo>> GetSharedTimelinesForIDs(List<string> idList)
        {
            try
            {
                List<MTimelineInfo> results = new List<MTimelineInfo>();

                Table table = Table.LoadTable(client, "SharedTimelines");
                DocumentBatchGet batchGet = table.CreateBatchGet();
                foreach (string id in idList) batchGet.AddKey(id);

                await batchGet.ExecuteAsync();

                foreach (Document doc in batchGet.Results) results.Add(DynamoDBAdapter.DynamoDoc2TimelineInfo(doc));
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetSharedTimelinesForIDs ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task DeleteSharedTimelinesByIDs(List<string> idList)
        {
            try
            {
                Table table = Table.LoadTable(client, "SharedTimelines");
                DocumentBatchWrite batchWrite = table.CreateBatchWrite();
                
                foreach (string id in idList) batchWrite.AddKeyToDelete(id);
                await batchWrite.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteSharedTimelinesByIDs ERROR: " + ex.Message);
                throw ex;
            }
        }
        #endregion

        #region "MTimelineEvent"
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

        public async Task DeleteEvent(MTimelineEvent tlevent)
        {
            try
            {
                Table table = Table.LoadTable(client, "TimelineEvents");
                await table.DeleteItemAsync(tlevent.TimelineId, tlevent.EventId);
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

                Dictionary<string, AttributeValue> lastKeyEvaluated = null;

                QueryRequest request = new QueryRequest("TimelineEvents");
                request.IndexName = "timelineid-startdate-index";
                request.ExpressionAttributeValues.Add(":timelineIdValue", new AttributeValue(timelineId));
                request.KeyConditionExpression = "timelineid=:timelineIdValue";

                do
                {
                    request.ExclusiveStartKey = lastKeyEvaluated;

                    var response = await client.QueryAsync(request);
                    
                    foreach(Dictionary<string, AttributeValue> data in response.Items)
                    {
                        Document doc = Document.FromAttributeMap(data);
                        results.Add(DynamoDBAdapter.DynamoDoc2TimelineEvent(doc));
                    }

                    lastKeyEvaluated = response.LastEvaluatedKey;

                } while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetEvents ERROR: " + ex.Message);
                throw ex;
            }
        }

        public async Task DeleteEventsByTimelineId(string timelineId)
        {
            try
            {
                List<MTimelineEvent> events = await GetEvents(timelineId);

                if (events.Count == 0) return;

                Table table = Table.LoadTable(client, "TimelineEvents");
                DocumentBatchWrite batchWrite = table.CreateBatchWrite();

                foreach (MTimelineEvent tlevent in events) batchWrite.AddKeyToDelete(tlevent.TimelineId, tlevent.EventId);

                await batchWrite.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteEventsByTimelineId ERROR: " + ex.Message);
                throw ex;
            }
        }
        #endregion
    }
}
