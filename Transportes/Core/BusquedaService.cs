using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Transportes.Core;

public class BusquedaService
{
    public BusquedaService(ObservableCollection<Transporte> transportes, ObservableCollection<Vehiculo> flota,
        ObservableCollection<Cliente> clientes)
    {
        Transportes = transportes;
        Flota = flota;
        Clientes = clientes;
        
        foreach (var t in transportes)
        {
            Console.WriteLine("Check = "+t.Vehiculo.Matricula);
        }
    }

    public ObservableCollection<Transporte> Transportes { get; set; }
    public ObservableCollection<Vehiculo> Flota { get; set; }
    public ObservableCollection<Cliente> Clientes { get; set; }

    // 1. Transportes pendientes
    public ObservableCollection<Transporte> BuscarTransportesPendientes(bool porCamion, TipoVehiculo? tipoCamion)
    {
        return new ObservableCollection<Transporte>(
            Transportes
                .Where(t =>
                {
                    // Obtener la matrícula asociada al transporte
                    /*var matriculaVehiculo = Flota.FirstOrDefault(c =>
                        (c.Matricula).ToUpper() == t.Id.ToString().Substring(0, 7).ToUpper());
                        Console.WriteLine("Matricula = "+Flota[0].Matricula);
                        Console.WriteLine("Matricula2 = "+t.Id.ToString().Substring(0, 7).ToUpper());*/
                    
                    var matriculaVehiculo = t.Vehiculo;
                    
                    // Verificar si la matrícula existe y cumple con los criterios
                    if (matriculaVehiculo == null)
                        return false;

                    var tieneTransportesActivos = t.FechaEntrega >= DateTime.Today;

                    if (porCamion)
                        return tieneTransportesActivos && tipoCamion.HasValue && t.Vehiculo.Tipo == tipoCamion;
                    return tieneTransportesActivos &&
                           (!tipoCamion.HasValue || matriculaVehiculo.Tipo == tipoCamion.Value);
                })
        );
    }

    // 2. Disponibilidad
    public ObservableCollection<Vehiculo> BuscarDisponibilidad(TipoVehiculo? tipoCamion)
    {
        return new ObservableCollection<Vehiculo>(
            Flota
                .Where(c =>
                {
                    var tieneTransportesActivos = Transportes.Any(t =>
                        ConvertirMatricula(c.Matricula) == t.Id.ToString().Substring(0, 7).ToUpper() &&
                        t.FechaEntrega >= DateTime.Today);

                    return !tieneTransportesActivos && (!tipoCamion.HasValue || c.Tipo == tipoCamion.Value);
                })
        );
    }

    // 3. Reservas por cliente
    public ObservableCollection<Transporte> BuscarReservasPorCliente(Cliente cliente, int? anioFiltro)
    {
        return new ObservableCollection<Transporte>(
            Transportes
                .Where(t => t.Cliente?.Nif == cliente.Nif &&
                            (!anioFiltro.HasValue || t.FechaContratacion.Year == anioFiltro.Value))
        );
    }


// 4. Reservas por camión
    public ObservableCollection<Transporte> BuscarReservasPorCamion(TipoVehiculo? tipoCamion, int? anioFiltro)
    {
        return new ObservableCollection<Transporte>(
            Transportes
                .Where(t =>
                {
                    var matriculaTransporte = t.Id.ToString().Substring(0, 7).ToUpper();
                    var camion = Flota.FirstOrDefault(c => ConvertirMatricula(c.Matricula) == matriculaTransporte);

                    return camion != null &&
                           (!tipoCamion.HasValue || camion.Tipo == tipoCamion.Value) &&
                           (!anioFiltro.HasValue || t.FechaContratacion.Year == anioFiltro.Value);
                })
        );
    }

    // 5. Reservas para una persona
    public ObservableCollection<Transporte> BuscarReservasParaPersona(Cliente cliente, int? anioFiltro)
    {
        return new ObservableCollection<Transporte>(
            Transportes
                .Where(t => t.Cliente?.Nif == cliente.Nif && t.FechaEntrega >= DateTime.Today &&
                            (!anioFiltro.HasValue || t.FechaContratacion.Year == anioFiltro.Value))
        );
    }

    // 6. Ocupación
    public ObservableCollection<Vehiculo> BuscarOcupacion(DateTimeOffset? fechaEspecifica, bool porAnho)
    {
        var resultado = new ObservableCollection<Vehiculo>();
        foreach (var t in Transportes)
        {
            Console.WriteLine("Matricula en trans = " + t.Vehiculo.Matricula);
            if (porAnho)
            {
                Console.WriteLine("Matricula = " + t.Vehiculo.Matricula +"Entrega = " + t.FechaEntrega.Year + "; Anho = " + fechaEspecifica.Value.Year);
                if (t.FechaEntrega.Year == fechaEspecifica.Value.Year)
                {
                    if (!contiene(resultado,t.Vehiculo))
                    {
                        resultado.Add(t.Vehiculo);
                    }
                } 
            }
            else
            {
                if (t.FechaEntrega == fechaEspecifica.Value)
                {
                    if (!contiene(resultado,t.Vehiculo))
                    {
                        resultado.Add(t.Vehiculo);
                    }
                }
            }
        }

        foreach (var v in resultado)
        {
            Console.WriteLine("Matricula = " + v.Matricula);
        }
        return resultado;
    }

    private bool contiene(ObservableCollection<Vehiculo> vehiculos, Vehiculo vehiculo)
    {
        foreach (var v in vehiculos)
        {
            if (vehiculo.Matricula == v.Matricula)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private string ConvertirMatricula(string matricula)
    {
        var matriculaAux = matricula.Replace(" ", "");
        return matriculaAux.Substring(3, 4) + matriculaAux.Substring(0, 3);
    }
}