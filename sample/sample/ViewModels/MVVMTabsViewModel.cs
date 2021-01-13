using Xamarin.Forms;

namespace sample
{
    public class MVVMTabsViewModel : BindableObject
    {
        #region Notified Property CurrentTabIndex
        /// <summary>
        /// Current Tab Index
        /// </summary>
        private int currenttabindex;
        public int CurrentTabIndex
        {
            get { return currenttabindex; }
            set { currenttabindex = value; OnPropertyChanged(); }
        }
        #endregion
    }
}