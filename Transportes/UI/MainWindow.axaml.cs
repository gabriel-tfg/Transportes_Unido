using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Transportes;
using Transportes.Core;
using Transportes.UI;

namespace Transportes
{
    public partial class MainWindow : Window
    {
        // Colecciones de datos
        public ObservableCollection<Transporte> Transportes { get; set; }
        public List<Cliente> Clientes { get; set; }
        public List<Vehiculo> Vehiculos { get; set; }

        // Fuente de datos dinámica
        public object ItemsSource { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Inicializar datos
            Clientes = new List<Cliente>
            {
                new Cliente("1A", "Juan Pérez", "123456789", "juan@example.com", "Calle Falsa 123"),
                new Cliente("2B", "Ana García", "987654321", "ana@example.com", "Avenida Siempreviva 742"),
                new Cliente("3C", "Carlos López", "654321987", "carlos@example.com", "Calle del Sol 45"),
                new Cliente("4D", "María Fernández", "321987654", "maria@example.com", "Plaza Mayor 89"),
                new Cliente("5E", "Luis Martínez", "789123456", "luis@example.com", "Calle Luna 67"),
                new Cliente("6F", "Laura Sánchez", "456789123", "laura@example.com", "Avenida del Parque 21"),
                new Cliente("7G", "Pedro Gómez", "123789456", "pedro@example.com", "Camino Real 33"),
                new Cliente("8H", "Elena Rodríguez", "987321654", "elena@example.com", "Paseo de las Flores 15")
            };

            Vehiculos = new List<Vehiculo>
            {
                new Vehiculo("1234ABC", TipoVehiculo.Furgoneta, "Ford", "Transit", 8.5, DateTime.Now.AddYears(-2), DateTime.Now.AddYears(-3)),
                new Vehiculo("5678DEF", TipoVehiculo.Camion, "Mercedes", "Actros", 25.0, DateTime.Now.AddYears(-5), DateTime.Now.AddYears(-6)),
                new Vehiculo("9101GHI", TipoVehiculo.Camion, "Volvo", "FH16", 30.0, DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-5)),
                new Vehiculo("1123JKL", TipoVehiculo.Furgoneta, "Volkswagen", "Crafter", 10.0, DateTime.Now.AddYears(-1), DateTime.Now.AddYears(-2)),
                new Vehiculo("4567MNO", TipoVehiculo.Camion, "Scania", "R500", 28.0, DateTime.Now.AddYears(-6), DateTime.Now.AddYears(-7)),
                new Vehiculo("8910PQR", TipoVehiculo.Furgoneta, "Peugeot", "Boxer", 9.0, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-4)),
                new Vehiculo("3344STU", TipoVehiculo.Camion, "MAN", "TGX", 27.5, DateTime.Now.AddYears(-7), DateTime.Now.AddYears(-8)),
                new Vehiculo("6677VWX", TipoVehiculo.Furgoneta, "Renault", "Master", 8.0, DateTime.Now.AddYears(-2), DateTime.Now.AddYears(-3))
            };

            Transportes = new ObservableCollection<Transporte>
            {
                new Transporte(
                    TipoTransporte.Mudanza,
                    Clientes[0],
                    Vehiculos[0],
                    DateTime.Now.AddDays(-30),
                    150.0,
                    DateTime.Now.AddDays(-28),
                    DateTime.Now.AddDays(-25),
                    50,
                    0.20,
                    21),
                new Transporte(
                    TipoTransporte.Mercancias,
                    Clientes[1],
                    Vehiculos[1],
                    DateTime.Now.AddDays(-15),
                    300.0,
                    DateTime.Now.AddDays(-14),
                    DateTime.Now.AddDays(-13),
                    100,
                    0.50,
                    21),
                new Transporte(
                    TipoTransporte.Vehiculos,
                    Clientes[2],
                    Vehiculos[2],
                    DateTime.Now.AddDays(-10),
                    500.0,
                    DateTime.Now.AddDays(-9),
                    DateTime.Now.AddDays(-8),
                    120,
                    0.75,
                    21)
            };

            // Mostrar transportes al iniciar
            //ItemsSource = Transportes;

            // Vincular DataGrid
            var dgrid = this.FindControl<DataGrid>("Dgrid");
            dgrid.ItemsSource = Transportes;
            
            dgrid.SelectionChanged += Dgrid_SelectionChanged;

            // Configurar eventos de botones
            ConfigureButtonEvents();
        }

        private void ConfigureButtonEvents()
        {
            var addTransportButton = this.FindControl<Button>("AddTransportButton");
            var consultTransportButton = this.FindControl<Button>("ConsultTransportButton");

            if (addTransportButton != null)
                addTransportButton.Click += AddTransport_Click;

            if (consultTransportButton != null)
                consultTransportButton.Click += ConsultTransport_Click;
        }

        private async void AddTransport_Click(object? sender, RoutedEventArgs e)
        {
            var addTransportWindow = new AddTransportWindow(Transportes.ToList(), Clientes, Vehiculos);
            await addTransportWindow.ShowDialog(this);

            if (addTransportWindow.NuevoTransporte != null)
            {
                Transportes.Add(addTransportWindow.NuevoTransporte);
            }
        }

        private async void ConsultTransport_Click(object? sender, RoutedEventArgs e)
        {
            var consultTransportWindow = new ConsultTransportWindow(Transportes.ToList());
            await consultTransportWindow.ShowDialog(this);
        }

        private async void Dgrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Transporte selectedTransporte)
            {
                // Inicializamos la ventana antes del ShowDialog
                var transportDetailsWindow = new TransportDetailsWindow(
                    selectedTransporte,
                    Transportes,
                    modifiedTransporte =>
                    {
                        if (modifiedTransporte == null)
                        {
                            Transportes.Remove(selectedTransporte); // Eliminar transporte
                        }
                        else
                        {
                            int index = Transportes.IndexOf(selectedTransporte);
                            if (index >= 0)
                                Transportes[index] = modifiedTransporte; // Actualizar transporte
                        }
                    });

                // Mostrar ventana de detalles como diálogo
                await transportDetailsWindow.ShowDialog(this);

                // Limpiar selección
                dataGrid.SelectedItem = null;
            }
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}