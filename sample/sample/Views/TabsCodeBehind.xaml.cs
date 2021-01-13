using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace sample
{
    public partial class TabsCodeBehind : ContentPage
    {
        public TabsCodeBehind()
        {
            InitializeComponent();
        }

        private void TabHostView_SelectedIndexChanged(object sender, int e)
        {
            Switcher.SelectedIndex = e;
        }

        private void Switcher_ViewChanged(object sender, Xamarin.Forms.Tabs.ViewChangedArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.OldSelectedIndex);
            System.Diagnostics.Debug.WriteLine(e.SelectedIndex);
            System.Diagnostics.Debug.WriteLine(e.CurrentView.GetType().Name);
        }
    }
}
