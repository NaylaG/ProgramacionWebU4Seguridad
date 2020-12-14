using Actividad1.ControlDeCuentasDeUsuario.Repositories;
using Actividad2RolesDeUsuario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad2RolesDeUsuario.Repositories
{
    public class AlumnoRepository: Repository<Alumno>
    {
        public AlumnoRepository(bdescuelaContext ctx) : base(ctx)
        {

        }


        public  IEnumerable<Alumno> GetAllByClaveMaestro(int clave)
        {
            return base.GetAll().Where(x => x.ClaveMaestro == clave.ToString());
        }
    }
}
