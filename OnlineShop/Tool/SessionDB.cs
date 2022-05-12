using System;
using System.Collections.Concurrent;

namespace OnlineShop.Tool
{
    public class SessionDB
    {
        public static ConcurrentDictionary<string, SessionInfo> sessionDB = new ConcurrentDictionary<string, SessionInfo>();//Modi@ Dictionary -> ConcurrentDictionary

        public class SessionInfo
        {
            public string SId { get; set; } = string.Empty;

            public DateTime ValidTime { get; set; } = DateTime.Now.AddMinutes(30);
        }
    }
}
