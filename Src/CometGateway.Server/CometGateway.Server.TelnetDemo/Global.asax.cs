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
using CometGateway.Utilities;
using Microsoft.Practices.ServiceLocation;
using System.Threading;
using AspComet.Samples.Chat;

namespace CometGateway.Server.TelnetDemo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static IContainer container;

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            SetupIoCContainer();
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            new Thread(ThreadProc).Start();
        }



        private static void ThreadProc()
        {
            while (true)
            {
                Debug.WriteLine("Thread");
                var sender = container.Resolve<MessageSender>();
                sender.SendMessage();
                Thread.Sleep(1000);
            }
        }

        static void OnEventPublished(PublishingEvent e)
        {
            var clientRepository = container.Resolve<IClientRepository>();
            var client = clientRepository.GetByID(e.Message.clientId);
            var data = e.Message.data as IDictionary<string, object>;
            var message = new Message()
            {
                channel = "/telnet",
                data = new { sender = "auto", message = "You said:" + data["message"] + "<br>" }
            };
            client.Enqueue(message);
            client.FlushQueue();
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

            builder.RegisterType(typeof (MessageSender)).As(typeof (MessageSender));
            EventHub.Subscribe<PublishingEvent>(OnEventPublished);

            // Set up the common service locator
            container = builder.Build();
        }
    }
}