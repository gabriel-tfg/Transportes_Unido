using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
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

    public Transporte getTransporte()
    {
        return _transporte;
    }
    public DataViewTransporte(Transporte transporte)
    {
        InitializeComponent();
        _transporte = transporte;
        DataContext = _transporte;
        _isEditing = false;
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
    private void OnEditarClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Editar ejecutado");
        var bEditar = this.FindControl<Button>("ButtonEditar");
        if (_isEditing)
        {
            // Guardar los cambios
            Console.WriteLine("Guardando cambios...");
            bEditar.Content = "Editar";
            
            this.Close();
            
        }
        else
        {
            Console.WriteLine("Modo edición activado.");
            bEditar.Content = "Guardar";
        }
        
        Console.WriteLine("editing = " + _isEditing);
        _isEditing = !_isEditing;
        Console.WriteLine("editing2 = " + _isEditing);
        
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
        // Crear una ventana de confirmación personalizada
        var dialog = new Window
        {
            Title = "Confirmación",
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(10),
                Children =
                {
                    new TextBlock
                    {
                        Text = "¿Está seguro de que desea eliminar?",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 20)
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Spacing = 10,
                        Children =
                        {
                            new Button
                            {
                                Content = "Sí",
                                Width = 75,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                            },
                            new Button
                            {
                                Content = "No",
                                Width = 75,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                            }
                        }
                    }
                }
            }
        };

        // Mostrar diálogo y esperar resultado
        dialog.ShowDialog(this);
    }
}
