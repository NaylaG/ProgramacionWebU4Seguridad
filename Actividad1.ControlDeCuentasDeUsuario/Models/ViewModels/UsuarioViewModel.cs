using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad1.ControlDeCuentasDeUsuario.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public Usuario Usuario { get; set; }
        public string NuevaContraseña { get; set; }
        public string ConfirmarContraseña { get; set; }
    }
}
