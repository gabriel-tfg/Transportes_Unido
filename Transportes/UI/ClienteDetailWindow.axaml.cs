using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Transportes.Core;

namespace Transportes.UI
{
    public partial class ClienteDetailWindow : Window
    {
        public Cliente _cliente;
        public event Action<Cliente>? ClienteBorrado; //evento cuando se quiera editar cliente
        public ClienteDetailWindow(Cliente cliente, ObservableCollection<Cliente> clientes)
        {
            InitializeComponent();

            _cliente = new Cliente()
            {
                Nif = cliente.Nif, Nombre = cliente.Nombre, DireccionPostal = cliente.DireccionPostal,
                Telefono = cliente.Telefono, Email = cliente.Email
            };
            DataContext = _cliente;
            /*_clienteOriginal = new Cliente()  // Crear una copia del cliente original
            {
                Nif = cliente.Nif,
                Nombre = cliente.Nombre,
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                DireccionPostal = cliente.DireccionPostal
            };*/
            var btSave = this.GetControl<Button>("BtSave");
            var btBorrar = this.GetControl<Button>("BtBorrar");
            
            btSave.Click += (_, _) => this.OnSaveClicked(clientes);
            btBorrar.Click += (_, _) => this.Delete_Click();
            this.IsCancelled = true;
        }

        public bool IsCancelled { get; private set; }

        void InitializeComponent()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AvaloniaXamlLoader.Load(this);

        }
        
        private bool _isEditing=false;
        void OnSaveClicked(ObservableCollection<Cliente> clientes)
        {
           
            
            
            var bEditar = this.FindControl<Button>("BtSave");
            if (_isEditing)
            {
                // Guardar los cambios
                Console.WriteLine("Guardando cambios...");
                bEditar.Content = "Editar";

                for (int i = 0; i < clientes.Count; i++)
                {
                    if (clientes[i].Nif == _cliente.Nif)
                    {
                        clientes[i] = _cliente;
                    }
                }
                this.Close();
            }
            else
            {
                bEditar.Content = "Guardar";
            }
        
            _isEditing = !_isEditing;
            this.GetControl<TextBox>("EdNom").IsEnabled = _isEditing;
            this.GetControl<TextBox>("EdTel").IsEnabled = _isEditing;
            this.GetControl<TextBox>("EdEmail").IsEnabled = _isEditing; 
            this.GetControl<TextBox>("EdDir").IsEnabled = _isEditing;
            
        }

        void Delete_Click()
        {
            var dialog = new Window
            {
                Title = "Confirmación",
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };

            // Mensaje de confirmación
            var message = new TextBlock
            {
                Text = "¿Está seguro de que desea eliminar?",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Botones
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Spacing = 10
            };

            var yesButton = new Button
            {
                Content = "Sí",
                Width = 75,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };

            yesButton.Click += (sender, e) =>
            {
                ClienteBorrado?.Invoke(_cliente);
                this.IsCancelled = true;
                dialog.Close();
                this.Close();
            };

            var noButton = new Button
            {
                Content = "No",
                Width = 75,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };

            noButton.Click += (sender, e) =>
            {
                dialog.Close();
            };

            buttonPanel.Children.Add(yesButton);
            buttonPanel.Children.Add(noButton);

            stackPanel.Children.Add(message);
            stackPanel.Children.Add(buttonPanel);

            dialog.Content = stackPanel;

            dialog.ShowDialog(this);
        }

        void OnExit()
        {
            this.Close();
        }
    }
}