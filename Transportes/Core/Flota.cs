namespace Transportes.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Flota
    {
        public Flota()
        {
            vehiculos = new List<Vehiculo>();
        }
        
        public void Add(Vehiculo v)
        {
            vehiculos.Add(v);
        }
        
        public void ModificarVehiculo(int pos, string matricula, TipoVehiculo tipo, string marca, string modelo, double consumoPorKm,
                                      DateTime fechaAdquisicion, DateTime fechaFabricacion, List<string> comodidades)
        {
            if (pos >= 0 && pos < vehiculos.Count)
            {
                vehiculos[pos] = new Vehiculo(matricula, tipo, marca, modelo, consumoPorKm, fechaAdquisicion, fechaFabricacion, comodidades);
            }
            else
            {
                Console.WriteLine("Posición no válida.");
            }
        }

        public void AltaVehiculo(string matricula, TipoVehiculo tipo, string marca, string modelo, double consumoPorKm,
                                 DateTime fechaAdquisicion, DateTime fechaFabricacion, List<string> comodidades)
        {
            Vehiculo v = new Vehiculo(matricula, tipo, marca, modelo, consumoPorKm, fechaAdquisicion, fechaFabricacion, comodidades);
            vehiculos.Add(v);
        }

        public void BajaVehiculo(int pos)
        {
            if (pos >= 0 && pos < vehiculos.Count)
            {
                vehiculos.RemoveAt(pos);
            }
            else
            {
                Console.WriteLine("Posición no válida.");
            }
        }

        public int PosicionPorMatricula(string matricula)
        {
            for (int i = 0; i < vehiculos.Count; i++)
            {
                if (vehiculos[i].Matricula == matricula)
                {
                    return i;
                }
            }
            return -1;
        }

        public void LimpiarFlota()
        {
            vehiculos.Clear();
        }

        public Vehiculo ObtenerVehiculo(int pos)
        {
            if (pos >= 0 && pos < vehiculos.Count)
            {
                return vehiculos[pos];
            }
            else
            {
                throw new IndexOutOfRangeException("Posición no válida.");
            }
        }

        public int ContarVehiculos()
        {
            return vehiculos.Count;
        }

        public bool MatriculaOk(string matricula)
        {
            string pattern = @"^[A-Z]{3}\d{4}$";
            return System.Text.RegularExpressions.Regex.IsMatch(matricula, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        public bool MatriculaUnica(string matricula)
        {
            var matriculasRegistradas = vehiculos.Select(v => v.Matricula).ToList();
            return !matriculasRegistradas.Contains(matricula);
        }

        private List<Vehiculo> vehiculos;
    }


}
