using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Transportes;
using System.Linq;
using Transportes.Core;
using System.IO;
using Avalonia.Interactivity;
using Transportes.UI;


namespace Transportes
{
    public partial class MainWindow : Window
    {
        // Colecciones de datos
        public ObservableCollection<Transporte> Transportes { get; set; }
        public ObservableCollection<Cliente> Clientes { get; set; }
        public ObservableCollection<Vehiculo> Vehiculos { get; set; }

        string rutaXml = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

        // Fuente de datos dinámica
        public object ItemsSource { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            XmlImporter importer = new XmlImporter();
            // Inicializar datos
            Clientes = new ObservableCollection<Cliente>(
                importer.CargarClientesXML(Path.Combine(rutaXml, "clientes.xml")));

            Vehiculos = new ObservableCollection<Vehiculo>(importer.CargarFlotaXML(Path.Combine(rutaXml, "flota.xml")));

            Transportes =
                new ObservableCollection<Transporte>(
                    importer.CargarTransportesXML(Path.Combine(rutaXml, "transportes.xml")));
            
            // Mostrar transportes al iniciar
            //ItemsSource = Transportes;

            // Vincular DataGrid
            var dgrid = this.FindControl<DataGrid>("Dgrid");
            if (dgrid != null) dgrid.ItemsSource = Transportes;
            //Vincular DataGrid Clientes
            var dgridcliente = this.FindControl<DataGrid>("DgridCliente");
            if (dgridcliente != null) dgridcliente.ItemsSource = Clientes;

            // Configurar eventos de botones
            ConfigureButtonEvents();
        }

        private void ConfigureButtonEvents()
        {
            var anadirButton = this.FindControl<Button>("AnadirButton");
            if (anadirButton != null)
            {
                anadirButton.Click += AnadirButton_Click;
            }

        }

        private void UpdateDataGrid()
        {
            var dgrid = this.FindControl<DataGrid>("Dgrid");
            if (dgrid != null) dgrid.ItemsSource = Transportes;
        }

        private void InitializeComponent()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AvaloniaXamlLoader.Load(this);
        }

        private async void AnadirButton_Click(object? sender, RoutedEventArgs e)
        {
            var tabcontrol = this.FindControl<TabControl>("TabControl");
            int activeTab = tabcontrol.SelectedIndex;
            switch (activeTab)
            {
                case 0: // Transportes
                    var transportesWindow = new AddTransportWindow(Transportes.ToList(), Clientes.ToList(), Vehiculos.ToList());
                    await transportesWindow.ShowDialog(this);
                    if (transportesWindow.NuevoTransporte != null)
                    {
                        Transportes.Add(transportesWindow.NuevoTransporte);
                    }
                    break;

                case 1: // Clientes
                    var clientesaddwindow = new AddClienteWindow();
                    await clientesaddwindow.ShowDialog(this);
                    if (!clientesaddwindow.IsCancelled)
                    {
                        Clientes.Add(new Cliente(){Nif=clientesaddwindow.Nif, DireccionPostal = clientesaddwindow.DireccionPostal, Nombre = clientesaddwindow.Nombre, Telefono = clientesaddwindow.Telefono, Email = clientesaddwindow.Email});
                    }
                    break;

                case 2: // Flota
                    //var flotaWindow = new FlotaWindow(); // FlotaWindow.axaml.cs
                    //flotaWindow.Show();
                    break;
            }
        }
    }

}