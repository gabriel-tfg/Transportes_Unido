using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Transportes.Core;
using Transportes.UI;

namespace Transportes;

public partial class MainWindow : Window
{
    private static readonly string[] _ejeMes =
        { "En", "Fb", "Ma", "Ab", "My", "Jn", "Jl", "Ag", "Sp", "Oc", "Nv", "Dc" };

    private static readonly string[] _ejeAnho = { "2020", "2021", "2022", "2023", "2024" };
    private static readonly string _labelMes = "Mes";
    private static readonly string _labelAnho = "Año";

    private readonly BusquedaService busqueda;

    public readonly string rutaXml =
        Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

    private Chart.ChartType _chartType;
    private int[] _dataAnho;
    private int[] _dataMes;
    private string _labelY = "Número transportes";

    private string lastTabItem;


    public MainWindow()
    {
        this.Icon = new WindowIcon("Resources/logo.ico");
        InitializeComponent();
        ConfigureButtonEvents();

        var opcionesCmbn = this.FindControl<ComboBox>("OpcionesComboBox1");
        var busquedaComb = this.FindControl<ComboBox>("BusquedaComboBox");
        var busquedaCombRC = this.FindControl<ComboBox>("BusquedaComboBoxRC");
        var cbFlota = this.FindControl<ComboBox>("OpcionesComboBoxFlota");
        var rb = this.FindControl<RadioButton>("rbAnho");
        var importer = new XmlImporter();

        rb.IsCheckedChanged += (sender, args) => RadioButtonChanged();
        cbFlota.SelectionChanged += (s, e) => SelectionChangedFlota(s, e);
        Clientes = new ObservableCollection<Cliente>(
            importer.CargarClientesXML(Path.Combine(rutaXml, "clientes.xml")));
        Vehiculos = new ObservableCollection<Vehiculo>(importer.CargarFlotaXML(Path.Combine(rutaXml, "flota.xml")));
        Transportes =
            new ObservableCollection<Transporte>(
                importer.CargarTransportesXML(Path.Combine(rutaXml, "transportes.xml")));

        Chart = this.GetControl<Chart>("ChartW");
        StartChart();

        var tabC = this.FindControl<TabControl>("tabControlMenu");
        if (tabC.SelectedItem is TabItem item) lastTabItem = item.Header.ToString();

        tabC.SelectionChanged += (s, e) => TabControl_SelectionChanged(s, e);
        UpdateDataGrid(Transportes, "DgridTransporte");
        if (opcionesCmbn != null) opcionesCmbn.SelectionChanged += (s, e) => SelectionChanged(s, e);
        if (busquedaComb != null)
            busquedaComb.SelectionChanged += (s, e) => BusquedaComboBox_SelectionChanged(s, e);
        if (busquedaCombRC != null)
            busquedaCombRC.SelectionChanged += (s, e) => BusquedaComboBox_SelectionChangedRC(s, e);
        busqueda = new BusquedaService(Transportes, Vehiculos, Clientes);
        var button = this.GetControl<Button>("ButtonOp");
        var rbMes = this.GetControl<RadioButton>("MesOp");
        var rbAnho = this.GetControl<RadioButton>("AnhoOp");
        var comboBox = this.GetControl<ComboBox>("OpGrafica");

        comboBox.SelectionChanged += (_, _) => OnComboChanged();
        button.Click += (_, _) => OnButtonClicked();
        rbMes.IsCheckedChanged += (_, _) => OnChangeFilter();
        rbAnho.IsCheckedChanged += (_, _) => OnChangeFilter();
        //boton añadir cliente
        var anadirCliente = this.FindControl<Button>("AnadirCliente");
        if (anadirCliente != null)
        {
            anadirCliente.Click += AnadirCliente_Click;
        }

        //cliente grid
        var dgridcliente = this.FindControl<DataGrid>("DgridClientes");
        if (dgridcliente != null)
        {
            dgridHandler = (sender, args) => DgridCliente_SelectionChanged(sender);
            dgridcliente.SelectionChanged += dgridHandler;
        }

        // transporte click fila

        var dgrid = this.FindControl<DataGrid>("DgridTransporte");
        if (dgrid != null)
        {
            //dgrid.SelectionChanged += Dgrid_SelectionChanged;
            dgridHandler = (sender, args) => Dgrid_SelectionChanged(sender, args);
            dgrid.ItemsSource = Transportes;
            dgrid.SelectionChanged += dgridHandler;
        }

        this.Closing += OnMainWindowClosing;
        // FLOTA click fila 
        var flota = this.FindControl<DataGrid>("DgridFlota");

        if (flota != null)
        {
            flotaHandler = (sender, args) => Dgrid_SelectionChanged2(sender);
            flota.ItemsSource = Vehiculos;
            flota.SelectionChanged += flotaHandler;
        }
    }

    private IEnumerable<Vehiculo> ObtenerVehiculos()
    {
        // Check if the Vehiculos collection is initialized
        return Vehiculos ?? Enumerable.Empty<Vehiculo>();
    }

    private EventHandler<SelectionChangedEventArgs> flotaHandler;
    private EventHandler<SelectionChangedEventArgs> dgridHandler;
    private EventHandler<SelectionChangedEventArgs> dgridclienteHandler;

    private void OnMainWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        Console.WriteLine("Guardando y saliendo...");
        XmlExporter xmlExporter = new XmlExporter();
        xmlExporter.ExportarClientesXML(Clientes, Path.Combine(rutaXml, "clientes.xml"));
        xmlExporter.ExportarFlotaXML(Vehiculos, Path.Combine(rutaXml, "flota.xml"));
        xmlExporter.ExportarTransportesXML(Transportes, Path.Combine(rutaXml, "transportes.xml"));
    }

    // Colecciones de datos
    public ObservableCollection<Transporte> Transportes { get; set; }
    public ObservableCollection<Cliente> Clientes { get; set; }
    public ObservableCollection<Cliente> FiltradoClientes { get; } = new ObservableCollection<Cliente>();

    public ObservableCollection<Vehiculo> Vehiculos { get; set; }

    // Fuente de datos dinámica
    public object ItemsSource { get; set; }

    private Chart Chart { get; }

    private void OnComboChanged()
    {
        var comboBox = this.GetControl<ComboBox>("OpGrafica");
        var labelOp = this.GetControl<Label>("LabelOp");
        var textBox = this.GetControl<TextBox>("TextBox");
        var button = this.GetControl<Button>("ButtonOp");
        var spFilter = this.GetControl<StackPanel>("SPFilter");

        if (comboBox.SelectedIndex == 0)
        {
            int[] values = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var cont = 0;

            foreach (var t in Transportes)
                if (t.FechaContratacion.Year == 2024)
                    values[t.FechaContratacion.Month - 1]++;

            _dataMes = values;

            values = new[] { 0, 0, 0, 0, 0 };

            foreach (var t in Transportes)
                if (t.FechaContratacion.Year == 2024)
                    values[4]++;
                else if (t.FechaContratacion.Year == 2023)
                    values[3]++;
                else if (t.FechaContratacion.Year == 2022)
                    values[2]++;
                else if (t.FechaContratacion.Year == 2021)
                    values[1]++;
                else if (t.FechaContratacion.Year == 2020) values[0]++;

            _dataAnho = values;
            labelOp.IsVisible = false;
            textBox.IsVisible = false;
            button.IsVisible = false;
            spFilter.IsVisible = true;

            OnChangeFilter();
        }
        else if (comboBox.SelectedIndex == 1)
        {
            labelOp.Content = "Introduce el NIF del cliente";
            labelOp.IsVisible = true;
            textBox.IsVisible = true;
            button.IsVisible = true;
            spFilter.IsVisible = false;
        }
        else if (comboBox.SelectedIndex == 2)
        {
            labelOp.Content = "Introduce la matrícula del vehiculo";
            labelOp.IsVisible = true;
            textBox.IsVisible = true;
            button.IsVisible = true;
            spFilter.IsVisible = false;
        }
        else
        {
            labelOp.IsVisible = false;
            textBox.IsVisible = false;
            button.IsVisible = false;
            spFilter.IsVisible = false;
            DrawChart2();
        }
    }

    private void OnButtonClicked()
    {
        var comboBox = this.GetControl<ComboBox>("OpGrafica");
        var textBox = this.GetControl<TextBox>("TextBox");
        var spFilter = this.GetControl<StackPanel>("SPFilter");

        if (!string.IsNullOrEmpty(textBox.Text))
        {
            if (comboBox.SelectedIndex == 1)
            {
                DataClienteMes(textBox.Text.ToUpper());
                DataClienteAnho(textBox.Text.ToUpper());
                _labelY = "Número transportes";
                spFilter.IsVisible = true;
            }
            else if (comboBox.SelectedIndex == 2)
            {
                DataCamionMes(textBox.Text);
                DataCamionAnho(textBox.Text);
                _labelY = "Número transportes";
                spFilter.IsVisible = true;
            }

            OnChangeFilter();
        }
    }

    private void DataCamionMes(string matricula)
    {
        int[] result = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        foreach (var t in Transportes)
        {
            var aux = new string(t.Id.Trim().ToLower().ToCharArray(0, 7));
            if (aux.Equals(matricula.Trim().ToLower()) && t.FechaContratacion.Year == 2024)
            {
                Console.WriteLine("Entro con matricula = " + matricula + " y aux = " + aux);
                result[t.FechaContratacion.Month - 1]++;
            }
        }

        _dataMes = result;
    }

    private void DataCamionAnho(string matricula)
    {
        int[] result = { 0, 0, 0, 0, 0 };

        foreach (var t in Transportes)
        {
            var aux = new string(t.Id.Trim().ToLower().ToCharArray(0, 7));
            if (aux.Equals(matricula.Trim().ToLower()))
            {
                if (t.FechaContratacion.Year == 2024)
                    result[4]++;
                else if (t.FechaContratacion.Year == 2023)
                    result[3]++;
                else if (t.FechaContratacion.Year == 2022)
                    result[2]++;
                else if (t.FechaContratacion.Year == 2021)
                    result[1]++;
                else if (t.FechaContratacion.Year == 2020) result[0]++;
            }
        }

        _dataAnho = result;
    }

    private void DataClienteMes(string NIF)
    {
        int[] result = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        foreach (var t in Transportes)
            if (t.Cliente.Nif == NIF && t.FechaContratacion.Year == 2024)
                result[t.FechaContratacion.Month - 1]++;

        _dataMes = result;
    }

    private void DataClienteAnho(string NIF)
    {
        int[] result = { 0, 0, 0, 0, 0 };

        foreach (var t in Transportes)
            if (t.Cliente.Nif == NIF)
            {
                if (t.FechaContratacion.Year == 2024)
                    result[4]++;
                else if (t.FechaContratacion.Year == 2023)
                    result[3]++;
                else if (t.FechaContratacion.Year == 2022)
                    result[2]++;
                else if (t.FechaContratacion.Year == 2021)
                    result[1]++;
                else if (t.FechaContratacion.Year == 2020) result[0]++;
            }

        _dataAnho = result;
    }

    private void OnChangeFilter()
    {
        var rbMes = this.GetControl<RadioButton>("MesOp");
        var rbAnho = this.GetControl<RadioButton>("AnhoOp");

        if (rbMes.IsChecked == true)
            DrawChart(0);
        else if (rbAnho.IsChecked == true) DrawChart(1);
    }


    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is TabControl tabControl)
        {
            var selectedTab = tabControl.SelectedItem as TabItem;

            if (selectedTab != null)
            {
                var header = selectedTab.Header.ToString();

                if (header != lastTabItem)
                {
                    switch (header)
                    {
                        case "Transportes":

                            UpdateDataGrid(Transportes, "DgridTransporte");
                            break;

                        case "Clientes":
                            FiltrarClientes(string.Empty);
                            this.FindControl<TextBox>("SearchTextBox").Text = string.Empty;
                            UpdateDataGrid(FiltradoClientes, "DgridClientes");
                            break;

                        case "Flota":
                            UpdateDataGrid(Vehiculos, "DgridFlota");
                            break;
                    }

                    lastTabItem = header;
                }
            }
        }
    }


    public void UpdateDataGrid<T>(ObservableCollection<T> list, string control)
    {
        Console.WriteLine("UpdateDataGrid");
        var dgrid = this.FindControl<DataGrid>(control);
        if (dgrid != null)
        {
            dgrid.ItemsSource = null;
            dgrid.ItemsSource = list;
        }
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
        openFileDialog.Filters.Add(new FileDialogFilter { Name = "Archivos XML", Extensions = { "xml" } });

        // Abrir el cuadro de diálogo y esperar la selección
        var result = await openFileDialog.ShowAsync(this);

        // Verificar si se seleccionó algún archivo
        if (result != null && result.Length > 0)
        {
            var archivoSeleccionado = result[0]; // Tomar el primer archivo seleccionado
            return archivoSeleccionado;
        }

        return null;
    }


    private void ExportarClientes_Click(object sender, RoutedEventArgs e)
    {
        var xmlExporter = new XmlExporter();
        xmlExporter.ExportarClientesXML(Clientes, Path.Combine(rutaXml, "clientes.xml"));
    }

    private void ExportarFlota_Click(object? sender, RoutedEventArgs e)
    {
        var xmlExporter = new XmlExporter();
        xmlExporter.ExportarFlotaXML(Vehiculos, Path.Combine(rutaXml, "flota.xml"));
    }

    private void ExportarTransporte_Click(object? sender, RoutedEventArgs e)
    {
        var xmlExporter = new XmlExporter();
        xmlExporter.ExportarTransportesXML(Transportes, Path.Combine(rutaXml, "transportes.xml"));
    }

    private async void ImportarCliente_Click(object? sender, RoutedEventArgs e)
    {
        var archivo = await cargarArchivo(sender, e);

        if (archivo != null)
        {
            var importer = new XmlImporter();
            var nuevosClientes = importer.CargarClientesXML(archivo);
            // Verificar si ya existe un cliente en la colección antes de agregarlo
            foreach (var cliente in nuevosClientes)
                // Verificar si el cliente ya existe en la colección 'Clientes'
                if (!Clientes.Any(c => c.Nif == cliente.Nif)) // NIF es unico para cliente.
                    Clientes.Add(cliente); // Agregar el cliente si no existe
        }
        else
        {
            MostrarErrorSeleccionArchivo();
        }
    }

    private async void ImportarFlota_Click(object? sender, RoutedEventArgs e)
    {
        var archivo = await cargarArchivo(sender, e);

        if (archivo != null)
        {
            var importer = new XmlImporter();
            var nuevasFlotas = importer.CargarFlotaXML(archivo);

            foreach (var flota in nuevasFlotas)
                if (!Vehiculos.Any(f => f.Matricula == flota.Matricula))
                    Vehiculos.Add(flota);
        }
        else
        {
            MostrarErrorSeleccionArchivo(); // Llamar al método para mostrar el error
        }
    }

    private async void ImportarTransporte_Click(object? sender, RoutedEventArgs e)
    {
        var archivo = await cargarArchivo(sender, e);

        if (archivo != null)
        {
            var importer = new XmlImporter();
            var nuevosTransportes = importer.CargarTransportesXML(archivo);

            foreach (var transporte in nuevosTransportes)
                if (!Transportes.Any(t => t.Id == transporte.Id))
                    Transportes.Add(transporte);
        }
        else
        {
            MostrarErrorSeleccionArchivo(); // Llamar al método para mostrar el error
        }
    }

    private async void RestaurarCliente_Click(object? sender, RoutedEventArgs e)
    {
        var archivo = await cargarArchivo(sender, e);

        if (archivo != null)
        {
            var importer = new XmlImporter();
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
        var archivo = await cargarArchivo(sender, e);

        if (archivo != null)
        {
            var importer = new XmlImporter();
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
        var archivo = await cargarArchivo(sender, e);

        if (archivo != null)
        {
            var importer = new XmlImporter();
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
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        messageWindow.ShowDialog(this); // Mostrar la ventana
    }

    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Verificar si el ComboBox y el elemento seleccionado son válidos
        var opcionesCmbn = this.FindControl<ComboBox>("OpcionesComboBox1");
        if (opcionesCmbn.SelectedItem is ComboBoxItem selectedItem)
        {
            // Obtener el texto de la opción seleccionada
            var seleccion = selectedItem.Content.ToString();
            var PasPen = this.FindControl<StackPanel>("PasadasPendientes");
            var tp = this.FindControl<StackPanel>("transportePendiente");
            var sprc = this.FindControl<StackPanel>("SPRC");

            // Tomar acciones según la selección
            switch (seleccion)
            {
                case "Todos":
                    if (tp != null) tp.IsVisible = false;
                    if (PasPen != null) PasPen.IsVisible = false;
                    if (sprc != null) sprc.IsVisible = false;
                    UpdateDataGrid(Transportes, "DgridTransporte");
                    break;
                case "Transportes pendientes":

                    if (tp != null) tp.IsVisible = true;
                    if (PasPen != null) PasPen.IsVisible = false;
                    if (sprc != null) sprc.IsVisible = false;
                    Console.WriteLine("Opción seleccionada: Transportes pendientes");
                    break;

                case "Reservas pasadas o pendientes":
                    Console.WriteLine("Opción seleccionada: Reservas pasadas o pendientes");

                    if (PasPen != null) PasPen.IsVisible = true;
                    if (tp != null) tp.IsVisible = false;
                    if (sprc != null) sprc.IsVisible = false;
                    break;

                case "Reservas por camión":
                    Console.WriteLine("Opción seleccionada: Reservas por camión");
                    if (PasPen != null) PasPen.IsVisible = false;
                    if (tp != null) tp.IsVisible = false;
                    if (sprc != null) sprc.IsVisible = true;
                    break;

                case "Reservas pendientes":
                    Console.WriteLine("Opción seleccionada: Reservas pendientes");
                    if (PasPen != null) PasPen.IsVisible = true;
                    if (tp != null) tp.IsVisible = false;
                    if (sprc != null) sprc.IsVisible = false;
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
        var opcionesCmbn = this.FindControl<ComboBox>("BusquedaComboBox");
        var opcionesCmbn2 = this.FindControl<ComboBox>("TipoCamionComboBox");
        var TipoCamionLabel = this.FindControl<Label>("TipoCamionLabel");

        if (opcionesCmbn?.SelectedItem is ComboBoxItem selectedItem)
        {
            var seleccion = selectedItem.Content?.ToString();
            if (string.IsNullOrEmpty(seleccion))
            {
                Console.WriteLine("No se seleccionó un valor válido.");
            }
            else
            {
                var esBusquedaPorCamion =
                    seleccion == "Vehiculo"; // esBusquedaPorCamion será true solo si "Camión" está seleccionado
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

    private void BusquedaComboBox_SelectionChangedRC(object sender, SelectionChangedEventArgs e)
    {
        var opcionesCmbn = this.FindControl<ComboBox>("BusquedaComboBoxRC");
        var opcionesCmbn2 = this.FindControl<ComboBox>("TipoCamionComboBoxRC");
        var labelRC = this.FindControl<Label>("LabelRC");

        if (opcionesCmbn?.SelectedItem is ComboBoxItem selectedItem)
        {
            var seleccion = selectedItem.Content?.ToString();
            if (string.IsNullOrEmpty(seleccion))
            {
                Console.WriteLine("No se seleccionó un valor válido.");
            }
            else
            {
                var esBusquedaPorCamion =
                    seleccion == "Vehiculo"; // esBusquedaPorCamion será true solo si "Camión" está seleccionado
                opcionesCmbn2.IsVisible = esBusquedaPorCamion;
                labelRC.IsVisible = esBusquedaPorCamion;
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

    private Cliente checkDni(string dni)
    {
        foreach (var cli in Clientes)
            if (cli.Nif == dni)
                return cli;

        return null;
    }

    private void SelectionChangedFlota(object sender, SelectionChangedEventArgs e)
    {
        var cbFlota = this.FindControl<ComboBox>("OpcionesComboBoxFlota");
        var cbDis = this.FindControl<StackPanel>("SpDis");
        var cbOcu = this.FindControl<StackPanel>("SpOcu");

        if (cbFlota.SelectedItem is ComboBoxItem siFlota)
            switch (siFlota.Content.ToString())
            {
                case "Todos":
                    UpdateDataGrid(Vehiculos, "DgridFlota");
                    if (cbDis != null) cbDis.IsVisible = false;
                    if (cbOcu != null) cbOcu.IsVisible = false;
                    break;
                case "Disponibilidad de vehículos":
                    if (cbDis != null) cbDis.IsVisible = true;
                    if (cbOcu != null) cbOcu.IsVisible = false;
                    break;
                case "Ocupación":
                    if (cbDis != null) cbDis.IsVisible = false;
                    if (cbOcu != null) cbOcu.IsVisible = true;
                    var dateP = this.FindControl<DatePicker>("DpOcu");
                    var dateP2 = this.FindControl<DatePicker>("DpOcu2");

                    dateP.SelectedDate = DateTime.Now;
                    dateP2.SelectedDate = DateTime.Now;
                    break;
            }
    }

    private void RadioButtonChanged()
    {
        var rb = this.FindControl<RadioButton>("rbAnho");
        var fecha = this.FindControl<DatePicker>("DpOcu");
        var fecha2 = this.FindControl<DatePicker>("DpOcu2");

        if (rb.IsChecked.Value)
        {
            fecha.IsVisible = true;
            fecha2.IsVisible = false;
        }
        else
        {
            fecha.IsVisible = false;
            fecha2.IsVisible = true;
        }

        fecha.ApplyTemplate();
    }

    private void BuscarButton_ClickFlota(object sender, RoutedEventArgs e)
    {
        var rb = this.FindControl<RadioButton>("rbAnho");
        var fecha = this.FindControl<DatePicker>("DpOcu");
        var fecha2 = this.FindControl<DatePicker>("DpOcu2");
        var porAnho = true;
        var cbFlota = this.FindControl<ComboBox>("OpcionesComboBoxFlota");
        var resultado = new ObservableCollection<Vehiculo>();

        if (cbFlota.SelectedItem is ComboBoxItem siFlota)
        {
            switch (siFlota.Content.ToString())
            {
                case "Disponibilidad de vehículos":
                    var tvDis = this.FindControl<ComboBox>("tipoVDis");

                    if (tvDis.SelectedItem is ComboBoxItem tipoDis)
                    {
                        TipoVehiculo? tipoVehiculo = tipoDis.Content switch
                        {
                            "Furgoneta" => TipoVehiculo.Furgoneta,
                            "Camión" => TipoVehiculo.Camion,
                            "Camión Articulado" => TipoVehiculo.CamionArticulado,
                            _ => null // "Todos" o ninguna selección aplica null para obtener todos los tipos
                        };

                        resultado = busqueda.BuscarDisponibilidad(tipoVehiculo);
                    }

                    break;
                case "Ocupación":
                    var aux = fecha;
                    if (rb.IsChecked.Value)
                    {
                        porAnho = true;
                        aux = fecha;
                    }
                    else
                    {
                        aux = fecha2;
                        porAnho = false;
                    }

                    if (fecha.SelectedDate != null)
                    {
                        resultado = busqueda.BuscarOcupacion(aux.SelectedDate, porAnho);
                    }

                    break;
            }
        }


        UpdateDataGrid(resultado, "DgridFlota");
    }

    private void BuscarButton_Click(object sender, RoutedEventArgs e)
    {
        //Determinar el tipo de búsqueda
        var opCB = this.FindControl<ComboBox>("OpcionesComboBox1");
        var bcb = this.FindControl<ComboBox>("BusquedaComboBox");
        var tipoSeleccionado = this.FindControl<ComboBox>("TipoCamionComboBox");
        var listBox2 = this.FindControl<ListBox>("ResultadosListBox");
        var resultado = new ObservableCollection<Transporte>();
        var tb = this.FindControl<TextBox>("DniCliente");
        var anho = this.FindControl<DatePicker>("AnhoPP");
        var toret = false;

        if (opCB.SelectedItem is ComboBoxItem selectedItem)
            switch (selectedItem.Content)
            {
                case "Transportes pendientes":
                    if (bcb.SelectedItem is ComboBoxItem selectedItem2)
                    {
                        if (selectedItem2.Content.ToString() == "Vehiculo")
                        {
                            Console.WriteLine("Camion");

                            // Filtra por el tipo de camión seleccionado
                            if (tipoSeleccionado.SelectedItem is ComboBoxItem selectedItem3)
                            {
                                // Convertimos el tipo seleccionado a TipoCamion enum
                                TipoVehiculo? tipoVehiculo = selectedItem3.Content switch
                                {
                                    "Furgoneta" => TipoVehiculo.Furgoneta,
                                    "Camión" => TipoVehiculo.Camion,
                                    "Camión Articulado" => TipoVehiculo.CamionArticulado,
                                    _ => null // "Todos" o ninguna selección aplica null para obtener todos los tipos
                                };
                                Console.WriteLine("Value = " + tipoVehiculo.Value);

                                // Llama al método centralizado para buscar transportes pendientes por camión
                                resultado = busqueda.BuscarTransportesPendientes(true, tipoVehiculo);
                            }
                        }
                        else if (selectedItem2.Content.ToString() == "Flota")
                        {
                            Console.WriteLine("Flota");
                            // Llama al método centralizado para buscar transportes pendientes para toda la flota
                            resultado = busqueda.BuscarTransportesPendientes(false, null);
                        }
                    }

                    break;
                case "Reservas pasadas o pendientes":
                    var cliente = checkDni(tb.Text);
                    if (cliente != null)
                    {
                        if (anho.SelectedDate == null)
                            resultado = busqueda.BuscarReservasPorCliente(cliente,
                                null);
                        else
                            resultado = busqueda.BuscarReservasPorCliente(cliente,
                                anho.SelectedDate.Value.Year);
                    }

                    break;
                case "Reservas por camión":
                    var bc = this.FindControl<ComboBox>("BusquedaComboBoxRC");
                    var anhorc = this.FindControl<DatePicker>("AnhoPPRC");
                    var tipoV = this.FindControl<ComboBox>("TipoCamionComboBoxRC");

                    if (bc.SelectedItem is ComboBoxItem selectedItemRC)
                    {
                        Console.WriteLine("Entro RC");
                        if (selectedItemRC.Content.ToString() == "Flota")
                        {
                            Console.WriteLine("Entro flota RC");
                            if (anhorc.SelectedDate == null)
                            {
                                Console.WriteLine("Flota- null");
                                resultado = busqueda.BuscarReservasPorCamion(null,
                                    null);
                            }
                            else
                            {
                                Console.WriteLine("Flota- NOTnull");
                                resultado = busqueda.BuscarReservasPorCamion(null,
                                    anhorc.SelectedDate.Value.Year);
                            }
                        }
                        else
                        {
                            if (tipoV.SelectedItem is ComboBoxItem tipoI)
                            {
                                TipoVehiculo? tipoVehiculo = tipoI.Content switch
                                {
                                    "Furgoneta" => TipoVehiculo.Furgoneta,
                                    "Camión" => TipoVehiculo.Camion,
                                    "Camión Articulado" => TipoVehiculo.CamionArticulado,
                                    _ => null // "Todos" o ninguna selección aplica null para obtener todos los tipos
                                };
                                if (anhorc.SelectedDate == null)
                                    resultado = busqueda.BuscarReservasPorCamion(tipoVehiculo,
                                        null);
                                else
                                    resultado = busqueda.BuscarReservasPorCamion(tipoVehiculo,
                                        anhorc.SelectedDate.Value.Year);
                            }
                        }
                    }

                    break;
                case "Reservas pendientes":
                    Console.WriteLine("Entro en reservas pendientes");
                    var client = checkDni(tb.Text);
                    if (client != null)
                    {
                        Console.WriteLine("Entro en el cliente no null");
                        if (anho.SelectedDate == null)
                        {
                            Console.WriteLine("Entro en el año null");
                            resultado = busqueda.BuscarReservasParaPersona(client,
                                null);
                        }
                        else
                        {
                            Console.WriteLine("Entro en el año null");
                            resultado = busqueda.BuscarReservasParaPersona(client,
                                anho.SelectedDate.Value.Year);
                        }
                    }

                    Console.WriteLine("Count = " + resultado.Count);
                    break;
            }

        // Mostrar los resultados en el ListBox
        UpdateDataGrid(resultado, "DgridTransporte");
    }

    private void StartChart()
    {
        int[] values = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        var cont = 0;

        foreach (var t in Transportes)
            if (t.FechaContratacion.Year == 2024)
                values[t.FechaContratacion.Month - 1]++;

        _dataMes = values;

        values = new[] { 0, 0, 0, 0, 0 };

        foreach (var t in Transportes)
            if (t.FechaContratacion.Year == 2024)
                values[4]++;
            else if (t.FechaContratacion.Year == 2023)
                values[3]++;
            else if (t.FechaContratacion.Year == 2022)
                values[2]++;
            else if (t.FechaContratacion.Year == 2021)
                values[1]++;
            else if (t.FechaContratacion.Year == 2020) values[0]++;

        _dataAnho = values;


        DrawChart(0);
    }

    private void DrawChart(int x)
    {
        Chart.LegendY = _labelY;
        if (x == 0)
        {
            Chart.LegendX = _labelMes;
            Chart.Values = _dataMes;
            Chart.Labels = _ejeMes;
        }
        else
        {
            Chart.LegendX = _labelAnho;
            Chart.Values = _dataAnho;
            Chart.Labels = _ejeAnho;
        }


        _chartType = Chart.ChartType.Lines;
        Chart.DataPen = new Pen(Brushes.Red, 2 * ((double?)2 ?? 1));
        Chart.Draw();
    }

    private void DrawChart2()
    {
        Chart.LegendY = "Número camiones";
        Chart.LegendX = "Comodidades";
        Chart.Labels = new[] { "Bluetooth", "Wifi", "Litera", "AC", "TV" };

        int[] values = { 0, 0, 0, 0, 0 };

        foreach (var v in Vehiculos)
        {
            if (v.Comodidades.Contains("Conexión del móbil por Bluetooth")) values[0]++;

            if (v.Comodidades.Contains("Wifi")) values[1]++;

            if (v.Comodidades.Contains("Litera de Descanso")) values[2]++;

            if (v.Comodidades.Contains("Aire Acondicionado")) values[3]++;

            if (v.Comodidades.Contains("TV")) values[4]++;
        }

        _dataMes = values;
        _dataAnho = values;

        Chart.Values = _dataMes;
        _chartType = Chart.ChartType.Lines;
        Chart.DataPen = new Pen(Brushes.Red, 2 * ((double?)2 ?? 1));
        Chart.Draw();
    }

    private async void Dgrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Transporte selectedTransporte)
        {
            // Deshabilitar temporalmente el evento SelectionChanged
            dataGrid.SelectionChanged -= dgridHandler;

            var transportDetailsWindow = new DataViewTransporte(
                selectedTransporte,
                Transportes,
                modifiedTransporte =>
                {
                    if (modifiedTransporte == null)
                    {
                        Transportes.Remove(selectedTransporte);
                    }
                    else
                    {
                        int index = Transportes.IndexOf(selectedTransporte);
                        if (index >= 0)
                        {
                            Transportes[index] = modifiedTransporte;
                        }
                    }

                    // Refrescar el DataGrid después de modificar
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = Transportes;
                });

            await transportDetailsWindow.ShowDialog(this);
            
            UpdateDataGrid(Transportes, "DgridTransporte");

            // Limpiar la selección y volver a habilitar el evento
            dataGrid.SelectedItem = null;
            dataGrid.SelectionChanged += dgridHandler;
        }
    }


    //ficha cliente
    private async void DgridCliente_SelectionChanged(object? sender)
    {
        var dgridcliente = this.FindControl<DataGrid>("DgridClientes");

        dgridcliente.SelectionChanged -= dgridclienteHandler;

        if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Cliente selectedCliente)
        {
            var clienteDetailsWindow = new ClienteDetailWindow(selectedCliente, Clientes);

            clienteDetailsWindow.ClienteBorrado += (cliente) =>
            {
                var clienteAEliminar = Clientes.FirstOrDefault(cli => cli.Nif == cliente.Nif);

                // Si se encuentra, eliminarlo
                if (clienteAEliminar != null)
                {
                    Clientes.Remove(clienteAEliminar);
                }

                FiltrarClientes(this.FindControl<TextBox>("SearchTextBox").Text ?? string.Empty);
                UpdateDataGrid(FiltradoClientes, "DgridClientes");
            };
            await clienteDetailsWindow.ShowDialog(this);

            if (!clienteDetailsWindow.IsCancelled)
            {
                FiltrarClientes(this.FindControl<TextBox>("SearchTextBox").Text ?? string.Empty);
                UpdateDataGrid(FiltradoClientes, "DgridClientes");
            }

            FiltrarClientes(this.FindControl<TextBox>("SearchTextBox").Text ?? string.Empty);
            UpdateDataGrid(FiltradoClientes, "DgridClientes");

            // Limpiar selección
            dataGrid.SelectedItem = null;
        }

        dgridcliente.SelectionChanged += dgridclienteHandler;
    }

    private void ConfigureButtonEvents()
    {
        var addButton = this.FindControl<Button>("AddButton");
        var searchButton = this.FindControl<Button>("SearchButton");

        if (addButton != null)
            addButton.Click += AddButton_Click;

        // if (searchButton != null)
        //     searchButton.Click += SearchButton_Click;
    }

    private async void AddButton_Click(object? sender, RoutedEventArgs e)
    {
        var selectedTab = this.FindControl<TabControl>("tabControlMenu").SelectedItem as TabItem;
        if (selectedTab?.Header.ToString() == "Transportes")
        {
            var addTransportWindow =
                new AddTransportWindow(Transportes.ToList(), Clientes.ToList(), Vehiculos.ToList());
            await addTransportWindow.ShowDialog(this);

            if (addTransportWindow.NuevoTransporte != null)
            {
                Transportes.Add(addTransportWindow.NuevoTransporte);
            }
        }
        else
        {
            // Lógica para otras pestañas si se necesita
            Console.WriteLine("Función de añadir no implementada para esta pestaña.");
        }
    }

    /*private async void SearchButton_Click(object? sender, RoutedEventArgs e)
    {
        var selectedTab = this.FindControl<TabControl>("tabControlMenu").SelectedItem as TabItem;

        if (selectedTab?.Header.ToString() == "Transportes")
        {
            var consultTransportWindow = new ConsultTransportWindow(Transportes.ToList());
            await consultTransportWindow.ShowDialog(this);
        }
        else
        {
            Console.WriteLine("Función de búsqueda no implementada para esta pestaña.");
        }
    }*/

    
    private void OnSearchKeyUpTransporte(object sender, KeyEventArgs e)
    {
        string searchText = this.FindControl<TextBox>("SearchTransporteTextBox").Text ?? string.Empty;
        FiltrarTransportes(searchText);
    }

    private void FiltrarTransportes(string searchText)
    {
        searchText = searchText.ToLower();
        var filtradoTransportes = new ObservableCollection<Transporte>();

        foreach (var transporte in Transportes.Where(t =>
                     t.Id.ToLower().Contains(searchText) ||
                     t.Tipo.ToString().ToLower().Contains(searchText) ||
                     t.Cliente.Nombre.ToLower().Contains(searchText) ||
                     t.FechaEntrega.ToString().ToLower().Contains(searchText)))
        {
            filtradoTransportes.Add(transporte);
        }

        // Actualizar el DataGrid con la lista filtrada
        UpdateDataGrid(filtradoTransportes, "DgridTransporte");
    }


    //Busqueda clientes
    private void OnSearchKeyUpCliente(object sender, KeyEventArgs e)
    {
        string searchText = this.FindControl<TextBox>("SearchTextBox").Text ?? string.Empty;
        FiltrarClientes(searchText);
    }

    private void FiltrarClientes(string searchText)
    {
        searchText = searchText.ToLower();
        FiltradoClientes.Clear();

        foreach (var cliente in this.Clientes.Where(c =>
                     c.Nif.ToLower().Contains(searchText) ||
                     c.Nombre.ToLower().Contains(searchText) ||
                     c.Telefono.ToLower().Contains(searchText) ||
                     c.Email.ToLower().Contains(searchText) ||
                     c.DireccionPostal.ToLower().Contains(searchText)))
        {
            FiltradoClientes.Add(cliente);
        }
    }

    //añadir clientes
    private async void AnadirCliente_Click(object? sender, RoutedEventArgs e)
    {
        var clientesaddwindow = new AddClienteWindow(Clientes);
        //clientesaddwindow.Closed += (s, args) => clientesaddwindow.IsCancelled = true;
        await clientesaddwindow.ShowDialog(this);
        FiltrarClientes(this.FindControl<TextBox>("SearchTextBox").Text ?? string.Empty);
    }


    //flota
    private async void OnAñadirVehiculoClick(object? sender, RoutedEventArgs e)
    {
        var crearVehiculoWindow = new CrearVehiculoWindow(Vehiculos);
        await crearVehiculoWindow.ShowDialog(this);

        // Check if a new vehicle was created
        if (crearVehiculoWindow.NuevoVehiculo != null)
        {
            Vehiculos.Add(crearVehiculoWindow.NuevoVehiculo);
            Console.WriteLine($"Vehículo añadido: {crearVehiculoWindow.NuevoVehiculo.Matricula}");

            // Update the DataGrid
            UpdateDataGrid(Vehiculos, "DgridFlota");

            // Save to XML
            var xmlExporter = new XmlExporter();
            xmlExporter.ExportarFlotaXML(Vehiculos, Path.Combine(rutaXml, "flota.xml"));
        }
        else
        {
            Console.WriteLine("No se añadió ningún vehículo.");
        }
    }

    private async void Dgrid_SelectionChanged2(object? sender)
    {
        var flota = this.FindControl<DataGrid>("DgridFlota");

        flota.SelectionChanged -= flotaHandler;
        if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Vehiculo selectedVehiculo)
        {
            Console.WriteLine(selectedVehiculo.FechaAdquisicion);

            var reservasPorCamionWindow = new DataViewFlota(selectedVehiculo);
            Console.WriteLine("Editar tesssss");
            await reservasPorCamionWindow.ShowDialog(this);

            // UpdateDataGrid(Transportes, "DgridTransporte");
            dataGrid.SelectedItem = null;
        }

        flota.SelectionChanged += flotaHandler;
    }

    private void OnBuscarVehiculoClick(object? sender, RoutedEventArgs e)
    {
        var matriculaBuscar = this.FindControl<TextBox>("BuscarMatriculaTextBox").Text;

        if (string.IsNullOrWhiteSpace(matriculaBuscar))
        {
            Console.WriteLine("Por favor, introduce una matrícula para buscar.");
            return;
        }

        // Filtrar la lista de vehículos por matrícula
        var vehiculoEncontrado =
            Vehiculos.FirstOrDefault(v => v.Matricula.Equals(matriculaBuscar, StringComparison.OrdinalIgnoreCase));

        if (vehiculoEncontrado != null)
        {
            Console.WriteLine($"Vehículo encontrado: {vehiculoEncontrado.Matricula}");

            // Seleccionar el vehículo en el DataGrid
            var dataGrid = this.FindControl<DataGrid>("DgridFlota");
            if (dataGrid != null)
            {
                dataGrid.SelectedItem = vehiculoEncontrado;
                dataGrid.ScrollIntoView(vehiculoEncontrado, null); // Aquí pasamos null para evitar el error
            }
        }
        else
        {
            Console.WriteLine("No se encontró ningún vehículo con la matrícula especificada.");
        }
    }
}