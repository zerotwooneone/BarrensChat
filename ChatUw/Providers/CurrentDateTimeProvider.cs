using System;

namespace ChatUw.Providers
{
    public class CurrentDateTimeProvider
    {
        public virtual DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}