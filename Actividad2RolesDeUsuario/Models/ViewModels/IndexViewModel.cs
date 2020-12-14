using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad2RolesDeUsuario.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Maestro> ListaMaestros { get; set; }

        public IEnumerable<Alumno> ListaAlumnos { get; set; }

        public Maestro Maestro { get; set; }
        public Alumno Alumno { get; set; }

    }
}
