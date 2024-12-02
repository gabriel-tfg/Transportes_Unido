using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Transportes.Core;

namespace Transportes.UI
{
    public partial class ModifyTransportWindow : Window
    {
        private readonly Transporte _transporte;
        private readonly Action<Transporte> _onModify;

        public ModifyTransportWindow(Transporte transporte, Action<Transporte> onModify)
        {
            InitializeComponent();
            _transporte = transporte ?? throw new ArgumentNullException(nameof(transporte));
            _onModify = onModify ?? throw new ArgumentNullException(nameof(onModify));

            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                string selectedProperty = (PropertyComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
                string newValue = NewValueTextBox.Text ?? string.Empty;

                switch (selectedProperty)
                {
                    case "Tipo":
                        _transporte.Tipo = Enum.TryParse(newValue, out TipoTransporte tipo) ? tipo : throw new Exception("Tipo inválido");
                        break;
                    case "KilometrosRecorridos":
                        _transporte.KilometrosRecorridos = double.TryParse(newValue, out double km) ? km : throw new Exception("Kilómetros inválidos");
                        break;
                    case "FechaSalida":
                        _transporte.FechaSalida = DateTime.TryParse(newValue, out DateTime salida) ? salida : throw new Exception("Fecha inválida");
                        break;
                    case "FechaEntrega":
                        _transporte.FechaEntrega = DateTime.TryParse(newValue, out DateTime entrega) ? entrega : throw new Exception("Fecha inválida");
                        break;
                    case "ImportePorDia":
                        _transporte.ImportePorDia = double.TryParse(newValue, out double dia) ? dia : throw new Exception("Importe inválido");
                        break;
                    case "ImportePorKilometro":
                        _transporte.ImportePorKilometro = double.TryParse(newValue, out double kmImporte) ? kmImporte : throw new Exception("Importe inválido");
                        break;
                    case "IvaAplicado":
                        _transporte.IvaAplicado = double.TryParse(newValue, out double iva) ? iva : throw new Exception("IVA inválido");
                        break;
                    default:
                        throw new Exception("Propiedad no válida seleccionada");
                }

                _onModify(_transporte);
                Close();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = ex.Message;
            }
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
