using System;
using System.Collections.Generic;

namespace Transportes.Core;

public enum TipoVehiculo {Furgoneta, Camion, CamionArticulado}


public class Vehiculo
{
    public string Matricula { get; set; }
    public TipoVehiculo Tipo { get; init; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public double ConsumoPorKm { get; set; }
    public DateTime FechaAdquisicion { get; set; }
    public DateTime FechaFabricacion { get; set; }
    public List<string> Comodidades { get; set; } = new List<string>();
    public bool Disponible { get; set; } = true;

    public Vehiculo()
    {
    }
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