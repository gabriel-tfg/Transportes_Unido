using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Transportes.Core;

namespace Transportes.UI
{
    public partial class GenerateInvoiceWindow : Window
    {
        private Transporte _transporte;

        public GenerateInvoiceWindow(Transporte transporte)
        {
            InitializeComponent();
            _transporte = transporte;

            // Configurar evento del botón de generar factura
            GenerateButton.Click += GenerateButton_Click;

            // Mostrar información inicial del transporte
            InvoiceTextBlock.Text = 
                $"Generar factura para el transporte:\n" +
                $"ID: {_transporte.Id}\n" +
                $"Cliente: {_transporte.Cliente?.Nombre ?? "Sin cliente"}\n" +
                $"Vehículo: {_transporte.Vehiculo?.Matricula ?? "Sin vehículo"}\n" +
                $"Fechas: {_transporte.FechaSalida:dd/MM/yyyy} - {_transporte.FechaEntrega:dd/MM/yyyy}\n" +
                $"Kilómetros: {_transporte.KilometrosRecorridos} km\n";
        }

        private void GenerateButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!double.TryParse(FuelPriceTextBox.Text, out var fuelPrice) || fuelPrice <= 0)
            {
                InvoiceTextBlock.Text = "Precio de combustible no válido.";
                return;
            }

            if (!double.TryParse(HourlyWageTextBox.Text, out var hourlyWage) || hourlyWage <= 0)
            {
                InvoiceTextBlock.Text = "Sueldo por hora no válido.";
                return;
            }

            try
            {
                // Calcular precio por día
                double ppd = _transporte.ImportePorDia > 0 ? _transporte.ImportePorDia : hourlyWage * 8;

                // Calcular número de días
                int numd = (int)(_transporte.FechaEntrega - _transporte.FechaSalida).TotalDays;
                if (numd < 0)
                {
                    InvoiceTextBlock.Text = "Fechas de transporte no válidas.";
                    return;
                }

                // Calcular suplencia
                double suplencia = numd > 1 ? 2 : 1;

                // Calcular precio por kilómetro
                double ppkm = 3 * fuelPrice;

                // Validar consumo por kilómetro
                if (_transporte.Vehiculo.ConsumoPorKm <= 0 || _transporte.Vehiculo.ConsumoPorKm > 50)
                {
                    InvoiceTextBlock.Text = "El consumo por kilómetro del vehículo no es válido.";
                    return;
                }

                // Calcular gasolina consumida en litros
                double gasLitros = (_transporte.KilometrosRecorridos * _transporte.Vehiculo.ConsumoPorKm) / 100;

                // Calcular coste de gasolina
                double gasCoste = gasLitros * fuelPrice;

                // Calcular el precio total
                double precioTotal = (numd * ppd * suplencia) + (_transporte.KilometrosRecorridos * ppkm) + gasCoste;

                // Mostrar desglose en la factura
                InvoiceTextBlock.Text =
                    $"Cliente: {_transporte.Cliente.Nombre}\n" +
                    $"Vehículo: {_transporte.Vehiculo.Matricula}\n" +
                    $"Días: {numd}\n" +
                    $"Precio por día: {ppd:C2}\n" +
                    $"Kilómetros recorridos: {_transporte.KilometrosRecorridos}\n" +
                    $"Precio por kilómetro: {ppkm:C2}\n" +
                    $"Gasolina consumida: {gasLitros:N2} litros\n" +
                    $"Coste gasolina: {gasCoste:C2}\n" +
                    $"IVA aplicado: {_transporte.IvaAplicado}%\n" +
                    $"Total: {precioTotal * (1 + _transporte.IvaAplicado / 100):C2}";
            }
            catch (Exception ex)
            {
                InvoiceTextBlock.Text = $"Error al generar factura: {ex.Message}";
            }
        }
    }
}