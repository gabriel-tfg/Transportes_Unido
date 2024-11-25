using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Transportes.Core;

namespace Transportes.UI
{
    public partial class AddTransportWindow : Window
    {
        private List<Transporte> _transportes;
        private List<Cliente> _clientes;
        private List<Vehiculo> _vehiculos;
        public Transporte? NuevoTransporte { get; private set; }

        public AddTransportWindow(List<Transporte> transportes, List<Cliente> clientes, List<Vehiculo> vehiculos)
        {
            InitializeComponent();
            this._transportes = transportes;
            this._clientes = clientes;
            this._vehiculos = vehiculos;

            tipoTransporteComboBox.ItemsSource = Enum.GetValues(typeof(TipoTransporte));
        }

        private async void AceptarButton_Click(object? sender, RoutedEventArgs e)
        {
            // Validar que el campo de NIF no esté vacío
            if (string.IsNullOrWhiteSpace(nifTextBox.Text))
            {
                await MostrarError("Por favor, introduce un NIF válido.");
                return;
            }

            // Buscar cliente por NIF
            var cliente = _clientes.FirstOrDefault(c => c.Nif == nifTextBox.Text);
            if (cliente == null)
            {
                await MostrarError($"No se encontró ningún cliente con el NIF: {nifTextBox.Text}");
                return;
            }

            // Tipo de transporte
            var tipoTransporte = (TipoTransporte?)tipoTransporteComboBox.SelectedItem ?? TipoTransporte.Mudanza;
            TipoVehiculo tipoVehiculoRequerido = tipoTransporte switch
            {
                TipoTransporte.Mudanza => TipoVehiculo.Furgoneta,
                TipoTransporte.Mercancias or TipoTransporte.Vehiculos => TipoVehiculo.Camion,
                _ => throw new InvalidOperationException("Tipo de transporte no soportado.")
            };

            // Verificar disponibilidad de vehículos
            Vehiculo? vehiculoDisponible = _vehiculos.FirstOrDefault(v => v.Disponible && v.Tipo == tipoVehiculoRequerido);
            if (vehiculoDisponible == null)
            {
                await MostrarError("No hay vehículos disponibles para este tipo de transporte.");
                return;
            }

            // Validar valores numéricos
            if (!double.TryParse(kilometrosRecorridosTextBox.Text, out double kilometrosRecorridos) ||
                !double.TryParse(importePorDiaTextBox.Text, out double importePorDia) ||
                !double.TryParse(importePorKilometroTextBox.Text, out double importePorKilometro) ||
                !double.TryParse(ivaAplicadoTextBox.Text, out double ivaAplicado))
            {
                await MostrarError("Introduce valores numéricos válidos en los campos de importe e IVA.");
                return;
            }

            // Fechas
            var fechaContratacion = fechaContratacionDatePicker.SelectedDate?.DateTime ?? DateTime.Now;
            var fechaSalida = fechaSalidaDatePicker.SelectedDate?.DateTime ?? DateTime.Now;
            var fechaEntrega = fechaEntregaDatePicker.SelectedDate?.DateTime ?? DateTime.Now;

            // Marcar el vehículo como no disponible
            vehiculoDisponible.Disponible = false;

            // Crear el nuevo transporte
            NuevoTransporte = new Transporte(
                tipoTransporte,
                cliente,  
                vehiculoDisponible,
                fechaContratacion,
                kilometrosRecorridos,
                fechaSalida,
                fechaEntrega,
                importePorDia,
                importePorKilometro,
                ivaAplicado
            );

            // Agregar el nuevo transporte a la lista
            _transportes.Add(NuevoTransporte);

            // Confirmar que el cliente y su NIF están asociados
            Console.WriteLine($"Nuevo transporte agregado con ID: {NuevoTransporte.Id} y NIF del cliente: {cliente.Nif}");

            // Cerrar la ventana
            Close();
        }
        private async Task MostrarError(string mensaje)
        {
            var errorWindow = new Window
            {
                Title = "Error",
                Content = new TextBlock { Text = mensaje, Margin = new Thickness(20) },
                Width = 300,
                Height = 150
            };
            await errorWindow.ShowDialog(this);
        }

        private void CancelarButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
