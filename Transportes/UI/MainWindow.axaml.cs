using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Transportes.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Interactivity;


namespace Transportes
{
    public partial class MainWindow : Window
    {
        // Colecciones de datos
        public ObservableCollection<Transporte> Transportes { get; set; }
        public ObservableCollection<Cliente> Clientes { get; set; }
        public ObservableCollection<Vehiculo> Vehiculos { get; set; }
        
        private readonly BusquedaService busqueda;

        string rutaXml = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

        // Fuente de datos dinámica
        public object ItemsSource { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            var opcionesCmbn=this.FindControl<ComboBox>("OpcionesComboBox1");
            var busquedaComb=this.FindControl<ComboBox>("BusquedaComboBox");
            XmlImporter importer = new XmlImporter();
            Clientes = new ObservableCollection<Cliente>(
                importer.CargarClientesXML(Path.Combine(rutaXml, "clientes.xml")));
            Vehiculos = new ObservableCollection<Vehiculo>(importer.CargarFlotaXML(Path.Combine(rutaXml, "flota.xml")));
            Transportes =
                new ObservableCollection<Transporte>(
                    importer.CargarTransportesXML(Path.Combine(rutaXml, "transportes.xml")));
            var tabC = this.FindControl<TabControl>("tabControlMenu");
            tabC.SelectionChanged += (s, e) => TabControl_SelectionChanged(s, e);
            UpdateDataGrid(Transportes, "DgridTransporte");
            if (opcionesCmbn != null) opcionesCmbn.SelectionChanged += (s, e) => SelectionChanged(s, e);
            if (busquedaComb != null) busquedaComb.SelectionChanged += (s, e) => BusquedaComboBox_SelectionChanged(s, e);
            busqueda = new BusquedaService(Transportes, Vehiculos, Clientes);
        }
        

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl)
            {
                var selectedTab = tabControl.SelectedItem as TabItem;

                if (selectedTab != null)
                {
                    string header = selectedTab.Header.ToString();

                    switch (header)
                    {
                        case "Transportes":
                            UpdateDataGrid(Transportes, "DgridTransporte");
                            break;

                        case "Clientes":
                            UpdateDataGrid(Clientes, "DgridClientes");
                            break;

                        case "Flota":
                            UpdateDataGrid(Vehiculos, "DgridFlota");
                            break;

                        default:
                            break;
                    }
                }
            }
        }


        private void UpdateDataGrid<T>(ObservableCollection<T> list, string control)
        {
            var dgrid = this.FindControl<DataGrid>(control);
            if (dgrid != null) dgrid.ItemsSource = list;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async Task<string?> cargarArchivo(object? sender, RoutedEventArgs e)
        {
            // Crear el cuadro de diálogo de apertura de archivo
            var openFileDialog = new OpenFileDialog();

            // Agregar filtros de archivo (opcional)
            openFileDialog.Filters.Add(new FileDialogFilter() { Name = "Archivos XML", Extensions = { "xml" } });

            // Abrir el cuadro de diálogo y esperar la selección
            var result = await openFileDialog.ShowAsync(this);

            // Verificar si se seleccionó algún archivo
            if (result != null && result.Length > 0)
            {
                string archivoSeleccionado = result[0]; // Tomar el primer archivo seleccionado
                return archivoSeleccionado;
            }

            return null;
        }


        private void ExportarClientes_Click(object sender, RoutedEventArgs e)
        {
            XmlExporter xmlExporter = new XmlExporter();
            xmlExporter.ExportarClientesXML(Clientes, Path.Combine(rutaXml, "clientes.xml"));
        }

        private void ExportarFlota_Click(object? sender, RoutedEventArgs e)
        {
            XmlExporter xmlExporter = new XmlExporter();
            xmlExporter.ExportarFlotaXML(Vehiculos, Path.Combine(rutaXml, "flota.xml"));
        }

        private void ExportarTransporte_Click(object? sender, RoutedEventArgs e)
        {
            XmlExporter xmlExporter = new XmlExporter();
            xmlExporter.ExportarTransportesXML(Transportes, Path.Combine(rutaXml, "transportes.xml"));
        }

        private async void ImportarCliente_Click(object? sender, RoutedEventArgs e)
        {
            string? archivo = await cargarArchivo(sender, e);

            if (archivo != null)
            {
                XmlImporter importer = new XmlImporter();
                var nuevosClientes = importer.CargarClientesXML(archivo);
                // Verificar si ya existe un cliente en la colección antes de agregarlo
                foreach (var cliente in nuevosClientes)
                {
                    // Verificar si el cliente ya existe en la colección 'Clientes'
                    if (!Clientes.Any(c => c.Nif == cliente.Nif)) // NIF es unico para cliente.
                    {
                        Clientes.Add(cliente); // Agregar el cliente si no existe
                    }
                }
            }
            else
            {
                MostrarErrorSeleccionArchivo();
            }
        }

        private async void ImportarFlota_Click(object? sender, RoutedEventArgs e)
        {
            string? archivo = await cargarArchivo(sender, e);

            if (archivo != null)
            {
                XmlImporter importer = new XmlImporter();
                var nuevasFlotas = importer.CargarFlotaXML(archivo);

                foreach (var flota in nuevasFlotas)
                {
                    if (!Vehiculos.Any(f => f.Matricula == flota.Matricula))
                    {
                        Vehiculos.Add(flota);
                    }
                }
            }
            else
            {
                MostrarErrorSeleccionArchivo(); // Llamar al método para mostrar el error
            }
        }

        private async void ImportarTransporte_Click(object? sender, RoutedEventArgs e)
        {
            string? archivo = await cargarArchivo(sender, e);

            if (archivo != null)
            {
                XmlImporter importer = new XmlImporter();
                var nuevosTransportes = importer.CargarTransportesXML(archivo);

                foreach (var transporte in nuevosTransportes)
                {
                    if (!Transportes.Any(t => t.Id == transporte.Id))
                    {
                        Transportes.Add(transporte);
                    }
                }
            }
            else
            {
                MostrarErrorSeleccionArchivo(); // Llamar al método para mostrar el error
            }
        }

        private async void RestaurarCliente_Click(object? sender, RoutedEventArgs e)
        {
            string? archivo = await cargarArchivo(sender, e);

            if (archivo != null)
            {
                XmlImporter importer = new XmlImporter();
                Clientes = new ObservableCollection<Cliente>(
                    importer.CargarClientesXML(archivo));
            }
            else
            {
                MostrarErrorSeleccionArchivo();
            }
        }

        private async void RestaurarFlota_Click(object? sender, RoutedEventArgs e)
        {
            string? archivo = await cargarArchivo(sender, e);

            if (archivo != null)
            {
                XmlImporter importer = new XmlImporter();
                Vehiculos = new ObservableCollection<Vehiculo>(
                    importer.CargarFlotaXML(archivo));
            }
            else
            {
                MostrarErrorSeleccionArchivo();
            }
        }

        private async void RestaurarTransporte_Click(object? sender, RoutedEventArgs e)
        {
            string? archivo = await cargarArchivo(sender, e);

            if (archivo != null)
            {
                XmlImporter importer = new XmlImporter();
                Transportes = new ObservableCollection<Transporte>(
                    importer.CargarTransportesXML(archivo));
            }
            else
            {
                MostrarErrorSeleccionArchivo();
            }
        }


        private void MostrarErrorSeleccionArchivo()
        {
            var messageWindow = new Window
            {
                Title = "Archivo Seleccionado",
                Width = 300,
                Height = 150,
                Content = new TextBlock
                {
                    Text = "Error, ningun archivo seleccionado",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                }
            };

            messageWindow.ShowDialog(this); // Mostrar la ventana
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verificar si el ComboBox y el elemento seleccionado son válidos
            var opcionesCmbn=this.FindControl<ComboBox>("OpcionesComboBox1");
            if (opcionesCmbn.SelectedItem is ComboBoxItem selectedItem)
            {
                // Obtener el texto de la opción seleccionada
                string seleccion = selectedItem.Content.ToString();

                // Tomar acciones según la selección
                switch (seleccion)
                {
                    case "Transportes pendientes":
                        var stackPanel = this.FindControl<StackPanel>("transportePendiente");
                        if (stackPanel != null) stackPanel.IsVisible = true;
                        
                        Console.WriteLine("Opción seleccionada: Transportes pendientes");
                        break;

                    case "Disponibilidad":
                        Console.WriteLine("Opción seleccionada: Disponibilidad");
                        break;

                    case "Reservas pasadas o pendientes":
                        Console.WriteLine("Opción seleccionada: Reservas pasadas o pendientes");
                        break;

                    case "Reservas por camión":
                        Console.WriteLine("Opción seleccionada: Reservas por camión");
                        break;

                    case "Reservas pendientes":
                        Console.WriteLine("Opción seleccionada: Reservas pendientes");
                        break;

                    case "Ocupación":
                        Console.WriteLine("Opción seleccionada: Ocupación");
                        break;

                    default:
                        Console.WriteLine("Opción no reconocida.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("No se seleccionó una opción válida.");
            }
            
            
        }

        
        private void BusquedaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var opcionesCmbn=this.FindControl<ComboBox>("BusquedaComboBox");
            var opcionesCmbn2=this.FindControl<ComboBox>("TipoCamionComboBox");
            var TipoCamionLabel=this.FindControl<Label>("TipoCamionLabel");
            
            if (opcionesCmbn?.SelectedItem is ComboBoxItem selectedItem)
            {
                string seleccion = selectedItem.Content?.ToString();
                if (string.IsNullOrEmpty(seleccion))
                {
                    Console.WriteLine("No se seleccionó un valor válido.");
                }
                else
                {
                    bool esBusquedaPorCamion = seleccion == "Vehiculo"; // esBusquedaPorCamion será true solo si "Camión" está seleccionado
                    opcionesCmbn2.IsVisible = esBusquedaPorCamion;
                    TipoCamionLabel.IsVisible = esBusquedaPorCamion;
                    Console.WriteLine($"Selección válida: {seleccion}");
                    // Lógica adicional basada en 'seleccion'.
                }
            }
            else
            {
                Console.WriteLine("No se ha seleccionado ningún elemento en el ComboBox.");
            }
            // Determina si el usuario seleccionó "Camión" para mostrar el ComboBox de TipoCamion
            
        }

        private void BuscarButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Determina si la búsqueda es por flota o por vehiculo
            
            var tipoSeleccionado=this.FindControl<ComboBox>("TipoCamionComboBox");
            var listBox2=this.FindControl<ListBox>("ResultadosListBox");
           
            TipoVehiculo? tipoVehiculo = tipoSeleccionado.SelectedItem switch
            {
                "Furgoneta" => TipoVehiculo.Furgoneta,
                "Camión" => TipoVehiculo.Camion,
                "Camión Articulado" => TipoVehiculo.CamionArticulado,
                _ => null // "Todos" o ninguna selección aplica null para obtener todos los tipos
            };

            // Busca camiones que no tengan transportes pendientes (considerados como "libres")
            var resultados = busqueda.BuscarDisponibilidad(tipoVehiculo);

            // Mostrar los resultados en el ListBox
            UpdateDataGrid(resultados, "DgridTransporte");
        }

        
    }
}