using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Transportes.Core;


namespace Transportes.UI;

public partial class DataViewTransporte : Window
{
    private Transporte _transporte;
    private bool _isEditing;
    private readonly Action<Transporte?> _onTransportModified;

    public Transporte getTransporte()
    {
        return _transporte;
    }
    public DataViewTransporte(Transporte transporte,ObservableCollection<Transporte> transportes, Action<Transporte?> onTransportModified)
    {
        InitializeComponent();
        _onTransportModified = onTransportModified;
        _transporte = new Transporte()
        {
            Id = transporte.Id,
            FechaContratacion = transporte.FechaContratacion,
            Cliente = transporte.Cliente,
            FechaEntrega = transporte.FechaEntrega,
            FechaSalida = transporte.FechaSalida,
            IvaAplicado = transporte.IvaAplicado,
            KilometrosRecorridos = transporte.KilometrosRecorridos,
            ImportePorDia = transporte.ImportePorDia,
            ImportePorKilometro = transporte.ImportePorKilometro,
            Vehiculo = transporte.Vehiculo,
            Tipo = transporte.Tipo
        };
        DataContext = _transporte;
        _isEditing = false;
        var btGuardar = this.FindControl<Button>("ButtonEditar");
        btGuardar.Click += (sender, args) => OnEditarClick(transporte,transportes);
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

    // Evento para manejar el botón "Generar Factura"
    private void OnGenerarFacturaClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Generar Factura ejecutado");
        if (_transporte != null)
        {
            var generateInvoiceWindow = new GenerateInvoiceWindow(_transporte);
            generateInvoiceWindow.ShowDialog(this);
        }
        else
        {
            Console.WriteLine("No hay un transporte seleccionado para generar la factura.", "Error");
        }
       
    }

    // Evento para manejar el botón "Editar"
    private void OnEditarClick(Transporte t,ObservableCollection<Transporte> transportes)
    {
        Console.WriteLine("Editar ejecutado");
        var bEditar = this.FindControl<Button>("ButtonEditar");
        if (_isEditing)
        {
            // Guardar los cambios
            Console.WriteLine("Guardando cambios...");
            bEditar.Content = "Editar";
            for (int i = 0; i < transportes.Count; i++)
            {
                if (t.Id == transportes[i].Id)
                {
                    transportes[i] = _transporte;
                }
            }
           
            this.Close();
            
        }
        else
        {
            Console.WriteLine("Modo edición activado.");
            bEditar.Content = "Guardar";
        }
        
        _isEditing = !_isEditing;
        
        this.FindControl<DatePicker>("FechaContratacionTextBox").IsEnabled = _isEditing;
        this.FindControl<TextBox>("KilometrosRecorridosTextBox").IsEnabled = _isEditing;
        this.FindControl<DatePicker>("FechaSalidaTextBox").IsEnabled = _isEditing;
        this.FindControl<DatePicker>("FechaEntregaTextBox").IsEnabled = _isEditing;
        this.FindControl<TextBox>("ImportePorDiaTextBox").IsEnabled = _isEditing;
        this.FindControl<TextBox>("ImportePorKilometroTextBox").IsEnabled = _isEditing;
        this.FindControl<TextBox>("IvaAplicadoTextBox").IsEnabled = _isEditing;
        
    }

    // Evento para manejar el botón "Eliminar"
    private async void OnEliminarClick(object? sender, RoutedEventArgs e)
    {
        if (_transporte == null) return;

        var deleteWindow = new DeleteTransportWindow(_transporte, transporteEliminado =>
        {
            if (transporteEliminado != null)
            {
                _onTransportModified(null);
                Close(); // Cerrar la ventana después de la eliminación
            }
        });

        await deleteWindow.ShowDialog(this);
    }
}
