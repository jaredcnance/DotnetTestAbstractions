using Autofac;
using Autofac.Core;

namespace DotnetTestAbstractions.DependencyInjection
{
    public class InterceptorModule : Module
    {
        private readonly IServiceInterceptor _interceptor;

        public InterceptorModule(IServiceInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        protected override void AttachToComponentRegistration(
            IComponentRegistry componentRegistry,
            IComponentRegistration registration
        ) => registration.Activating += OnComponentActivating;

        private void OnComponentActivating(object sender, ActivatingEventArgs<object> e)
        {
            var originalInstance = e.Instance;
            var newInstance = _interceptor.OnResolving(originalInstance, e.Context);
            if(newInstance != originalInstance) {
                e.ReplaceInstance(newInstance);
            }
        }
    }
}