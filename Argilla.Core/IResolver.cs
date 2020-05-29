using System;
using Argilla.Core.Common.Entities;

namespace Argilla.Core
{
    public interface IResolver
    {
        ResolveResponse Resolve(string serviceName);
        RegisterResponse Register(RegisterRequest registerRequest);
        RegisterResponse Unregister(RegisterRequest registerRequest);
    }
}
