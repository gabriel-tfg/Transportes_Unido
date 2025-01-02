using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Transportes.Core;

namespace Transportes.UI
{
    public partial class AddClienteWindow : Window
    {
        public AddClienteWindow(ObservableCollection<Cliente> clientes)
        {
            InitializeComponent();
            var btOk = this.GetControl<Button>("BtGuardar");
            var btCancel = this.GetControl<Button>("BtCancelar");

            btOk.Click += (_, _) => this.OnExit(clientes);
            btCancel.Click += (_, _) => this.OnCancelClicked();

            
            this.IsCancelled = false;
        }
        
        public bool IsCancelled { get;  set; }

        void InitializeComponent()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AvaloniaXamlLoader.Load(this);

        }

        void OnCancelClicked()
        {
            this.IsCancelled = true;
            this.Close();
        }

        void OnExit(ObservableCollection<Cliente> clientes)
        {
            if (string.IsNullOrWhiteSpace(Nif) || 
                string.IsNullOrWhiteSpace(Nombre) || 
                string.IsNullOrWhiteSpace(Telefono) || 
                string.IsNullOrWhiteSpace(Email) || 
                string.IsNullOrWhiteSpace(DireccionPostal))
            {
                this.IsCancelled = true;
                this.Close();
                return;
            }
            clientes.Add(new Cliente( Nif.ToUpper(),  Nombre,  Telefono,  Email,  DireccionPostal));
            this.Close();
        }

        public string Nif
        {
            get
            {
                var edNif = this.GetControl<TextBox>("EdNif");
                return edNif?.Text?.Trim() ?? string.Empty;          
            }
        }

        public string Nombre
        {
            get
            {
                var edNom = this.GetControl<TextBox>("EdNom");
                return edNom?.Text?.Trim()??string.Empty;
            }
        }

        public string Email
        {
            get
            {
                var edEmail = this.GetControl<TextBox>("EdEmail");
                return edEmail?.Text?.Trim()??string.Empty;
            }
        }

        public string Telefono
        {
            get
            {
                var edTel = this.GetControl<TextBox>("EdTel");
                return edTel?.Text?.Trim()??string.Empty;
            }
        }

        public string DireccionPostal
        {
            get
            {
                var edDir = this.GetControl<TextBox>("EdDir");
                return edDir?.Text?.Trim()??string.Empty;
            }
        }
    }
}