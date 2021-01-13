using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xamarin.Forms.Tabs
{
    public enum TabMode
    {
        Fixed, Scrollable
    }

    public class TabHostView : Frame
    {

        #region BindableProperty TabMode
        /// <summary>
        /// Description of property
        /// </summary>
        public static readonly BindableProperty TabModeProperty = BindableProperty.Create(nameof(TabMode), typeof(TabMode), typeof(TabHostView), TabMode.Fixed, BindingMode.TwoWay);

        /// <summary>
        /// Description of property
        /// </summary>
        public TabMode TabMode
        {
            get
            {
                return (TabMode)GetValue(TabModeProperty);
            }

            set
            {
                SetValue(TabModeProperty, value);
            }
        }
        #endregion

        #region Notified Property Tabs
        /// <summary>
        /// Tabs
        /// </summary>
        private ObservableCollection<View> tabs;
        public ObservableCollection<View> Tabs
        {
            get { return tabs; }
            set { tabs = value; }
        }
        #endregion

        #region BindableProperty SelectedIndex
        /// <summary>
        /// Description of property
        /// </summary>
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(TabHostView), default(int), BindingMode.TwoWay);

        /// <summary>
        /// Description of property
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }

            set
            {
                SetValue(SelectedIndexProperty, value);
                SelectedIndexChanged?.Invoke(this, value);
            }
        }
        #endregion

        #region BindableProperty SelectedColor
        /// <summary>
        /// Description of property
        /// </summary>
        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(TabHostView), Color.Accent, BindingMode.OneWay);

        /// <summary>
        /// Description of property
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                return (Color)GetValue(SelectedColorProperty);
            }

            set
            {
                SetValue(SelectedColorProperty, value);
            }
        }
        #endregion

        #region BindableProperty UnselectedColor
        /// <summary>
        /// Description of property
        /// </summary>
        public static readonly BindableProperty UnselectedColorProperty = BindableProperty.Create(nameof(UnselectedColor), typeof(Color), typeof(TabHostView), Color.Gray, BindingMode.OneWay);

        /// <summary>
        /// Description of property
        /// </summary>
        public Color UnselectedColor
        {
            get
            {
                return (Color)GetValue(UnselectedColorProperty);
            }

            set
            {
                SetValue(UnselectedColorProperty, value);
            }
        }
        #endregion

        public event EventHandler<int> SelectedIndexChanged;

        public ITabHostContent TabHostContent { get; set; }

        public TabHostView()
        {
            Padding = 0;
            BindableProperties = new List<BindablePropertyChanged>
            {
                new BindablePropertyChanged(SelectedIndexProperty, () =>
                {
                    System.Diagnostics.Debug.WriteLine("TabHostView");
                    System.Diagnostics.Debug.WriteLine(SelectedIndex);
                }),
                new BindablePropertyChanged(TabModeProperty, () =>
                {
                    TabHostContent = GetTabContent();
                    Content = TabHostContent as View;
                }),
                new BindablePropertyChanged(SelectedColorProperty, () => SetFirstTab()),
                new BindablePropertyChanged(UnselectedColorProperty, () => SetFirstTab())
            };
            TabHostContent = GetTabContent();
            Tabs = new ObservableCollection<View>();
            Tabs.CollectionChanged += Tabs_CollectionChanged;
            Content = TabHostContent as View;
        }

        private ITabHostContent GetTabContent()
        {
            ITabHostContent content;
            switch (TabMode)
            {
                default:
                case TabMode.Fixed:
                    content = new TabHostFixContent();
                    break;
                case TabMode.Scrollable:
                    content = new TabHostScrollableContent();
                    break;
            }
            return content;
        }

        private void Tabs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var tab = item as ISimpleTab;
                        if (tab == null) continue;
                        tab.Unselect(UnselectedColor);
                        if (!(tab is View view)) return;
                        view.HeightRequest = HeightRequest;
                        view.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            NumberOfTapsRequired = 1,
                            Command = new Command<ISimpleTab>(TapSelected),
                            CommandParameter = item
                        });
                        TabHostContent.Add(view);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    TabHostContent.Clear();
                    foreach (var tab in Tabs)
                    {
                        tab.HeightRequest = HeightRequest;
                        tab.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            NumberOfTapsRequired = 1,
                            Command = new Command<ISimpleTab>(TapSelected),
                            CommandParameter = tab
                        });
                        TabHostContent.Add(tab);
                    }
                    break;
            }

            SetFirstTab();
        }

        private void SetFirstTab()
        {
            System.Diagnostics.Debug.WriteLine("SetFirstTab");
            if (Tabs == null || Tabs.Count == 0) return;
            for (int i = 0; i < Tabs.Count; i++)
            {
                var tab = Tabs[i] as ISimpleTab;
                if (i == 0)
                    tab.Select(SelectedColor);
                else
                    tab.Unselect(UnselectedColor);
            }
        }

        private void TapSelected(ISimpleTab tab)
        {
            if (Tabs == null || Tabs.Count == 0) return;
            foreach (var item in Tabs)
            {
                var simpletab = (item as ISimpleTab);
                simpletab.Unselect(UnselectedColor);
            }
            tab.Select(SelectedColor);
            SelectedIndex = Tabs.IndexOf(tab as View);
        }

        #region Bindable Property Changed
        public List<BindablePropertyChanged> BindableProperties { get; set; }
        protected override void OnPropertyChanged(string propertyname = null)
        {
            base.OnPropertyChanged(propertyname);
            if (BindableProperties == null) return;
            BindableProperties.FirstOrDefault(b => b.PropertyChanged(propertyname));
        }

        public class BindablePropertyChanged
        {
            protected string PropetyName
            {
                get
                {
                    return SourceProperty?.PropertyName;
                }
            }
            protected BindableProperty SourceProperty;
            protected Action Action { get; set; }

            public BindablePropertyChanged(BindableProperty sourceproperty, Action action)
            {
                SourceProperty = sourceproperty;
                Action = action;
            }

            public bool PropertyChanged(string propertyname)
            {
                if (SourceProperty.PropertyName != propertyname) return false;
                Action?.Invoke();
                return true;
            }
        }
        #endregion
    }

    public class TabHostFixContent : Grid, ITabHostContent
    {
        private int Column { get; set; } = 0;

        public void Add(View view)
        {
            SetColumn(view, Column);
            Children.Add(view);
            Column++;
        }

        public void Clear()
        {
            Children.Clear();
        }

        public void Remove(View view)
        {
            Children.Remove(view);
        }
    }

    public class TabHostScrollableContent : ScrollView, ITabHostContent
    {
        public TabHostScrollableContent()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            Orientation = ScrollOrientation.Horizontal;
            Margin = new Thickness(0);
            Padding = new Thickness(0);
            Content = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0
            };
        }

        public void Add(View view)
        {
            (Content as StackLayout).Children.Add(view);
        }

        public void Clear()
        {
            (Content as StackLayout).Children.Clear();
        }

        public void Remove(View view)
        {
            (Content as StackLayout).Children.Remove(view);
        }
    }

    public interface ITabHostContent
    {
        void Add(View view);

        void Remove(View view);

        void Clear();
    }

    public interface ISimpleTab
    {
        void Unselect(Color color);

        void Select(Color color);
    }

    public class SimpleTab : Grid, ISimpleTab
    {
        #region BindableProperty Text
        /// <summary>
        /// Description of property
        /// </summary>
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SimpleTab), default(string), BindingMode.TwoWay);

        /// <summary>
        /// Description of property
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }
        #endregion

        private Label Label { get; } = new Label();

        public SimpleTab()
        {
            Padding = new Thickness(15, 0);
            Label.HorizontalTextAlignment = TextAlignment.Center;
            Label.VerticalTextAlignment = TextAlignment.Center;
            Label.LineBreakMode = LineBreakMode.TailTruncation;
            Children.Add(Label);
            BindableProperties = new List<BindablePropertyChanged>
            {
                new BindablePropertyChanged(TextProperty, () => Label.Text = Text)
            };
        }

        public void Unselect(Color color)
        {
            Label.TextColor = color;
        }

        public void Select(Color color)
        {
            Label.TextColor = color;
        }

        #region Bindable Property Changed
        public List<BindablePropertyChanged> BindableProperties { get; set; }
        protected override void OnPropertyChanged(string propertyname = null)
        {
            base.OnPropertyChanged(propertyname);
            if (BindableProperties == null) return;
            BindableProperties.FirstOrDefault(b => b.PropertyChanged(propertyname));
        }

        public class BindablePropertyChanged
        {
            protected string PropetyName
            {
                get
                {
                    return SourceProperty?.PropertyName;
                }
            }
            protected BindableProperty SourceProperty;
            protected Action Action { get; set; }

            public BindablePropertyChanged(BindableProperty sourceproperty, Action action)
            {
                SourceProperty = sourceproperty;
                Action = action;
            }

            public bool PropertyChanged(string propertyname)
            {
                if (SourceProperty.PropertyName != propertyname) return false;
                Action?.Invoke();
                return true;
            }
        }
        #endregion

    }

    public class ViewSwitcher : Grid, IList<View>
    {
        private int OldSelectedIndex { get; set; }

        #region BindableProperty SelectedIndex
        /// <summary>
        /// Description of property
        /// </summary>
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(ViewSwitcher), default(int), BindingMode.TwoWay);

        /// <summary>
        /// Description of property
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }

            set
            {
                OldSelectedIndex = SelectedIndex;
                SetValue(SelectedIndexProperty, value);
            }
        }
        #endregion

        public event EventHandler<ViewChangedArgs> ViewChanged;

        public ViewSwitcher()
        {
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            BindableProperties = new List<BindablePropertyChanged>
            {
                new BindablePropertyChanged(SelectedIndexProperty, SelectedIndex_Property)
            };
        }

        private void SelectedIndex_Property()
        {
            System.Diagnostics.Debug.WriteLine("ViewSwitcher");
            System.Diagnostics.Debug.WriteLine(OldSelectedIndex);
            System.Diagnostics.Debug.WriteLine(SelectedIndex);
            if (OldSelectedIndex >= Count || SelectedIndex >= Count) return;
            if (OldSelectedIndex != SelectedIndex)
            {
                var previus = Children[OldSelectedIndex];
                previus.IsVisible = false;
                Unload(previus);
            }
            var current = Children[SelectedIndex];
            current.IsVisible = true;
            Load(current);
            ViewChanged?.Invoke(this, new ViewChangedArgs
            {
                OldSelectedIndex = OldSelectedIndex,
                SelectedIndex = SelectedIndex,
                CurrentView = current
            });
        }

        private void Load(View current)
        {
            if (current is ISelectedTab view)
                view.Appearing();
            else if (current.BindingContext is ISelectedTab viewmodel)
                viewmodel.Appearing();
        }

        private void Unload(View previus)
        {
            if (previus is ISelectedTab view)
                view.Disappearing();
            else if (previus.BindingContext is ISelectedTab viewmodel)
                viewmodel.Disappearing();
        }

        #region IList
        public View this[int index] { get => Children[index]; set => Insert(index, value); }

        public int Count => Children.Count;

        public bool IsReadOnly => false;

        public void Add(View item)
        {
            if (Count > 0)
                item.IsVisible = false;
            Children.Add(item);
            if (Count == 1)
                Load(item);
        }

        public void Clear()
        {
            Children.Clear();
        }

        public bool Contains(View item)
        {
            return Children.Contains(item);
        }

        public void CopyTo(View[] array, int arrayIndex)
        {
            Children.CopyTo(array, arrayIndex);
        }

        public IEnumerator<View> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public int IndexOf(View item)
        {
            return Children.IndexOf(item);
        }

        public void Insert(int index, View item)
        {
            item.IsVisible = false;
            Children.Insert(index, item);
        }

        public bool Remove(View item)
        {
            return Children.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Children.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default(IEnumerator);
        }
        #endregion

        #region Bindable Property Changed
        public List<BindablePropertyChanged> BindableProperties { get; set; }
        protected override void OnPropertyChanged(string propertyname = null)
        {
            base.OnPropertyChanged(propertyname);
            if (BindableProperties == null) return;
            BindableProperties.FirstOrDefault(b => b.PropertyChanged(propertyname));
        }

        public class BindablePropertyChanged
        {
            protected string PropetyName
            {
                get
                {
                    return SourceProperty?.PropertyName;
                }
            }
            protected BindableProperty SourceProperty;
            protected Action Action { get; set; }

            public BindablePropertyChanged(BindableProperty sourceproperty, Action action)
            {
                SourceProperty = sourceproperty;
                Action = action;
            }

            public bool PropertyChanged(string propertyname)
            {
                if (SourceProperty.PropertyName != propertyname) return false;
                Action?.Invoke();
                return true;
            }
        }
        #endregion
    }

    public class ViewChangedArgs
    {
        public int OldSelectedIndex { get; set; }
        public int SelectedIndex { get; set; }
        public View CurrentView { get; set; }
    }

    public interface ISelectedTab
    {
        void Appearing();
        void Disappearing();
    }
}