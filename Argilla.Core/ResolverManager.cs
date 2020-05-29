using System;
namespace Argilla.Core
{
    public class ResolverManager
    {
        //TODO the provider must be configurable via settings

        public static IResolver CreateInstance()
        {
            return new Resolvers.Server();
        }
    }
}
