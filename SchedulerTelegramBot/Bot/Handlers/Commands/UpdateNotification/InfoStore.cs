using Quartz.Util;
using System.Collections.Concurrent;

namespace SchedulerTelegramBot.Bot.Handlers.Commands.UpdateNotification
{
    public class InfoStore
    {
        public class StoreData
        {
            public Guid? Id { get; set; }
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
    }
}
