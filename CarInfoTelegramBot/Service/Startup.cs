﻿using System;
using System.ServiceProcess;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using LoggerCommon;
using Service.Configuration;
using TelegramBots;

namespace Service
{
    public class Startup
    {
        private static ILogger _logger;

        public static void Main(string[] args)
        {
            // This is global error handler
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            IContainer container = CreateContainer();

            var serviceLocator = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            _logger = ServiceLocator.Current.GetInstance<ILogger>();
            _logger.Info($"{nameof(Startup)}|starting");

            var factory = ServiceLocator.Current.GetInstance<ITelegramBotsFactory>();

            // This is configuration provider
            var carInfoConfig = ServiceLocator.Current.GetInstance<ICarInfoConfiguration>();
            var englishConfig = ServiceLocator.Current.GetInstance<IEnglishConfiguration>();


            var svc = new MainService(new[]
            {
                factory.GetCarInfoService(carInfoConfig.Token),
                factory.GetEnglishService(englishConfig.Token),
            });

            if (Array.IndexOf(args, "console") != -1 || Array.IndexOf(args, "c") != -1)
            {
                svc.StartSvc();
                _logger.Info($"{nameof(Startup)}|started in console mode");
                Console.WriteLine("Press a key for exit...");
                Console.ReadKey(true);
                svc.StopSvc();
                _logger.Info($"{nameof(Startup)}|stoped");
            }
            else
            {
                _logger.Info($"{nameof(Startup)}|started in service mode");
                ServiceBase.Run(svc);
                _logger.Info($"{nameof(Startup)}|stoped");
            }
            
            _logger.Shutdown();
        }

        private static IContainer CreateContainer()
        {
            // Create your builder.
            var builder = new ContainerBuilder();
            new Bootstrapper().BootStrap(builder);
            return builder.Build();
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var ex = (Exception)args.ExceptionObject;
            _logger.Error(ex, nameof(UnhandledException));
        }
    }
}