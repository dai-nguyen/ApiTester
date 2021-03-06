﻿/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Microsoft.Practices.Unity;
using ModuleLeftPane.Views;
using Prism.Modularity;
using Prism.Regions;

namespace ModuleLeftPane
{
    public class ModuleLeftPane : IModule
    {
        IUnityContainer _container;
        IRegionManager _regionManager;

        public ModuleLeftPane(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("LeftRegion", typeof(LeftPaneView));
        }
    }
}
