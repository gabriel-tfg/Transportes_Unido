using System;
using System.Collections.Generic;

namespace Transportes.Core;

public enum TipoVehiculo
{
    Furgoneta,
    Camion
}

public class Vehiculo
{
    public string Matricula { get; }
    public TipoVehiculo Tipo { get; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public double ConsumoPorKm { get; set; }
    public DateTime FechaAdquisicion { get; set; }
    public DateTime FechaFabricacion { get; set; }
    public List<string> Comodidades { get; } = new List<string>();
    public bool Disponible { get; set; } = true;

    public Vehiculo(string matricula, TipoVehiculo tipo, string marca, string modelo, double consumoPorKm,
        DateTime fechaAdquisicion, DateTime fechaFabricacion)
    {
        Matricula = matricula;
        Tipo = tipo;
        Marca = marca;
        Modelo = modelo;
        ConsumoPorKm = consumoPorKm;
        FechaAdquisicion = fechaAdquisicion;
        FechaFabricacion = fechaFabricacion;
    }
}