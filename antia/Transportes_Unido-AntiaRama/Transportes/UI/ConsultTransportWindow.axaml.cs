using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Transportes.Core;

namespace Transportes.UI;

public partial class ConsultTransportWindow : Window
{
    private List<Transporte> _transportes;
    public ObservableCollection<Transporte> TransportList { get; set; }

    public ConsultTransportWindow(List<Transporte> transportes)
    {
        InitializeComponent();
        _transportes = transportes;
        TransportList = new ObservableCollection<Transporte>();
        ConsultButton.Click += ConsultButton_Click;

        // Enlazar DataGrid con la lista
        TransportDataGrid.ItemsSource = TransportList;
    }

    private void ConsultButton_Click(object? sender, RoutedEventArgs e)
    {
        string? id = TransportIdTextBox.Text;

        try
        {
            var transporte = _transportes.Find(t => t.Id == id) ?? throw new Exception("Transporte no encontrado");

            // Limpiar la lista y añadir el transporte encontrado
            TransportList.Clear();
            TransportList.Add(transporte);

            StatusTextBlock.Text = $"Transporte con ID {id} encontrado.";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }
}