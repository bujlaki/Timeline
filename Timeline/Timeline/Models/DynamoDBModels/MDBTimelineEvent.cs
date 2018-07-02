using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Models.DynamoDBModels
{
    public class MDBTimelineEvent
    {
        public string TimelineId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string URL { get; set; }
        public string Data { get; set; }
        public Int64 StartDate { get; set; }
        public Int64 EndDate { get; set; }
        public byte Precision { get; set; }
    }
}
