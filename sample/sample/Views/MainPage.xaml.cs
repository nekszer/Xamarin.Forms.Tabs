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
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void MVVM_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MVVMTabsPage
            {
                BindingContext = new MVVMTabsViewModel()
            });
        }

        private void CodeBehind_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabsCodeBehind());
        }
    }
}