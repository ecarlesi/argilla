using System.Security;
using Argilla.Core.Common.Entities;
using Argilla.Server.Entities;

namespace Argilla.Server.Managers
{
    public class DummySecurityManager : ISecurityManager
    {
        public void Verify(SecurityAssertion securityAssertion)
        {
            if (securityAssertion == null && securityAssertion.Payload == null)
            {
                return;
            }

            ObjectWithProperties owp = securityAssertion.Payload as ObjectWithProperties;

            if (owp == null)
            {
                return;
            }

            if (owp.Properties != null && owp.Properties.ContainsKey("fail"))
            {
                throw new SecurityException("Access denied");
            }
        }
    }
}
