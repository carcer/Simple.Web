using System;
using System.Collections.Generic;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Simple.Web.Castle.Tests
{
    using Xunit;
    using CodeGeneration;

    public class HandlerFactoryBuilderTests
    {
        [Fact]
        public void CreatesInstanceOfType()
        {
            var startup = new TestStartup();
            startup.Run(SimpleWeb.Configuration, SimpleWeb.Environment);
            var target = new HandlerBuilderFactory(SimpleWeb.Configuration);
            var actualFunc = target.BuildHandlerBuilder(typeof(TestHandler));
            var actual = (TestHandler)actualFunc(new Dictionary<string, string[]> { { "TestProperty", new[] { "Foo" } } }).Handler;

            Assert.Equal(Status.OK, actual.Get());
            Assert.Equal("Foo", actual.TestProperty);
        }

        [Fact]
        public void DisposesInstances()
        {
            var startup = new TestStartup();
            startup.Run(SimpleWeb.Configuration, SimpleWeb.Environment);
            var target = new HandlerBuilderFactory(SimpleWeb.Configuration);
            var actualFunc = target.BuildHandlerBuilder(typeof(TestHandler));

            TestHandler handler;
            using (var scopedHandler = actualFunc(new Dictionary<string, string[]>()))
            {
                handler = (TestHandler)scopedHandler.Handler;
                Assert.Equal(false, handler.IsDisposed);
            }
            Assert.Equal(true, handler.IsDisposed);
        }
    }

    public class TestHandler : IGet, IDisposable
    {
        private readonly IResult _result;
        public bool IsDisposed { get; set; }

        public TestHandler(IResult result)
        {
            _result = result;
        }

        public Status Get()
        {
            return _result.Result;
        }

        public string TestProperty { get; set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    public interface IResult
    {
        Status Result { get; }
    }

    public class OkResult : IResult
    {
        public Status Result { get { return Status.OK; } }
    }

    public class TestStartup: CastleStartupBase
    {
        protected override IWindsorContainer CreateContainer()
        {
            var windsorContainer = new WindsorContainer();

            windsorContainer.Register(AllTypes.FromThisAssembly().RegisterHandler());

            windsorContainer.Register(Component.For<IResult>().ImplementedBy<OkResult>());

            return windsorContainer;
        }
    }

    public static class RegisterHandlersExtension
    {
        public static BasedOnDescriptor RegisterHandler(this FromAssemblyDescriptor descriptor)
        {
            return descriptor.Where(t =>
                                    t.IsAssignableTo<IDelete>() ||
                                    t.IsAssignableTo<IDeleteAsync>() ||
                                    t.IsAssignableTo<IGet>() ||
                                    t.IsAssignableTo<IGetAsync>() ||
                                    t.IsAssignableTo<IHead>() ||
                                    t.IsAssignableTo<IHeadAsync>() ||
                                    t.IsAssignableTo<IPatch>() ||
                                    t.IsAssignableTo<IPatchAsync>() ||
                                    t.IsAssignableTo<IPost>() ||
                                    t.IsAssignableTo<IPostAsync>() ||
                                    t.IsAssignableTo<IPut>() ||
                                    t.IsAssignableTo<IPutAsync>());
        }

        private static bool IsAssignableTo<T>(this Type @this)
        {
            if (@this == null)
                throw new ArgumentNullException("this");

            return typeof(T).IsAssignableFrom(@this);
        }
    }
}
