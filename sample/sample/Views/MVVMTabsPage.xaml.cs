using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MVVMTabsPage : ContentPage
    {
        public MVVMTabsPage()
        {
            InitializeComponent();
        }
    }
}