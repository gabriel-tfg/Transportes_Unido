using System.Collections.ObjectModel;

namespace Transportes.Core;

using System;
using System.Collections.Generic;
using System.Linq;

public class BusquedaService
{
    public ObservableCollection<Transporte> Transportes { get; set; }
    public ObservableCollection<Vehiculo> Flota { get; set; }
    public ObservableCollection<Cliente> Clientes { get; set; }

    public BusquedaService(ObservableCollection<Transporte> transportes, ObservableCollection<Vehiculo> flota, ObservableCollection<Cliente> clientes)
    {
        Transportes = transportes;
        Flota = flota;
        Clientes = clientes;
    }

    // 1. Transportes pendientes
    public ObservableCollection<string> BuscarTransportesPendientes(bool porCamion, TipoVehiculo? tipoCamion = null)
    {
        DateTime fechaLimite = DateTime.Today.AddDays(5);

        return new ObservableCollection<string>(
            Transportes
                .Where(t =>
                {
                    string matriculaTransporte = t.Id.ToString().Substring(0, 7).ToUpper();
                    var camion = Flota.FirstOrDefault(c => ConvertirMatricula(c.Matricula) == matriculaTransporte);

                    bool transporteValido = t.FechaSalida >= DateTime.Today && t.FechaSalida <= fechaLimite;

                    if (porCamion)
                    {
                        transporteValido &= camion != null && (!tipoCamion.HasValue || camion.Tipo == tipoCamion.Value);
                    }

                    return transporteValido;
                })
                .Select(t =>
                {
                    string matricula = t.Id.ToString().Substring(0, 7);
                    return $"ID Transporte: {t.Id}, Fecha Salida: {t.FechaSalida.ToShortDateString()}, Matrícula: {matricula}";
                })
        );
    }

    // 2. Disponibilidad
    public ObservableCollection<string> BuscarDisponibilidad(TipoVehiculo? tipoCamion)
    {
        return new ObservableCollection<string>(
            Flota
                .Where(c =>
                {
                    bool tieneTransportesActivos = Transportes.Any(t =>
                        ConvertirMatricula(c.Matricula) == t.Id.ToString().Substring(0, 7).ToUpper() &&
                        t.FechaEntrega >= DateTime.Today);

                    return !tieneTransportesActivos && (!tipoCamion.HasValue || c.Tipo == tipoCamion.Value);
                })
                .Select(c =>
                {
                    var transporteMasReciente = Transportes
                        .Where(t => ConvertirMatricula(c.Matricula) == t.Id.ToString().Substring(0, 7).ToUpper())
                        .OrderByDescending(t => t.FechaEntrega)
                        .FirstOrDefault();

                    string fechaEntrega = transporteMasReciente?.FechaEntrega.ToShortDateString() ?? "Sin entregas";
                    return $"Matrícula: {c.Matricula}, Tipo: {c.Tipo}, Marca: {c.Marca}, Última Fecha de Entrega: {fechaEntrega}";
                })
        );
    }

    // 3. Reservas por cliente
    public ObservableCollection<string> BuscarReservasPorCliente(Cliente cliente, int? anioFiltro)
    {
        return new ObservableCollection<string>(
            Transportes
                .Where(t => t.Cliente?.Nif == cliente.Nif && (!anioFiltro.HasValue || t.FechaContratacion.Year == anioFiltro.Value))
                .Select(t =>
                    $"Cliente: {cliente.Nombre}, Tipo Transporte: {t.Tipo}, Fecha Contratación: {t.FechaContratacion.ToShortDateString()}, Fecha Salida: {t.FechaSalida.ToShortDateString()}, Fecha Entrega: {t.FechaEntrega.ToShortDateString()}")
        );
    }

    // 4. Reservas por camión
    public ObservableCollection<string> BuscarReservasPorCamion(TipoVehiculo? tipoCamion, int? anioFiltro)
    {
        return new ObservableCollection<string>(
            Transportes
                .Where(t =>
                {
                    string matriculaTransporte = t.Id.ToString().Substring(0, 7).ToUpper();
                    var camion = Flota.FirstOrDefault(c => ConvertirMatricula(c.Matricula) == matriculaTransporte);

                    return camion != null &&
                           (!tipoCamion.HasValue || camion.Tipo == tipoCamion.Value) &&
                           (!anioFiltro.HasValue || t.FechaContratacion.Year == anioFiltro.Value);
                })
                .Select(t =>
                {
                    string matriculaTransporte = t.Id.ToString().Substring(0, 7).ToUpper();
                    var camion = Flota.FirstOrDefault(c => ConvertirMatricula(c.Matricula) == matriculaTransporte);

                    return $"Matrícula Camión: {matriculaTransporte}, Tipo: {camion?.Tipo}, Marca: {camion?.Marca}, Fecha Contratación: {t.FechaContratacion.ToShortDateString()}, Fecha Salida: {t.FechaSalida.ToShortDateString()}, Fecha Entrega: {t.FechaEntrega.ToShortDateString()}";
                })
        );
    }

    // 5. Reservas para una persona
    public ObservableCollection<string> BuscarReservasParaPersona(Cliente cliente, int? anioFiltro)
    {
        return new ObservableCollection<string>(
            Transportes
                .Where(t => t.Cliente?.Nif == cliente.Nif && t.FechaContratacion >= DateTime.Today && (!anioFiltro.HasValue || t.FechaContratacion.Year == anioFiltro.Value))
                .Select(t => $"Matrícula Camión: {t.Id.Substring(0, 7)}, Fecha Contratación: {t.FechaContratacion.ToShortDateString()}, Fecha Salida: {t.FechaSalida.ToShortDateString()}")
        );
    }

    // 6. Ocupación
    public ObservableCollection<string> BuscarOcupacion(DateTime? fechaEspecifica, int? anioFiltro)
    {
        var transportesRealizados = Transportes
            .Where(t =>
                fechaEspecifica.HasValue
                    ? t.FechaSalida.Date == fechaEspecifica.Value.Date
                    : anioFiltro.HasValue && t.FechaSalida.Year == anioFiltro.Value)
            .Select(t => t.Id.ToString().Substring(0, 7).ToUpper())
            .Distinct();

        return new ObservableCollection<string>(
            Flota
                .Where(c => transportesRealizados.Contains(ConvertirMatricula(c.Matricula)))
                .Select(c => $"Matrícula: {c.Matricula}, Tipo: {c.Tipo}, Marca: {c.Marca}")
        );
    }

    private string ConvertirMatricula(string matricula)
    {
        string matriculaAux = matricula.Replace(" ", "");
        return matriculaAux.Substring(3, 4) + matriculaAux.Substring(0, 3);
    }
}