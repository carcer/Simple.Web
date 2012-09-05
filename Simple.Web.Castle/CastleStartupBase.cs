using Castle.MicroKernel;
using Castle.Windsor;
using Simple.Web.DependencyInjection;

namespace Simple.Web.Castle
{
    public abstract class CastleStartupBase : IStartupTask
    {
        public void Run(IConfiguration configuration, IWebEnvironment environment)
        {
            var container = CreateContainer();
            configuration.Container = new CastleContainer(container);
        }

        protected abstract IWindsorContainer CreateContainer();
    }

    public class CastleContainer : ISimpleContainer
    {
        private readonly IWindsorContainer container;

        public CastleContainer(IWindsorContainer container)
        {
            this.container = container;
        }

        public ISimpleContainerScope BeginScope()
        {
            return new CastleContainerScope(container);
        }
    }

    public class CastleContainerScope : ISimpleContainerScope
    {
        private readonly IWindsorContainer container;

        public CastleContainerScope(IWindsorContainer container)
        {
            this.container = container;
        }

        public void Dispose()
        {
            container.Dispose();
        }

        public T Get<T>()
        {
            return container.Resolve<T>();
        }
    }
}