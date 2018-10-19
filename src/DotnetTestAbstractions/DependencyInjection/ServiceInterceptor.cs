using System;

namespace DotnetTestAbstractions.DependencyInjection
{
    public interface IServiceInterceptor
    {
        object OnResolving(Object service);
    }
}