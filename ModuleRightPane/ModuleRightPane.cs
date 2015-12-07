using Microsoft.Practices.Unity;
using ModuleRightPane.Views;
using Prism.Modularity;
using Prism.Regions;

namespace ModuleRightPane
{
    public class ModuleRightPane : IModule
    {
        IUnityContainer _container;
        IRegionManager _regionManager;

        public ModuleRightPane(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("RightRegion", typeof(RightPaneView));
        }
    }
}
