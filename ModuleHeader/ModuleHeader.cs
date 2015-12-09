/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Microsoft.Practices.Unity;
using ModuleHeader.Views;
using Prism.Modularity;
using Prism.Regions;

namespace ModuleHeader
{
    public class ModuleHeader : IModule
    {
        IUnityContainer _container;
        IRegionManager _regionManager;

        public ModuleHeader(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {            
            _regionManager.RegisterViewWithRegion("HeaderRegion", typeof(HeaderView));
        }
    }
}
