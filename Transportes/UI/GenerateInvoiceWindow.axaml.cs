using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Transportes.Core;

namespace Transportes.UI
{
    public partial class GenerateInvoiceWindow : Window
    {
        private List<Transporte> _transportes;

        public GenerateInvoiceWindow(List<Transporte> transportes)
        {
            InitializeComponent();
            this._transportes = transportes;
            GenerateButton.Click += GenerateButton_Click;
        }

        private void GenerateButton_Click(object? sender, RoutedEventArgs e)
        {
            string? transportId = TransportIdTextBox.Text;
            if (string.IsNullOrWhiteSpace(transportId))
            {
                InvoiceTextBlock.Text = "ID de transporte no válido.";
                return;
            }

            var transporte = _transportes.Find(t => t.Id == transportId);
            if (transporte == null)
            {
                InvoiceTextBlock.Text = "Transporte no encontrado.";
                return;
            }

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
                // Calcular el precio por día (ppd)
                double ppd = transporte.ImportePorDia > 0 ? transporte.ImportePorDia : hourlyWage * 8;

                // Calcular el número de días (numd)
                int numd = (transporte.FechaEntrega - transporte.FechaSalida).Days;
                if (numd < 0)
                {
                    InvoiceTextBlock.Text = "Fechas de transporte no válidas.";
                    return;
                }

                // Calcular suplencia
                double suplencia = numd > 1 ? 2 : 1;

                // Calcular precio por kilómetro (ppkm)
                double ppkm = 3 * fuelPrice;

                // Validar consumo por kilómetro
                if (transporte.Vehiculo.ConsumoPorKm <= 0 || transporte.Vehiculo.ConsumoPorKm > 50)
                {
                    InvoiceTextBlock.Text = "El consumo por kilómetro del vehículo no es válido.";
                    return;
                }

                // Calcular gasolina consumida en litros
                var gasLitros = (transporte.KilometrosRecorridos * transporte.Vehiculo.ConsumoPorKm) / 100;

                // Calcular coste de gasolina
                double gasCoste = gasLitros * fuelPrice;

                // Calcular el precio total
                double precioTotal = (numd * ppd * suplencia) + (transporte.KilometrosRecorridos * ppkm) + gasCoste;

                // Crear la factura y mostrar el desglose
                InvoiceTextBlock.Text =
                    $"Cliente: {transporte.Cliente.Nombre}\n" +
                    $"Vehículo: {transporte.Vehiculo.Matricula}\n" +
                    $"Días: {numd}\n" +
                    $"Precio por día: {ppd:C2}\n" +
                    $"Kilómetros recorridos: {transporte.KilometrosRecorridos}\n" +
                    $"Precio por kilómetro: {ppkm:C2}\n" +
                    $"Gasolina consumida: {gasLitros:N2} litros\n" +
                    $"Coste gasolina: {gasCoste:C2}\n" +
                    $"IVA aplicado: {transporte.IvaAplicado}%\n" +
                    $"Total: {precioTotal * (1 + transporte.IvaAplicado / 100):C2}";
            }
            catch (Exception ex)
            {
                InvoiceTextBlock.Text = $"Error al generar factura: {ex.Message}";
            }
        }
    }
}
