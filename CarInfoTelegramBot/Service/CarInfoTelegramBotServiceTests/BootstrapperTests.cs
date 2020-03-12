﻿using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarInfoTelegramBotServiceTests
{
    [TestClass]
    public class BootstrapperTests
    {
        [TestMethod]
        public void GetDependencyInjectionContainerTest()
        {
            CarInfoTelegramBotService.Bootstrapper.BootStrap(new ContainerBuilder());
        }
    }
}