namespace Transportes;

public class Cliente
{
    public string Nif { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
    public string DireccionPostal { get; set; }

    public Cliente(string nif, string nombre, string telefono, string email, string direccionPostal)
    {
        Nif = nif;
        Nombre = nombre;
        Telefono = telefono;
        Email = email;
        DireccionPostal = direccionPostal;
    }

    public Cliente()
    {
        
    }
}
