using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad1.ControlDeCuentasDeUsuario
{
    public static class GeneraCodigoHelper
    {
        public static string GetCodigo()
        {
            Random r = new Random();
            var codigo = r.Next(1000, 9999);
            return codigo.ToString();
        }
    }
}
