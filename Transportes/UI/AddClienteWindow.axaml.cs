using System;
using System.Collections.Generic;
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
        public AddClienteWindow()
        {
            InitializeComponent();
            var btOk = this.GetControl<Button>("BtOk");
            var btCancel = this.GetControl<Button>("BtCancel");

            btOk.Click += (_, _) => this.OnExit();
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

        void OnExit()
        {
            this.Close();
        }

        public string Nif
        {
            get
            {
                var edNif = this.GetControl<TextBox>("EdNif");
                return edNif.Text!.Trim();
            }
        }

        public string Nombre
        {
            get
            {
                var edNom = this.GetControl<TextBox>("EdNom");
                return edNom.Text!.Trim();
            }
        }

        public string Email
        {
            get
            {
                var edEmail = this.GetControl<TextBox>("EdEmail");
                return edEmail.Text!.Trim();
            }
        }

        public string Telefono
        {
            get
            {
                var edTel = this.GetControl<TextBox>("EdTel");
                return edTel.Text!.Trim();
            }
        }

        public string DireccionPostal
        {
            get
            {
                var edDir = this.GetControl<TextBox>("EdDir");
                return edDir.Text!.Trim();
            }
        }
    }
}