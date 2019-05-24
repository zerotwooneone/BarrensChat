using System;
using System.Threading.Tasks;

namespace BackendServer.Publish
{
    [Obsolete("This is only meant for development.")]
    public class DummyPublishService : IPublishService
    {
        public async Task Publish()
        {
            await Task.CompletedTask;
        }
    }
}