namespace Transportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public class XmlImporter
{
    public List<Cliente> CargarClientesXML(string archivoXml)
    {
        List<Cliente> clientes = new List<Cliente>();

        try
        {
            XDocument doc = XDocument.Load(archivoXml);
        
            var clientesXml = doc.Descendants("Cliente");

            foreach (var clienteXml in clientesXml)
            {
                string nif = clienteXml.Element("NIF")?.Value;
                string nombre = clienteXml.Element("Nombre")?.Value;
                string telefono = clienteXml.Element("Telefono")?.Value;
                string email = clienteXml.Element("Email")?.Value;
                string direccion = clienteXml.Element("Direccion")?.Value;

                if (string.IsNullOrEmpty(nif))
                {
                    Console.WriteLine("Error: Falta NIF del cliente.");
                    continue;
                }

                Cliente nuevoCliente = new Cliente
                {
                    Nif = nif,
                    Nombre = nombre,
                    Telefono = telefono,
                    Email = email,
                    DireccionPostal = direccion
                };

                clientes.Add(nuevoCliente);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar el archivo XML: {ex.Message}");
        }

        return clientes;
    }

public List<Transporte> CargarTransportesXML(string archivoXml)
{
    List<Transporte> transportes = new List<Transporte>();

    try
    {
        // Cargar el archivo XML
        XDocument doc = XDocument.Load(archivoXml);
        var transportesXml = doc.Descendants("Transporte");

        foreach (var transporteXml in transportesXml)
        {
            string idTransporte = transporteXml.Element("Id")?.Value ?? "0";
            string tipo1 = transporteXml.Element("Tipo")?.Value;
            TipoTransporte tipo = (TipoTransporte)Enum.Parse(typeof(TipoTransporte), tipo1);

            var clienteXml = transporteXml.Element("Cliente");
            string nif = clienteXml?.Element("NIF")?.Value;
            string nombre = clienteXml?.Element("Nombre")?.Value;
            string telefono = clienteXml?.Element("Telefono")?.Value;
            string email = clienteXml?.Element("Email")?.Value;
            string direccion = clienteXml?.Element("Direccion")?.Value;

            Cliente cliente = new Cliente
            {
                Nif = nif,
                Nombre = nombre,
                Telefono = telefono,
                Email = email,
                DireccionPostal = direccion
            };

            var vehiculoXml = transporteXml.Element("Vehiculo");
            Vehiculo vehiculo = new Vehiculo
            {
                Matricula = vehiculoXml?.Element("Matricula")?.Value,
                Tipo = (TipoVehiculo)Enum.Parse(typeof(TipoVehiculo), vehiculoXml?.Element("Tipo")?.Value ?? "Furgoneta"),
                Marca = vehiculoXml?.Element("Marca")?.Value,
                Modelo = vehiculoXml?.Element("Modelo")?.Value,
                ConsumoPorKm = double.Parse(vehiculoXml?.Element("ConsumoPorKm")?.Value ?? "0"),
                FechaAdquisicion = DateTime.Parse(vehiculoXml?.Element("FechaAdquisicion")?.Value ?? DateTime.MinValue.ToString()),
                FechaFabricacion = DateTime.Parse(vehiculoXml?.Element("FechaFabricacion")?.Value ?? DateTime.MinValue.ToString()),
                Disponible = bool.Parse(vehiculoXml?.Element("Disponible")?.Value ?? "true"),
                Comodidades = vehiculoXml?.Element("Comodidades")?.Elements("Comodidad")?.Select(c => c.Value).ToList() ?? new List<string>()
            };

            DateTime fechaContratacion = DateTime.Parse(transporteXml.Element("FechaContratacion")?.Value ?? DateTime.MinValue.ToString());
            int kilometrosRecorridos = int.Parse(transporteXml.Element("KilometrosRecorridos")?.Value ?? "0");
            DateTime fechaSalida = DateTime.Parse(transporteXml.Element("FechaSalida")?.Value ?? DateTime.MinValue.ToString());
            DateTime fechaEntrega = DateTime.Parse(transporteXml.Element("FechaEntrega")?.Value ?? DateTime.MinValue.ToString());
            double importePorDia = double.Parse(transporteXml.Element("ImportePorDia")?.Value ?? "0");
            double importePorKilometro = double.Parse(transporteXml.Element("ImportePorKilometro")?.Value ?? "0");
            double iva = double.Parse(transporteXml.Element("IVA")?.Value ?? "0");

            Transporte nuevoTransporte = new Transporte
            {
                Id = idTransporte,
                Tipo = tipo,
                Cliente = cliente,
                Vehiculo = vehiculo,
                FechaContratacion = fechaContratacion,
                KilometrosRecorridos = kilometrosRecorridos,
                FechaSalida = fechaSalida,
                FechaEntrega = fechaEntrega,
                ImportePorDia = importePorDia,
                ImportePorKilometro = importePorKilometro,
                IvaAplicado = iva
            };

            transportes.Add(nuevoTransporte);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al cargar el archivo XML: {ex.Message}");
    }

    return transportes;
}

    public List<Vehiculo> CargarFlotaXML(string archivoXml)
    {
        List<Vehiculo> flota = new List<Vehiculo>();

        try
        {
            // Cargar el archivo XML
            XDocument doc = XDocument.Load(archivoXml);
            var vehiculosXml = doc.Descendants("Vehiculo");

            foreach (var vehiculoXml in vehiculosXml)
            {
                string matricula = vehiculoXml.Element("Matricula")?.Value;
                string tipo1 = vehiculoXml.Element("Tipo")?.Value;
                TipoVehiculo tipo = (TipoVehiculo)Enum.Parse(typeof(TipoVehiculo), tipo1);
                string marca = vehiculoXml.Element("Marca")?.Value;
                string modelo = vehiculoXml.Element("Modelo")?.Value;
                double consumoPorKm = double.Parse(vehiculoXml.Element("ConsumoPorKm")?.Value ?? "0");
                // Obtener las fechas asegurándonos de que no son nulas o vacías
                DateTime fechaDeAdquisicion = ObtenerFecha(vehiculoXml.Element("FechaAdquisicion")?.Value);
                DateTime fechaDeFabricacion = ObtenerFecha(vehiculoXml.Element("FechaFabricacion")?.Value);


                var comodidadesXml = vehiculoXml.Element("Comodidades")?.Elements("Comodidad");
                List<string> comodidades = comodidadesXml?.Select(c => c.Value).ToList() ?? new List<string>();
                Console.WriteLine($"Vehículo: {matricula}, Comodidades: {string.Join(", ", comodidades)}");

               
                Vehiculo nuevoVehiculo = new Vehiculo
                {
                    Matricula = matricula,
                    Tipo = tipo,
                    Marca = marca,
                    Modelo = modelo,
                    ConsumoPorKm = consumoPorKm,
                    FechaAdquisicion = fechaDeAdquisicion,
                    FechaFabricacion = fechaDeFabricacion,
                    Comodidades = comodidades
                };
                Console.WriteLine(matricula);
                flota.Add(nuevoVehiculo);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar el archivo XML: {ex.Message}");
        }

        return flota;
    }

    private DateTime ObtenerFecha(string fechaStr)
    {
        if (!string.IsNullOrEmpty(fechaStr))
        {
            // Intentar parsear con múltiples formatos
            var formatos = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "yyyy-MM-ddTHH:mm:ss" };
            if (DateTime.TryParseExact(fechaStr, formatos, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var fecha))
            {
                return fecha;
            }
        }

        // Si no se puede parsear, se devuelve la fecha actual como ejemplo
        return DateTime.Now;
    }


}