﻿using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Timeline.Models;
using Timeline.Objects.Auth;
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

        public async Task CreateUser(LoginData login)
        {
            await ddb.CreateUser(login);
        }

        public async Task UpdateUser(MUser user)
        {
            await ddb.UpdateUser(user);
        }

        public async Task DeleteUser(MUser user)
        {
            await ddb.DeleteUser(user);
        }

        public async Task<MUser> GetUser(string userId)
        {
            return await ddb.GetUserById(userId);
        }

        public async Task<MUser> GetUserOrCreate(LoginData login)
        {
            MUser u = await ddb.GetUserById(login.UserId);
            if (u == null) u = await ddb.CreateUser(login);
            return u;
        }

        public async Task ShareTimeline(MTimelineInfo tlinfo, MUser user)
        {
            await ddb.StoreSharedTimeline(tlinfo, user);
        }

        public async Task UpdateSharedTimeline(MTimelineInfo tlinfo, MUser user)
        {
            await ddb.UpdateSharedTimeline(tlinfo, user);
        }

        public async Task UpdateSharedTimelineTags(MTimelineInfo tlinfo)
        {
            await ddb.UpdateSharedTimelineTags(tlinfo);
        }

        public async Task DeleteSharedTimelineTags(MTimelineInfo tlinfo)
        {
            await ddb.DeleteSharedTimelineTags(tlinfo);
        }

        public async Task<List<MTimelineInfo>> SearchSharedTimeline(string tag)
        {
            List<string> idList = await ddb.SearchSharedTimelinesForTag(tag);

            return await ddb.GetSharedTimelinesForIDs(idList);
        }

        public async Task<MTimelineInfo> GetSharedTimelineByID(string id)
        {
            return await ddb.GetSharedTimelineForID(id);
        }

        public async Task<List<MTimelineInfo>> GetSharedTimelinesByIDs(List<string> idList)
        {
            return await ddb.GetSharedTimelinesForIDs(idList);
        }

        public async Task DeleteSharedTimelinesByIDs(List<string> idList)
        {
            await ddb.DeleteSharedTimelinesByIDs(idList);
        }

        public async Task StoreEvent(MTimelineEvent tlevent)
        {
            await ddb.StoreEvent(tlevent);
        }

        public async Task DeleteEvent(MTimelineEvent tlevent)
        {
            await ddb.DeleteEvent(tlevent);
        }

        public async Task UpdateEvent(MTimelineEvent tlevent)
        {
            await ddb.UpdateEvent(tlevent);
        }

        public async Task StoreEvents(List<MTimelineEvent> timelineEvents)
        {
            await ddb.StoreEvents(timelineEvents);
        }

        public async Task<List<MTimelineEvent>> GetEvents(string timelineId)
        {
            return await ddb.GetEvents(timelineId);
        }

        public async Task DeleteEventsByTimelineId(string timelineId)
        {
            await ddb.DeleteEventsByTimelineId(timelineId);
        }
    }
}
