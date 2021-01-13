### SETUP

Install nuget into Shared Project
```
Install-Package Xamarin.Forms.Tabs -Version 1.0.0
```

### Usage

Reference the following xmlns to your page:
```
xmlns:Tabs="clr-namespace:Xamarin.Forms.Tabs;assembly=Xamarin.Forms.Tabs"
```

### CodeBehind

Now to create the tabs, you need to add this code to your view.

```
<Tabs:TabHostView SelectedColor="White" UnselectedColor="Gray" BackgroundColor="DodgerBlue" 
    HeightRequest="45" TabMode="Fixed"
    SelectedIndexChanged="TabHostView_SelectedIndexChanged">
    <Tabs:TabHostView.Tabs>
        <!-- Tabs -->
        <Tabs:SimpleTab Text="Tab 1" />
        <Tabs:SimpleTab Text="Tab 2" />
    </Tabs:TabHostView.Tabs>
</Tabs:TabHostView>

<ScrollView>
    <Tabs:ViewSwitcher x:Name="Switcher" ViewChanged="Switcher_ViewChanged">
        <StackLayout>
            <!-- Tab Content 1 -->
            <Label Text="1" />
        </StackLayout>
        <Grid>
            <!-- Tab Content 2 -->
            <Label Text="2" />
        </Grid>
    </Tabs:ViewSwitcher>
</ScrollView>
```

Finally, be sure to change the ViewSwitcher SelectedIndex in the C# of page

```
private void TabHostView_SelectedIndexChanged(object sender, int e)
{
    Switcher.SelectedIndex = e;
}
```

### MVVM

To use this with MVVM, you can use the following code in XAML [don't forget to create a property called CurrentTabIndex that implements the OnPropertyChanged() method in the SET action]

```
<Tabs:TabHostView SelectedColor="White" UnselectedColor="Gray" BackgroundColor="DodgerBlue" 
    HeightRequest="45" TabMode="Fixed"
    SelectedIndex="{Binding Source={x:Reference Switcher}, Path=SelectedIndex, Mode=TwoWay}">
    <Tabs:TabHostView.Tabs>
        <!-- Tabs -->
        <Tabs:SimpleTab Text="Tab 1" />
        <Tabs:SimpleTab Text="Tab 2" />
    </Tabs:TabHostView.Tabs>
</Tabs:TabHostView>

<ScrollView>
    <Tabs:ViewSwitcher x:Name="Switcher" SelectedIndex="{Binding CurrentTabIndex, Mode=TwoWay}">
        <StackLayout>
            <!-- Tab Content 1 -->
            <Label Text="1" />
        </StackLayout>
        <Grid>
            <!-- Tab Content 2 -->
            <Label Text="2" />
        </Grid>
    </Tabs:ViewSwitcher>
</ScrollView>
```

