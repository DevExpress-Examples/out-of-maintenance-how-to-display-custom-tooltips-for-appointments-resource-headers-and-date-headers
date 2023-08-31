Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.Windows
Imports DevExpress.XtraScheduler

Namespace SchedulerCustomTooltipsWpf

    Public Partial Class MainWindow
        Inherits Window

        Private dataSet As CarsDBDataSet = New CarsDBDataSet()

        Private tableAdapterAppointments As CarsDBDataSetTableAdapters.CarSchedulingTableAdapter = New CarsDBDataSetTableAdapters.CarSchedulingTableAdapter()

        Private tableAdapterResources As CarsDBDataSetTableAdapters.CarsTableAdapter = New CarsDBDataSetTableAdapters.CarsTableAdapter()

        Public Sub New()
            Me.InitializeComponent()
            tableAdapterAppointments.Fill(dataSet.CarScheduling)
            tableAdapterResources.Fill(dataSet.Cars)
            Me.schedulerControl1.Storage.ResourceStorage.DataSource = dataSet.Cars
            Me.schedulerControl1.Storage.AppointmentStorage.DataSource = dataSet.CarScheduling
            If Me.schedulerControl1.Storage.AppointmentStorage.Count > 0 Then Me.schedulerControl1.Start = Me.schedulerControl1.Storage.AppointmentStorage(0).Start
            AddHandler Me.schedulerControl1.Storage.AppointmentsInserted, AddressOf Me.Storage_AppointmentsModified
            AddHandler Me.schedulerControl1.Storage.AppointmentsChanged, AddressOf Me.Storage_AppointmentsModified
            AddHandler Me.schedulerControl1.Storage.AppointmentsDeleted, AddressOf Me.Storage_AppointmentsModified
            AddHandler tableAdapterAppointments.Adapter.RowUpdated, AddressOf Me.adapter_RowUpdated
            If Me.schedulerControl1.Storage.AppointmentStorage.Count > 0 Then Me.schedulerControl1.Start = Me.schedulerControl1.Storage.AppointmentStorage(0).Start
        End Sub

        Private Sub Storage_AppointmentsModified(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
            tableAdapterAppointments.Adapter.Update(dataSet)
            dataSet.AcceptChanges()
        End Sub

        Private Sub adapter_RowUpdated(ByVal sender As Object, ByVal e As OleDbRowUpdatedEventArgs)
            If e.Status = UpdateStatus.Continue AndAlso e.StatementType = StatementType.Insert Then
                Dim id As Integer = 0
                Using cmd As OleDbCommand = New OleDbCommand("SELECT @@IDENTITY", tableAdapterAppointments.Connection)
                    id = CInt(cmd.ExecuteScalar())
                End Using

                e.Row("ID") = id
            End If
        End Sub

        Private Sub schedulerControl1_AppointmentViewInfoCustomizing(ByVal sender As Object, ByVal e As DevExpress.Xpf.Scheduler.AppointmentViewInfoCustomizingEventArgs)
            Dim cad As CustomAppointmentData = New CustomAppointmentData()
            Dim price As Object = e.ViewInfo.Appointment.CustomFields("cfPrice")
            If price IsNot Nothing AndAlso price IsNot DBNull.Value Then cad.Price = Convert.ToDecimal(price)
            e.ViewInfo.CustomViewInfo = cad
        End Sub
    End Class

    Public Class CustomAppointmentData
        Inherits DependencyObject

        Public Shared ReadOnly PriceProperty As DependencyProperty = DependencyProperty.Register("Price", GetType(Decimal), GetType(CustomAppointmentData), New PropertyMetadata(0D))

        Public Property Price As Decimal
            Get
                Return CDec(GetValue(PriceProperty))
            End Get

            Set(ByVal value As Decimal)
                SetValue(PriceProperty, value)
            End Set
        End Property
    End Class
' Usage in XAML: PropertyName="{Binding ..., Converter={local:DebugConverter}}"
' public class DebugConverter : System.Windows.Markup.MarkupExtension, System.Windows.Data.IValueConverter {
' // Return a value without conversion
' public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
' VisualTimelineResource res = (VisualTimelineResource)value;
' 
' return (res != null ? res.ResourceHeader.ResourceCaption : string.Empty);
' }
' 
' // No need to implement converting back on a one-way binding 
' public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
' throw new NotImplementedException("DebugConverterFormatter::ConvertBack");
' }
' 
' // Self-provided
' public override object ProvideValue(IServiceProvider serviceProvider) {
' return this;
' }
' }
End Namespace
