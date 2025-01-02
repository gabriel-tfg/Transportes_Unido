using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Transportes.Core;


namespace Transportes.UI;

public partial class DataViewFlota : Window
{
    private Vehiculo _vehiculo;
    private bool _isEditing;

    public Vehiculo GetVehiculo()
    {
        return _vehiculo;
    }

    public DataViewFlota(Vehiculo vehiculo)
    {
        InitializeComponent();
        _vehiculo = vehiculo;
        DataContext = _vehiculo; // Asegura que el binding funcione correctamente
        _isEditing = false;
        SetFieldsEnabled(false);
        SetComodidades();
    }
    private void SetComodidades()
    {
        chkWifi.IsChecked = _vehiculo.Comodidades.Contains("Wifi");
        chkBluetooth.IsChecked = _vehiculo.Comodidades.Contains("Conexión del móvil por Bluetooth");
        chkAireAcondicionado.IsChecked = _vehiculo.Comodidades.Contains("Aire Acondicionado");
        chkLitera.IsChecked = _vehiculo.Comodidades.Contains("Litera de Descanso");
        chkTV.IsChecked = _vehiculo.Comodidades.Contains("TV");
    }

    public class BooleanToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameters = parameter.ToString()?.Split(',');
            return (bool)value ? parameters?[0] : parameters?[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

   private void OnEditarClick(object? sender, RoutedEventArgs e)
{
    var bEditar = this.FindControl<Button>("ButtonEditar");
    var matriculaTextBox = this.FindControl<TextBox>("MatriculaTextBox");
    var tipoComboBox = this.FindControl<ComboBox>("TipoVehiculoComboBox");

    if (_isEditing)
    {
        // Guardar las comodidades seleccionadas
        _vehiculo.Comodidades = new List<string>();
        if (chkWifi.IsChecked == true) _vehiculo.Comodidades.Add("Wifi");
        if (chkBluetooth.IsChecked == true) _vehiculo.Comodidades.Add("Conexión del móvil por Bluetooth");
        if (chkAireAcondicionado.IsChecked == true) _vehiculo.Comodidades.Add("Aire Acondicionado");
        if (chkLitera.IsChecked == true) _vehiculo.Comodidades.Add("Litera de Descanso");
        if (chkTV.IsChecked == true) _vehiculo.Comodidades.Add("TV");

        // Guardar otros campos como ya tienes
        _vehiculo.Matricula = this.FindControl<TextBox>("MatriculaTextBox").Text;
        _vehiculo.Marca = this.FindControl<TextBox>("MarcaTextBox").Text;
        _vehiculo.Modelo = this.FindControl<TextBox>("ModeloTextBox").Text;
        _vehiculo.ConsumoPorKm = double.TryParse(this.FindControl<TextBox>("ConsumoPorKmTextBox").Text, out var consumo)
            ? consumo : 0;
        _vehiculo.FechaAdquisicion = this.FindControl<DatePicker>("FechaAdquisicionPicker").SelectedDate?.DateTime 
                                     ?? _vehiculo.FechaAdquisicion;
        _vehiculo.FechaFabricacion = this.FindControl<DatePicker>("FechaFabricacionPicker").SelectedDate?.DateTime 
                                     ?? _vehiculo.FechaFabricacion;
    

        // Guardar en XML y actualizar el DataGrid
        if (Owner is MainWindow mainWindow)
        {
            mainWindow.UpdateDataGrid(mainWindow.Vehiculos, "DgridFlota");
            var xmlExporter = new XmlExporter();
            xmlExporter.ExportarFlotaXML(mainWindow.Vehiculos, Path.Combine(mainWindow.rutaXml, "flota.xml"));
        }

        bEditar.Content = "Editar";
        SetFieldsEnabled(false);
    }
    else
    {
        bEditar.Content = "Guardar";
        SetFieldsEnabled(true);
    }

    _isEditing = !_isEditing;
}



private bool EsFormatoMatriculaValido(string matricula)
{
    // Patrón: 3 letras mayúsculas seguidas de un espacio y 4 números (ejemplo: ABC 1234)
    var patron = @"^[A-Z]{3} \d{4}$";
    return System.Text.RegularExpressions.Regex.IsMatch(matricula, patron);
}


    private async void OnEliminarClick(object? sender, RoutedEventArgs e)
    {
        var confirmDialog = new Window
        {
            Title = "Confirmación",
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var stackPanel = new StackPanel
        {
            Margin = new Thickness(10)
        };

        var textBlock = new TextBlock
        {
            Text = "¿Está seguro de que desea eliminar este vehículo?",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 20)
        };

        var buttonPanel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Spacing = 10
        };

        // Sí button
        var yesButton = new Button
        {
            Content = "Sí",
            Width = 75,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };
        yesButton.Click += async (_, _) =>
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                DeleteVehiculo();
                confirmDialog.Close();
            });
        };

        // No button
        var noButton = new Button
        {
            Content = "No",
            Width = 75,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };
        noButton.Click += async (_, _) =>
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                confirmDialog.Close();
            });
        };

        // Assemble UI elements
        buttonPanel.Children.Add(yesButton);
        buttonPanel.Children.Add(noButton);
        stackPanel.Children.Add(textBlock);
        stackPanel.Children.Add(buttonPanel);
        confirmDialog.Content = stackPanel;

        // Show the confirmation dialog
        await confirmDialog.ShowDialog(this);
    }




private void DeleteVehiculo()
{
    if (_vehiculo == null) return; // Safety check

    // Access the parent MainWindow
    if (Owner is MainWindow mainWindow)
    {
        // Remove the vehicle from the ObservableCollection
        var removed = mainWindow.Vehiculos.Remove(_vehiculo);

        if (removed)
        {
            
            // Update the DataGrid in the MainWindow
            mainWindow.UpdateDataGrid(mainWindow.Vehiculos, "DgridFlota");

            // Save changes to XML
            var xmlExporter = new XmlExporter();
            xmlExporter.ExportarFlotaXML(mainWindow.Vehiculos, Path.Combine(mainWindow.rutaXml, "flota.xml"));
            Console.WriteLine("Cambios guardados en el archivo XML.");
        }
        else
        {
            Console.WriteLine("Error: No se pudo eliminar el vehículo.");
        }
    }
    else
    {
        Console.WriteLine("Error: No se pudo acceder a la ventana principal.");
    }

    // Close the current window
    this.Close();
}





    // Evento para manejar el botón "Buscar Vehículo"
    private void BuscarVehiculo_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Buscar vehículo ejecutado");
    }

    // Evento para manejar el botón "Agregar Vehículo"
    private void AgregarVehiculo_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Agregar vehículo ejecutado");
    }

    // Evento para manejar el botón "Modificar Vehículo"
    private void ModificarVehiculo_Click(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Modificar vehículo ejecutado");
    }

    private void EliminarVehiculo_Click(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
    
    private void SetFieldsEnabled(bool isEnabled)
    {
        this.FindControl<TextBox>("MatriculaTextBox").IsEnabled = isEnabled;
        this.FindControl<TextBox>("MarcaTextBox").IsEnabled = isEnabled;
        this.FindControl<TextBox>("ModeloTextBox").IsEnabled = isEnabled;
        this.FindControl<TextBox>("ConsumoPorKmTextBox").IsEnabled = isEnabled;
        this.FindControl<DatePicker>("FechaAdquisicionPicker").IsEnabled = isEnabled;
        this.FindControl<DatePicker>("FechaFabricacionPicker").IsEnabled = isEnabled;

        // ComboBox TipoVehiculo: habilitado solo si no es Camion o Furgoneta
        var tipoComboBox = this.FindControl<ComboBox>("TipoVehiculoComboBox");
        tipoComboBox.IsEnabled = isEnabled && _vehiculo.Tipo != TipoVehiculo.Camion && _vehiculo.Tipo != TipoVehiculo.Furgoneta;

        // Set checkboxes
        this.FindControl<CheckBox>("chkWifi").IsEnabled = isEnabled;
        this.FindControl<CheckBox>("chkBluetooth").IsEnabled = isEnabled;
        this.FindControl<CheckBox>("chkAireAcondicionado").IsEnabled = isEnabled;
        this.FindControl<CheckBox>("chkLitera").IsEnabled = isEnabled;
        this.FindControl<CheckBox>("chkTV").IsEnabled = isEnabled;
    }
}
