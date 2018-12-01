using System;
using Autofac;

namespace DotnetTestAbstractions.DependencyInjection
{
    public interface IServiceInterceptor
    {
        object OnResolving(Object service, IComponentContext context);
    }
}