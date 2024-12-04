using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Transportes;
using Transportes.Core;
using System.IO;


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
            Clientes  = new ObservableCollection<Cliente>(importer.CargarClientesXML(Path.Combine(rutaXml, "clientes.xml")));

            Vehiculos  = new ObservableCollection<Vehiculo>(importer.CargarFlotaXML(Path.Combine(rutaXml, "flota.xml")));

            Transportes = new ObservableCollection<Transporte>(importer.CargarTransportesXML(Path.Combine(rutaXml, "transportes.xml")));
               
            

            // Mostrar transportes al iniciar
            //ItemsSource = Transportes;

            // Vincular DataGrid
            var dgrid = this.FindControl<DataGrid>("Dgrid");
            if (dgrid != null) dgrid.ItemsSource = Transportes;

            // Configurar eventos de botones
            ConfigureButtonEvents();
        }

        private void ConfigureButtonEvents()
        {
            var clientesButton = this.FindControl<Button>("ClientesButton");
            var flotaButton = this.FindControl<Button>("FlotaButton");
            var datosButton = this.FindControl<Button>("DatosButton");
            var transportesButton = this.FindControl<Button>("TransportesButton");
            var graficosButton = this.FindControl<Button>("GraficosButton");

            if (clientesButton != null)
                clientesButton.Click += (sender, e) => OnClientesClick();

            if (flotaButton != null)
                flotaButton.Click += (sender, e) => OnFlotaClick();

            if (datosButton != null)
                datosButton.Click += (sender, e) => OnDatosClick();

            if (transportesButton != null)
                transportesButton.Click += (sender, e) => OnTransportesClick();

            if (graficosButton != null)
                graficosButton.Click += (sender, e) => OnGraficosClick();
        }

        private void UpdateDataGrid()
        {
            var dgrid = this.FindControl<DataGrid>("Dgrid");
            if (dgrid != null) dgrid.ItemsSource = Transportes;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private void OnClientesClick()
        {
            // Lógica para el botón Clientes
        }

        private void OnFlotaClick()
        {
            // Lógica para el botón Flota
        }

        private void OnDatosClick()
        {
            // Lógica para el botón Datos
        }

        private void OnTransportesClick()
        {
            // Lógica para el botón Transportes
        }

        private void OnGraficosClick()
        {
            // Lógica para el botón Gráficos
        }
    }
}
