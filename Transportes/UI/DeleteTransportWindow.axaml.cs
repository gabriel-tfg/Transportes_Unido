using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Transportes.Core;

namespace Transportes.UI
{
    public partial class DeleteTransportWindow : Window
    {
        private readonly Transporte _transporte;
        private readonly Action<Transporte> _onDelete;

        public DeleteTransportWindow(Transporte transporte, Action<Transporte> onDelete)
        {
            InitializeComponent();
            _transporte = transporte ?? throw new ArgumentNullException(nameof(transporte));
            _onDelete = onDelete ?? throw new ArgumentNullException(nameof(onDelete));

            ConfirmButton.Click += ConfirmButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void ConfirmButton_Click(object? sender, RoutedEventArgs e)
        {
            _onDelete(_transporte);
            Close();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}