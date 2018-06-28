using System;
namespace Timeline.Services
{
    public interface IDBService
    {
        bool Connect();
        void CreateTimeline(string userId);
    }
}
