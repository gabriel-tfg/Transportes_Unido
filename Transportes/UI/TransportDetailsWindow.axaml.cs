using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using Transportes.Core;

namespace Transportes.UI
{
    public partial class TransportDetailsWindow : Window
    {
        private Transporte _transporte;
        private readonly Action<Transporte?> _onTransportModified;
        public ObservableCollection<Transporte> Transportes { get; set; }

        public TransportDetailsWindow(Transporte transporte, ObservableCollection<Transporte> transportes, Action<Transporte?> onTransportModified)
        {
            InitializeComponent();
            _transporte = transporte;
            Transportes = transportes;
            _onTransportModified = onTransportModified;

            // Mostrar los datos del transporte
            //UpdateTransportDetails();

            // Configurar eventos de botones
            GenerateInvoiceButton.Click += GenerateInvoiceButton_Click;
            CloseButton.Click += CloseButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        private void GenerateInvoiceButton_Click(object? sender, RoutedEventArgs e)
        {
            var generateInvoiceWindow = new GenerateInvoiceWindow(_transporte);
            generateInvoiceWindow.ShowDialog(this);
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_transporte == null)
            {
                // Si no hay un transporte seleccionado, no hacemos nada
                return;
            }

            // Crear y mostrar la ventana de confirmación de eliminación
            var deleteWindow = new DeleteTransportWindow(_transporte, transporteEliminado =>
            {
                if (transporteEliminado != null)
                {
                    // Eliminar el transporte de la colección y notificar
                    Transportes.Remove(transporteEliminado);
                    _onTransportModified(null); // Notificar que el transporte fue eliminado
                    Close(); // Cerrar esta ventana después de confirmar la eliminación
                }
            });

            await deleteWindow.ShowDialog(this); // Mostrar como ventana modal
        }
/*
        private void ModifyButton_Click(object? sender, RoutedEventArgs e)
        {
            var modifyTransportWindow = new ModifyTransportWindow(_transporte, updatedTransporte =>
            {
                if (updatedTransporte != null)
                {
                    // Buscar el índice del transporte actual en la colección
                    int index = Transportes.IndexOf(_transporte);
                    if (index >= 0)
                    {
                        // Reemplazar el transporte modificado en la colección
                        Transportes[index] = updatedTransporte;
                    }

                    // Actualizar la referencia local y los detalles mostrados
                    _transporte = updatedTransporte;
                    UpdateTransportDetails();

                    // Notificar a través del callback
                    _onTransportModified(_transporte);
                }
            });

            modifyTransportWindow.ShowDialog(this);
        }


        private void UpdateTransportDetails()
        {
            if (_transporte == null) return;

            IdTextBlock.Text = _transporte.Id ?? "N/A";
            TipoTextBlock.Text = _transporte.Tipo.ToString();
            ClienteTextBlock.Text = _transporte.Cliente?.Nombre ?? "Sin cliente asignado";
            VehiculoTextBlock.Text = _transporte.Vehiculo != null
                ? $"{_transporte.Vehiculo.Tipo} ({_transporte.Vehiculo.Matricula})"
                : "Sin vehículo asignado";

            FechaContratacionTextBlock.Text = _transporte.FechaContratacion.ToString("dd/MM/yyyy");
            FechaSalidaTextBlock.Text = _transporte.FechaSalida.ToString("dd/MM/yyyy");
            FechaEntregaTextBlock.Text = _transporte.FechaEntrega.ToString("dd/MM/yyyy");
            KilometrosTextBlock.Text = $"{_transporte.KilometrosRecorridos} km";
            ImportePorDiaTextBlock.Text = $"{_transporte.ImportePorDia:0.00} €";
            ImportePorKilometroTextBlock.Text = $"{_transporte.ImportePorKilometro:0.00} €";
            IvaTextBlock.Text = $"{_transporte.IvaAplicado * 100:0.0}%";

            double dias = (_transporte.FechaEntrega - _transporte.FechaSalida).TotalDays;
            double totalSinIva = (dias * _transporte.ImportePorDia) +
                                 (_transporte.KilometrosRecorridos * _transporte.ImportePorKilometro);
            double totalConIva = totalSinIva * (1 + _transporte.IvaAplicado);
            TotalConIvaTextBlock.Text = $"{totalConIva:0.00} €";
        }*/
    }
}
