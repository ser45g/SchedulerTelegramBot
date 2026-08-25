using Quartz.Util;
using System.Collections.Concurrent;

namespace SchedulerTelegramBot.Bot.Stores
{
    public class AddNotificationCommandInfoStore
    {
        public class StoreData
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime? NotifyDate { get; set; }
        }

        private ConcurrentDictionary<long, StoreData> _data;

        public StoreData? Get(long id)
        {
            return _data.TryGetAndReturn(id);
        }
        public void Set(long id, StoreData data)
        {
            _data[id]= data;
        }

        public AddNotificationCommandInfoStore() 
        {
            _data = new ConcurrentDictionary<long, StoreData>();
        }
    }
}
