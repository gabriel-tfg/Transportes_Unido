using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Transportes.Core;

namespace Transportes.UI;

public partial class DeleteTransportWindow : Window
{
    private List<Transporte> _transportes;

    // Evento para notificar la eliminación de un transporte
    public event Action? TransporteEliminado;

    public DeleteTransportWindow(List<Transporte> transportes)
    {
        InitializeComponent();
        _transportes = transportes;
        DeleteButton.Click += DeleteButton_Click;
    }

    private void DeleteButton_Click(object? sender, RoutedEventArgs e)
    {
        string? id = TransportIdTextBox.Text;

        try
        {
            var transporte = _transportes.Find(t => t.Id == id) ?? throw new Exception("Transporte no encontrado");
            _transportes.Remove(transporte);
            StatusTextBlock.Text = "Transporte eliminado correctamente";

            // Invocar el evento para notificar la eliminación
            TransporteEliminado?.Invoke();
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }
}