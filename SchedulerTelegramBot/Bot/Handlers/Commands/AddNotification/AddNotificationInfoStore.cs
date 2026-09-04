using Quartz.Util;
using System.Collections.Concurrent;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.AddNotification
{
    public class AddNotificationInfoStore
    {
        public class StoreData
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime? NotifyDate { get; set; }
        }

        private ConcurrentDictionary<long, StoreData> _data = new();

        public StoreData? Get(long id)
        {
            return _data.TryGetAndReturn(id);
        }
        public void Set(long id, StoreData data)
        {
            _data[id]= data;
        }

        public void Remove(long id)
        {
            _data.TryRemove(id, out var _);
        }
    }
}
