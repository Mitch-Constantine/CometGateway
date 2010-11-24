using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AspComet;
using AspComet.Eventing;
using Autofac;
using CometGateway.Server.TelnetDemo.AspCometMessageHandlers;
using CometGateway.Server.TelnetDemo.Controllers;
using CometGateway.Utilities;
using Microsoft.Practices.ServiceLocation;
using CometGateway.Server.Gateway;
using CometGateway.Server.Gateway.ANSIColorsSupport;


namespace CometGateway.Server.TelnetDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IContainer container;

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{action}/{id}", // URL with parameters
                new { controller = "Main", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            SetupIoCContainer();
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            TelnetProtocolTranslator.WireUp();
        }

        private static void SetupIoCContainer()
        {
            var builder = new ContainerBuilder();

            // Let AspComet put its registrations into the container
            foreach (ServiceMetadata metadata in ServiceMetadata.GetMinimumSet())
            {
                if (metadata.IsPerRequest)
                    builder.RegisterType(metadata.ActualType).As(metadata.ServiceType);
                else
                    builder.RegisterType(metadata.ActualType).As(metadata.ServiceType).SingleInstance();
            }
            builder.RegisterType<TelnetProtocolTranslator>().As<TelnetProtocolTranslator>();
            builder.RegisterType<MessageHandlerCache>().As<IMessageHandlerCache>().SingleInstance();
            builder.RegisterType<SocketConnection>().As<ISocketConnection>();
            builder.RegisterType<TelnetConnection>().As<ITelnetConnection>();
            builder.RegisterType<TelnetStateMachine>().As<ITelnetStateMachine>();
            builder.RegisterType<BytesToStringConversionLayer>()
                .As<IRawStringConnection>();
            builder.RegisterType<ANSICommandsDecoder>().As<IConnection<IANSICommand[], string>>();
            builder.RegisterType<ANSICommandStateMachine>().As<IANSICommandsStateMachine>();
            builder.RegisterType<ANSICommandToHTMLConverter>().As<IHTMLConnection>();

            // Set up the common service locator
            container = builder.Build();
        }
    }
}