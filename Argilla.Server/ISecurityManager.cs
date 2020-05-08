using Argilla.Server.Entities;

namespace Argilla.Server
{
    internal interface ISecurityManager
    {
        void Verify(SecurityAssertion securityAssertion);
    }
}