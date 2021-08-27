<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128657224/12.1.9%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E4432)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [MainWindow.xaml](./CS/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/MainWindow.xaml))
* [MainWindow.xaml.cs](./CS/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/MainWindow.xaml.vb))
<!-- default file list end -->
# How to display custom tooltips for appointments, resource headers and date headers


<p>This example demonstrates how to display custom tooltips for appointments, resource headers and date headers. Note that we can display custom fields in the appointment and resource header tooltips. To display custom field in the appointment tooltip you can use the approach from the <a href="http://documentation.devexpress.com/#WPF/CustomDocument9452"><u>How to: Customize Appointment Tooltips</u></a> help section. Here are code/XAML snippets that illustrate how this is accomplished in the current example:<br />
</p>

```xaml
        <!--AppointmentToolTipContentTemplate-->
        <DataTemplate x:Key="AppointmentToolTipContentTemplate">
            <Grid>
                ...
                <TextBlock Grid.Row="3" Text="{Binding CustomViewInfo.Price, StringFormat={}{0:C2}}" Foreground="Red"/>
            </Grid>
        </DataTemplate>

```



```cs
        private void schedulerControl1_AppointmentViewInfoCustomizing(object sender, DevExpress.Xpf.Scheduler.AppointmentViewInfoCustomizingEventArgs e) {
            CustomAppointmentData cad = new CustomAppointmentData();
            object price = e.ViewInfo.Appointment.CustomFields["cfPrice"];

            if (price != null && price != DBNull.Value)
                cad.Price = Convert.ToDecimal(price);
            e.ViewInfo.CustomViewInfo = cad;
        }

    ...

    public class CustomAppointmentData : DependencyObject {
        public static readonly DependencyProperty PriceProperty = DependencyProperty.Register("Price", typeof(decimal), typeof(CustomAppointmentData), new PropertyMetadata(0m));
        public decimal Price { get { return (decimal)GetValue(PriceProperty); } set { SetValue(PriceProperty, value); } }
    }

```

<p>There resource header tooltip default implementation allows you to accomplish this task in XAML (without the need to write code):<br />
</p>

```xaml
        <!--HorizontalResourceHeaderStyle-->
        <Style x:Key="HorizontalResourceHeaderStyle" TargetType="{x:Type dxschint:VisualResourceHeader}">
            ...                            
                            <Grid.ToolTip>
                                ...
                                    <TextBlock Text="{Binding ResourceHeader.CustomFields[cfTrademark]}" Foreground="Red"/>
        </Style>

```

<p>In addition, we override styles for date headers and add tooltips to them as well.</p><p>Here is a screenshot that illustrates a sample application in action (we see an appointment tooltip here):</p><p><img src="https://raw.githubusercontent.com/DevExpress-Examples/how-to-display-custom-tooltips-for-appointments-resource-headers-and-date-headers-e4432/12.1.9+/media/1b10dae8-83d7-489b-9f3d-32e8704c232a.png"></p><p><strong>See Also:</strong><br />
<a href="http://documentation.devexpress.com/#WPF/CustomDocument8922"><u>Styles and Templates</u></a><br />
<a href="https://www.devexpress.com/Support/Center/p/E3450">How to customize the SchedulerControl presentation differently for different themes</a></p>

<br/>


