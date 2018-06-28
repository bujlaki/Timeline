using System;
using Timeline.Objects.Database;

namespace Timeline.Services
{
	public class DBService : IDBService
    {
        private DynamoDBConnector dynamo;

        public DBService()
        {
            dynamo = new DynamoDBConnector();
        }

        public bool Connect()
        {
            throw new NotImplementedException();
        }

        public void CreateTimeline(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
