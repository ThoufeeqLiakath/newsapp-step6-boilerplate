using System;

namespace NewsService.Exceptions
{
    public class ReminderAlreadyExistsException:ApplicationException
    {
        public ReminderAlreadyExistsException() { }
        public ReminderAlreadyExistsException(string message) : base(message) { }
    }
}
