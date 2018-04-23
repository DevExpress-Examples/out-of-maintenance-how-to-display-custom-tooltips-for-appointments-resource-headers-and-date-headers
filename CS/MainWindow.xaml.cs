using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using DevExpress.XtraScheduler;

namespace SchedulerCustomTooltipsWpf {
    public partial class MainWindow : Window {
        private CarsDBDataSet dataSet = new CarsDBDataSet();
        private CarsDBDataSetTableAdapters.CarSchedulingTableAdapter tableAdapterAppointments = new CarsDBDataSetTableAdapters.CarSchedulingTableAdapter();
        private CarsDBDataSetTableAdapters.CarsTableAdapter tableAdapterResources = new CarsDBDataSetTableAdapters.CarsTableAdapter();

        public MainWindow() {
            InitializeComponent();

            tableAdapterAppointments.Fill(dataSet.CarScheduling);
            tableAdapterResources.Fill(dataSet.Cars);

            schedulerControl1.Storage.ResourceStorage.DataSource = dataSet.Cars;
            schedulerControl1.Storage.AppointmentStorage.DataSource = dataSet.CarScheduling;

            if (schedulerControl1.Storage.AppointmentStorage.Count > 0)
                schedulerControl1.Start = schedulerControl1.Storage.AppointmentStorage[0].Start;

            schedulerControl1.Storage.AppointmentsInserted += Storage_AppointmentsModified;
            schedulerControl1.Storage.AppointmentsChanged += Storage_AppointmentsModified;
            schedulerControl1.Storage.AppointmentsDeleted += Storage_AppointmentsModified;

            tableAdapterAppointments.Adapter.RowUpdated += adapter_RowUpdated;
            
            if (schedulerControl1.Storage.AppointmentStorage.Count > 0)
                schedulerControl1.Start = schedulerControl1.Storage.AppointmentStorage[0].Start;
        }

        private void Storage_AppointmentsModified(object sender, PersistentObjectsEventArgs e) {
            this.tableAdapterAppointments.Adapter.Update(this.dataSet);
            this.dataSet.AcceptChanges();
        }

        private void adapter_RowUpdated(object sender, System.Data.OleDb.OleDbRowUpdatedEventArgs e) {
            if (e.Status == UpdateStatus.Continue && e.StatementType == StatementType.Insert) {
                int id = 0;
                using (OleDbCommand cmd = new OleDbCommand("SELECT @@IDENTITY", tableAdapterAppointments.Connection)) {
                    id = (int)cmd.ExecuteScalar();
                }
                e.Row["ID"] = id;
            }
        }

        private void schedulerControl1_AppointmentViewInfoCustomizing(object sender, DevExpress.Xpf.Scheduler.AppointmentViewInfoCustomizingEventArgs e) {
            CustomAppointmentData cad = new CustomAppointmentData();
            object price = e.ViewInfo.Appointment.CustomFields["cfPrice"];

            if (price != null && price != DBNull.Value)
                cad.Price = Convert.ToDecimal(price);
            e.ViewInfo.CustomViewInfo = cad;
        }
    }

    public class CustomAppointmentData : DependencyObject {
        public static readonly DependencyProperty PriceProperty = DependencyProperty.Register("Price", typeof(decimal), typeof(CustomAppointmentData), new PropertyMetadata(0m));
        public decimal Price { get { return (decimal)GetValue(PriceProperty); } set { SetValue(PriceProperty, value); } }
    }

    // Usage in XAML: PropertyName="{Binding ..., Converter={local:DebugConverter}}"
    /*public class DebugConverter : System.Windows.Markup.MarkupExtension, System.Windows.Data.IValueConverter {
        // Return a value without conversion
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            VisualTimelineResource res = (VisualTimelineResource)value;

            return (res != null ? res.ResourceHeader.ResourceCaption : string.Empty);
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException("DebugConverterFormatter::ConvertBack");
        }

        // Self-provided
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }*/
}