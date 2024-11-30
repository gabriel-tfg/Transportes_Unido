using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Transportes.Core;

namespace Transportes.UI;

public partial class TransportDetailsWindow : Window
{
    public Transporte Transporte { get; }

    public TransportDetailsWindow(Transporte transporte)
    {
        InitializeComponent();
        Transporte = transporte;

        // Enlaza los datos del transporte a los controles de la ventana
        this.DataContext = Transporte;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }

}
