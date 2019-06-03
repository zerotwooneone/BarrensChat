using System;
using Newtonsoft.Json;

namespace ChatUw.NotificationHub
{
    public class RegistrationModel
    {
        public RegistrationModel(string id, DateTime expiration)
        {
            Id = id;
            Expiration = expiration;
        }

        public string Id { get; }
        public DateTime Expiration { get; }
    }
}