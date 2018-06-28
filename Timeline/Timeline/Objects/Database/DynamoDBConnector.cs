using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

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
    }
}
