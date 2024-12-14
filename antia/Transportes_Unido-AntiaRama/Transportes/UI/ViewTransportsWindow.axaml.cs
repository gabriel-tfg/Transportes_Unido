using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Transportes.Core;

namespace Transportes.UI;

public partial class ViewTransportsWindow : Window
{
    public ViewTransportsWindow(List<Transporte> transportes)
    {
        InitializeComponent();
        var data = this.GetControl<DataGrid>("Dgrid");
        
        TransportList = new ObservableCollection<Transporte>(transportes);
        data.ItemsSource = this.TransportList;
        // Línea de depuración para verificar el conteo de elementos
        Console.WriteLine("Total de transportes en la lista: " + TransportList.Count);
    }

    public ObservableCollection<Transporte> TransportList { get; set; }
}