using System;
using System.Collections.Generic;
using System.Text;

namespace CYBERSECURITY_AWARENESS.Models
{
    public class LogEntry
    {
        public int Id { get; set; }
		public string Action { get; set; }
		public string Details { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
