

namespace CYBERSECURITY_AWARENESS.Models
{
    public class Task
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ReminderDate { get; set; }
        public bool IsCompleted { get; set; }

        public string CreatedAt { get; set; }
	}
}
