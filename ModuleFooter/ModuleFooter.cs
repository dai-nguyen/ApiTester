/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Microsoft.Practices.Unity;
using ModuleFooter.Views;
using Prism.Modularity;
using Prism.Regions;

namespace ModuleFooter
{
    public class ModuleFooter : IModule
    {
        IUnityContainer _container;
        IRegionManager _regionManager;

        public ModuleFooter(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {            
            _regionManager.RegisterViewWithRegion("FooterRegion", typeof(FooterView));
        }
    }
}
