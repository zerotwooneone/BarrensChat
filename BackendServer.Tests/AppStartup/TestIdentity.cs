using System.Security.Claims;

namespace BackendServer.Tests.AppStartup
{
    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(params Claim[] claims) : base(claims)
        {
        }
    }
}