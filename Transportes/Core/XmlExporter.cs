﻿using System.Collections.Generic;
using System.Xml.Linq;
namespace Transportes.Core;

public class XmlExporter
{
    public void ExportarClientesXML(List<Cliente> _clientes, string rutaArchivo)
    {
        var raiz = new XElement("Clientes");

        foreach (var cliente in _clientes)
        {
            var elementoCliente = new XElement("Cliente",
                new XElement("NIF", cliente.Nif),
                new XElement("Nombre", cliente.Nombre),
                new XElement("Telefono", cliente.Telefono),
                new XElement("Email", cliente.Email),
                new XElement("Direccion", cliente.DireccionPostal)
            );
            raiz.Add(elementoCliente);
        }

        raiz.Save(rutaArchivo);
    }
    
    public void ExportarFlotaXML(List<Vehiculo> _flota, string rutaArchivo)
    {
        var raiz = new XElement("Vehiculos");

        foreach (var vehiculo in _flota)
        {
            var elementoVehiculo = new XElement("Vehiculo",
                new XElement("Matricula", vehiculo.Matricula),
                new XElement("Tipo", vehiculo.Tipo),
                new XElement("Marca", vehiculo.Marca),
                new XElement("Modelo", vehiculo.Modelo),
                new XElement("ConsumoPorKm", vehiculo.ConsumoPorKm),
                new XElement("FechaAdquisicion", vehiculo.FechaAdquisicion.ToString("yyyy-MM-dd")),
                new XElement("FechaFabricacion", vehiculo.FechaFabricacion.ToString("yyyy-MM-dd"))
            );

            var comodidades = new XElement("Comodidades");
            foreach (var comodidad in vehiculo.Comodidades)
            {
                comodidades.Add(new XElement("Comodidad", comodidad));
            }
            elementoVehiculo.Add(comodidades);

            raiz.Add(elementoVehiculo);
        }

        raiz.Save(rutaArchivo);
    }

    public void ExportarTransportesXML(List<Transporte> transportes, string rutaArchivo)
    {
        var raiz = new XElement("Transportes");

        foreach (var transporte in transportes)
        {
            var elementoTransporte = new XElement("Transporte",
                new XElement("Id", transporte.Id),
                new XElement("Tipo", transporte.Tipo),
                new XElement("FechaContratacion", transporte.FechaContratacion.ToString("yyyy-MM-dd")),
                new XElement("KilometrosRecorridos", transporte.KilometrosRecorridos),
                new XElement("FechaSalida", transporte.FechaSalida.ToString("yyyy-MM-dd")),
                new XElement("FechaEntrega", transporte.FechaEntrega.ToString("yyyy-MM-dd")),
                new XElement("ImportePorDia", transporte.ImportePorDia),
                new XElement("ImportePorKilometro", transporte.ImportePorKilometro),
                new XElement("IVA", transporte.IvaAplicado)
            );

            var clienteElemento = new XElement("Cliente",
                new XElement("NIF", transporte.Cliente.Nif),
                new XElement("Nombre", transporte.Cliente.Nombre),
                new XElement("Telefono", transporte.Cliente.Telefono),
                new XElement("Email", transporte.Cliente.Email),
                new XElement("Direccion", transporte.Cliente.DireccionPostal)
            );
            elementoTransporte.Add(clienteElemento);

            raiz.Add(elementoTransporte);
        }

        raiz.Save(rutaArchivo);
    } 
}