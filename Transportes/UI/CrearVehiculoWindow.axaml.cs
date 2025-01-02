using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Transportes.Core;

namespace Transportes.UI;

public partial class CrearVehiculoWindow : Window
{
    private readonly ObservableCollection<Vehiculo> _vehiculosExistentes;

    public Vehiculo? NuevoVehiculo { get; private set; }
    public CrearVehiculoWindow(ObservableCollection<Vehiculo> vehiculos)
    {
        InitializeComponent();
        _vehiculosExistentes = vehiculos; // Store the existing vehicles list
    }

    private bool EsFormatoMatriculaValido(string matricula)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(matricula, @"^\d{4}[A-Z,a-z]{3}$");
    }

    private void OnAñadirVehiculoButtonClick(object? sender, RoutedEventArgs e)
{
    var matricula = this.FindControl<TextBox>("MatriculaTextBox").Text?.Trim();
    var tipoIndex = this.FindControl<ComboBox>("TipoVehiculoComboBox").SelectedIndex;
    var marca = this.FindControl<TextBox>("MarcaTextBox").Text?.Trim();
    var modelo = this.FindControl<TextBox>("ModeloTextBox").Text?.Trim();
    var consumoPorKmText = this.FindControl<TextBox>("ConsumoPorKmTextBox").Text?.Trim();
    var fechaAdquisicion = this.FindControl<DatePicker>("FechaAdquisicionPicker").SelectedDate?.DateTime;
    var fechaFabricacion = this.FindControl<DatePicker>("FechaFabricacionPicker").SelectedDate?.DateTime;

    // Capturar comodidades seleccionadas
    var comodidades = new List<string>();
    if (this.FindControl<CheckBox>("chkWifi")?.IsChecked == true) comodidades.Add("Wifi");
    if (this.FindControl<CheckBox>("chkBluetooth")?.IsChecked == true) comodidades.Add("Conexión del móvil por Bluetooth");
    if (this.FindControl<CheckBox>("chkAireAcondicionado")?.IsChecked == true) comodidades.Add("Aire Acondicionado");
    if (this.FindControl<CheckBox>("chkLitera")?.IsChecked == true) comodidades.Add("Litera de Descanso");
    if (this.FindControl<CheckBox>("chkTV")?.IsChecked == true) comodidades.Add("TV");

    // Validación de campos obligatorios
    if (string.IsNullOrWhiteSpace(matricula) || tipoIndex < 0 || string.IsNullOrWhiteSpace(marca) || 
        string.IsNullOrWhiteSpace(modelo) || string.IsNullOrWhiteSpace(consumoPorKmText) || !fechaAdquisicion.HasValue || 
        !fechaFabricacion.HasValue)
    {
        Console.WriteLine("Todos los campos obligatorios deben estar completos.");
        return;
    }

    if (_vehiculosExistentes.Any(v => v.Matricula.Equals(matricula, StringComparison.OrdinalIgnoreCase)))
    {
        Console.WriteLine($"Ya existe un vehículo con la matrícula '{matricula}'. No se puede añadir duplicados.");
        return;
    }
    
    // Validar matrícula
    if (!EsFormatoMatriculaValido(matricula))
    {
        Console.WriteLine("Formato de matrícula incorrecto. Debe ser 4 numeros y 3 letras.");
        return;
    }

    // Validar consumo
    if (!double.TryParse(consumoPorKmText, out var consumoPorKm))
    {
        Console.WriteLine("Consumo por Km debe ser un número válido.");
        return;
    }

    // Crear nuevo vehículo
    var nuevoVehiculo = new Vehiculo
    {
        Matricula = matricula.Substring(0,4) + matricula.Substring(4, 3).ToUpper(),
        Tipo = (TipoVehiculo)tipoIndex,
        Marca = marca,
        Modelo = modelo,
        ConsumoPorKm = consumoPorKm,
        FechaAdquisicion = fechaAdquisicion.Value,
        FechaFabricacion = fechaFabricacion.Value,
        Comodidades = comodidades
    };

    Console.WriteLine($"Nuevo vehículo añadido: {nuevoVehiculo.Matricula}");
    Console.WriteLine($"Comodidades: {string.Join(", ", comodidades)}");

// Set the new vehicle and close the window
    NuevoVehiculo = nuevoVehiculo;
    Close();
}


}
