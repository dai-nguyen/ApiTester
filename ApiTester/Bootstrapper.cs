/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Prism.Modularity;
using Microsoft.Practices.Unity;
using System.Windows;
using Prism.Unity;

namespace ApiTester
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();            
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void InitializeModules()
        {
            IModule moduleHeader = Container.Resolve<ModuleHeader.ModuleHeader>();
            IModule moduleFooter = Container.Resolve<ModuleFooter.ModuleFooter>();
            IModule moduleLeft = Container.Resolve<ModuleLeftPane.ModuleLeftPane>();
            IModule moduleRight = Container.Resolve<ModuleRightPane.ModuleRightPane>();

            moduleHeader.Initialize();
            moduleFooter.Initialize();
            moduleLeft.Initialize();
            moduleRight.Initialize();
        }
    }
}
