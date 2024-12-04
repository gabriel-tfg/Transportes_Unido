using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Transportes.Core;

namespace Transportes.UI
{
    public partial class ClienteDetailWindow : Window
    {
        public Cliente _cliente;
        public Cliente _clienteOriginal;
        public ClienteDetailWindow(Cliente cliente, ObservableCollection<Transporte> transportes)
        {
            InitializeComponent();
            _cliente = cliente;
            _clienteOriginal = new Cliente()  // Crear una copia del cliente original
            {
                Nif = cliente.Nif,
                Nombre = cliente.Nombre,
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                DireccionPostal = cliente.DireccionPostal
            };
            var btSave = this.GetControl<Button>("BtSave");
            var btCancel = this.GetControl<Button>("BtCancel");

            
            var ednif = this.GetControl<TextBox>("EdNif");
            var ednom = this.GetControl<TextBox>("EdNom");
            var edtel = this.GetControl<TextBox>("EdTel");
            var edemail = this.GetControl<TextBox>("EdEmail");
            var eddir = this.GetControl<TextBox>("EdDir");
            ednif.Text = cliente.Nif;
            ednom.Text = cliente.Nombre;
            edtel.Text = cliente.Telefono;
            edemail.Text = cliente.Email;
            eddir.Text = cliente.DireccionPostal;
            
            btSave.Click += (_, _) => this.OnSaveClicked(ednif.Text, ednom.Text, edtel.Text, edemail.Text, eddir.Text, transportes,_clienteOriginal);
            btCancel.Click += (_, _) => this.OnCancelClicked();
            this.IsCancelled = false;
        }

        public bool IsCancelled { get; private set; }

        void InitializeComponent()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AvaloniaXamlLoader.Load(this);

        }

        void OnCancelClicked()
        {
            this.IsCancelled = true;
            this.OnExit();
        }

        void OnSaveClicked(String nif, string nombre, string telefono, string email, string dir, ObservableCollection<Transporte> transportes, Cliente cliente)
        {
            _cliente.Nif = nif;
            _cliente.Nombre = nombre;
            _cliente.Telefono = telefono;
            _cliente.Email = email;
            _cliente.DireccionPostal = dir;

            //Sobreescribir los cambios de cliente en Transportes
            foreach (var transporte in transportes)
            {
                if (transporte.Cliente.Nif == cliente.Nif)
                {
                    transporte.Cliente = _cliente;
                }
            }
            
            this.IsCancelled = false;
            this.OnExit();
        }

        void OnExit()
        {
            this.Close();
        }
    }
}