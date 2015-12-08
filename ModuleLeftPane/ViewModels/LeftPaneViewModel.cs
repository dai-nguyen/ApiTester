using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleLeftPane.ViewModels
{
    public class LeftPaneViewModel : BindableBase
    {
        private string _filename;
        public string Filename
        {
            get { return _filename; }
            set { SetProperty(ref _filename, value); }
        }

        public LeftPaneViewModel()
        {
            Filename = "Filename";
        }
    }
}
