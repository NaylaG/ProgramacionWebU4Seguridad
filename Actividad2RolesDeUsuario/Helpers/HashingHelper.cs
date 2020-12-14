using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Actividad2RolesDeUsuario
{
    public static class HashingHelper
    {
        public static string GetHash(string cadena)
        {
            var alg = SHA256.Create();
            byte[] codificar = System.Text.Encoding.UTF8.GetBytes(cadena+"SistemaITESRC");
            byte[] hash = alg.ComputeHash(codificar);

            string res = "";
            foreach (var item in hash)
            {
                res += item.ToString("X2");
            }
            return res;

        }
    }
}
