using Actividad1.ControlDeCuentasDeUsuario.Repositories;
using Actividad2RolesDeUsuario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad2RolesDeUsuario.Repositories
{
    public class AlumnoRepository : Repository<Alumno>
    {
        public AlumnoRepository(bdescuelaContext ctx) : base(ctx)
        {

        }


        public IEnumerable<Alumno> GetAllByClaveMaestro(int clave)
        {
            return base.GetAll().Where(x => x.ClaveMaestro == clave.ToString());
        }

        public override bool Validate(Alumno entidad)
        {
            if (Context.Alumno.Any(x => x.NumControl == entidad.NumControl && x.Id != entidad.Id))
            {
                throw new Exception("El numero de control asignado ya pertenece a otro alumno");
            }
            
            if (!Context.Maestro.Any(x => x.Clave.ToString() == entidad.ClaveMaestro))
            {
                throw new Exception("No existe el maestro especificado");
            }

            return true;

        }
    }
}
