using System;

namespace Transportes.Core;
public enum TipoTransporte
    {
        Mudanza,
        Mercancias,
        Vehiculos
    }

    public class Transporte
    {
        public string Id { get; private set; }
        public TipoTransporte Tipo { get; set; }
        public Cliente Cliente { get; set; }
        public Vehiculo Vehiculo { get; set; }
        public DateTime FechaContratacion { get; set; }
        public double KilometrosRecorridos { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaEntrega { get; set; }
        public double ImportePorDia { get; set; }
        public double ImportePorKilometro { get; set; }
        public double IvaAplicado { get; set; }

        
        public Transporte(TipoTransporte tipo, Cliente cliente, Vehiculo vehiculo, DateTime fechaContratacion, double kilometrosRecorridos, DateTime fechaSalida, DateTime fechaEntrega, double importePorDia, double importePorKilometro, double ivaAplicado)
        {
            Id = GenerarIdTransporte(vehiculo.Matricula, fechaContratacion);
            Tipo = tipo;
            Cliente = cliente;
            Vehiculo = vehiculo;
            FechaContratacion = fechaContratacion;
            KilometrosRecorridos = kilometrosRecorridos;
            FechaSalida = fechaSalida;
            FechaEntrega = fechaEntrega;
            ImportePorDia = importePorDia;
            ImportePorKilometro = importePorKilometro;
            IvaAplicado = ivaAplicado;
        }

        private string GenerarIdTransporte(string matricula, DateTime fechaContratacion)
        {
            string fecha = fechaContratacion.ToString("yyyyMMdd");
            return $"{matricula}{fecha}";
        }
    }