using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Transportes.Core;

namespace Transportes.UI;

public partial class ModifyTransportWindow : Window
{
    private List<Transporte> _transportes;
    private Transporte? _selectedTransporte;

    public ModifyTransportWindow(List<Transporte> transportes)
    {
        InitializeComponent();
        _transportes = transportes ?? throw new ArgumentNullException(nameof(transportes));
        SearchButton.Click += SearchButton_Click;
        ModifyButton.Click += ModifyButton_Click;
    }

    private void SearchButton_Click(object? sender, RoutedEventArgs e)
    {
        string id = TransportIdTextBox.Text ?? string.Empty;

        try
        {
            _selectedTransporte = _transportes.Find(t => t.Id == id) ?? throw new Exception("Transporte no encontrado");
            StatusTextBlock.Text = $"Transporte encontrado: {_selectedTransporte.Id}";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void ModifyButton_Click(object? sender, RoutedEventArgs e)
{
    if (_selectedTransporte == null)
    {
        StatusTextBlock.Text = "Primero busca un transporte";
        return;
    }

    string selectedProperty = (PropertyComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
    string newValue = NewValueTextBox.Text ?? string.Empty;

    try
    {
        switch (selectedProperty)
        {
            case "Tipo":
                if (Enum.TryParse(newValue, out TipoTransporte tipo))
                {
                    _selectedTransporte.Tipo = tipo;
                }
                else
                {
                    throw new Exception("Tipo de transporte inválido.");
                }
                break;

            case "KilometrosRecorridos":
                if (double.TryParse(newValue, out double kilometros))
                {
                    _selectedTransporte.KilometrosRecorridos = kilometros;
                }
                else
                {
                    throw new Exception("Valor inválido para kilómetros recorridos.");
                }
                break;

            case "FechaSalida":
                if (DateTime.TryParse(newValue, out DateTime fechaSalida))
                {
                    _selectedTransporte.FechaSalida = fechaSalida;
                }
                else
                {
                    throw new Exception("Formato de fecha inválido.");
                }
                break;

            case "FechaEntrega":
                if (DateTime.TryParse(newValue, out DateTime fechaEntrega))
                {
                    _selectedTransporte.FechaEntrega = fechaEntrega;
                }
                else
                {
                    throw new Exception("Formato de fecha inválido.");
                }
                break;

            case "ImportePorDia":
                if (double.TryParse(newValue, out double importePorDia))
                {
                    _selectedTransporte.ImportePorDia = importePorDia;
                }
                else
                {
                    throw new Exception("Valor inválido para importe por día.");
                }
                break;

            case "ImportePorKilometro":
                if (double.TryParse(newValue, out double importePorKilometro))
                {
                    _selectedTransporte.ImportePorKilometro = importePorKilometro;
                }
                else
                {
                    throw new Exception("Valor inválido para importe por kilómetro.");
                }
                break;

            case "IvaAplicado":
                if (double.TryParse(newValue, out double iva))
                {
                    _selectedTransporte.IvaAplicado = iva;
                }
                else
                {
                    throw new Exception("Valor inválido para IVA.");
                }
                break;

            default:
                throw new Exception("Propiedad no válida seleccionada.");
        }

        // Actualizar la lista de transportes
        int index = _transportes.FindIndex(t => t.Id == _selectedTransporte.Id);
        if (index >= 0)
        {
            _transportes[index] = _selectedTransporte;
        }

        StatusTextBlock.Text = "Transporte modificado correctamente.";
    }
    catch (Exception ex)
    {
        StatusTextBlock.Text = ex.Message;
    }
}

}


