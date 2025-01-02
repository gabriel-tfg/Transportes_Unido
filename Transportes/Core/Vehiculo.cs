using System;
using System.Collections.Generic;

namespace Transportes.Core;

public enum TipoVehiculo {Furgoneta, Camion, CamionArticulado}


public class Vehiculo
{
    public string Matricula { get; set; }
    public TipoVehiculo Tipo { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public double ConsumoPorKm { get; set; }
    public DateTime FechaAdquisicion { get; set; } = DateTime.MinValue;
    public DateTime FechaFabricacion { get; set; } = DateTime.MinValue;
    public List<string> Comodidades { get; set; } = new List<string>();
    public bool Disponible { get; set; } = true;
    
    public DateTimeOffset? FechaAdquisicionOffset
    {
        get => FechaAdquisicion == DateTime.MinValue ? null : new DateTimeOffset(FechaAdquisicion);
        set => FechaAdquisicion = value?.DateTime ?? DateTime.MinValue;
    }

    public DateTimeOffset? FechaFabricacionOffset
    {
        get => FechaFabricacion == DateTime.MinValue ? null : new DateTimeOffset(FechaFabricacion);
        set => FechaFabricacion = value?.DateTime ?? DateTime.MinValue;
    }
    
    public bool HasWifi
    {
        get => Comodidades.Contains("Wifi");
        set => UpdateComodidad("Wifi", value);
    }

    public bool HasBluetooth
    {
        get => Comodidades.Contains("Conexión del móvil por Bluetooth");
        set => UpdateComodidad("Conexión del móvil por Bluetooth", value);
    }

    public bool HasAireAcondicionado
    {
        get => Comodidades.Contains("Aire Acondicionado");
        set => UpdateComodidad("Aire Acondicionado", value);
    }

    public bool HasLitera
    {
        get => Comodidades.Contains("Litera de Descanso");
        set => UpdateComodidad("Litera de Descanso", value);
    }

    public bool HasTV
    {
        get => Comodidades.Contains("TV");
        set => UpdateComodidad("TV", value);
    }

    private void UpdateComodidad(string comodidad, bool agregar)
    {
        if (agregar && !Comodidades.Contains(comodidad))
            Comodidades.Add(comodidad);
        else if (!agregar)
            Comodidades.Remove(comodidad);
    }
    
    public Vehiculo()
    {
    }
    public Vehiculo(string matricula, TipoVehiculo tipo, string marca, string modelo, double consumoPorKm,
        DateTime fechaAdquisicion, DateTime fechaFabricacion, List<string> comodidades)
    {
        Matricula = matricula;
        Tipo = tipo;
        Marca = marca;
        Modelo = modelo;
        ConsumoPorKm = consumoPorKm;
        FechaAdquisicion = fechaAdquisicion;
        FechaFabricacion = fechaFabricacion;
        Comodidades = comodidades;
    }
}