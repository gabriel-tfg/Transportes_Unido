namespace Transportes.Core;

public class Factura
{
    public Transporte Transporte { get; }
    public int NumeroDias => (Transporte.FechaEntrega - Transporte.FechaSalida).Days + 1;
    public double PrecioCombustible { get; set; }
    public double SueldoporHora { get; set; }

    public Factura(Transporte transporte, double precioCombustible, double sueldoporHora)
    {
        Transporte = transporte;
        PrecioCombustible = precioCombustible;
        SueldoporHora = sueldoporHora;
    }
}

