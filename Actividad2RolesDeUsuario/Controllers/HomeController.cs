using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Actividad2RolesDeUsuario.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Actividad1.ControlDeCuentasDeUsuario.Repositories;
using Actividad2RolesDeUsuario.Models.ViewModels;
using Actividad2RolesDeUsuario.Repositories;
using System.Net.Mail;
using System.Net;

namespace Actividad2RolesDeUsuario.Controllers
{
    public class HomeController : Controller
    {
        public IWebHostEnvironment Environment { get; set; }
        bdescuelaContext context;
        public HomeController(bdescuelaContext ctx, IWebHostEnvironment env)
        {
            context = ctx;
            Environment = env;
        }


        //FUNCIONES DEL DIRECTOR
        //---------------------------------------------------
        [Authorize(Roles = "Director")]
        public IActionResult VerMaestros()
        {
            IndexViewModel vm = new IndexViewModel();
            MaestroRepository maestroRepository = new MaestroRepository(context);

            vm.ListaMaestros = maestroRepository.GetAll();

            return View(vm);
        }


        [Authorize(Roles = "Director")]
        public IActionResult ActivarMaestro(int id)
        {
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(id);
            return View(maestro);
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult ActivarMaestro(Maestro m)
        {
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(m.Id);
            if(maestro!=null)
            {
                maestro.Activo = 1;
                repos.Update(maestro);
            }
            return RedirectToAction("VerMaestros");
        }


        [Authorize(Roles = "Director")]
        public IActionResult DesactivarMaestro(int id)
        {
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(id);
            return View(maestro);
        }


        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult DesactivarMaestro(Maestro m)
        {
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(m.Id);
            if (maestro != null)
            {
                maestro.Activo = 0;
                repos.Update(maestro);
            }
            return RedirectToAction("VerMaestros");
        }


        [Authorize(Roles = "Director")]
        public IActionResult CambiarContrasena(int id)
        {
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(id);
            return View(maestro);
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult CambiarContrasena(Maestro m, string confirmaContrasena)
        {
            MaestroRepository repos = new MaestroRepository(context);
            var maestro = repos.GetById(m.Id);
            if (m.Password== confirmaContrasena)
            {               
                maestro.Password = HashingHelper.GetHash(m.Password);
                repos.Update(maestro);
            }
            else
            {
                ModelState.AddModelError("", "las contraseñas no coinciden");
                return View(m);
            }
            return RedirectToAction("VerMaestros");
        }

        //DAR DE ALTA MAESTROS
        [Authorize(Roles = "Director")]
        public IActionResult AgregarMaestro()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Director")]
        public IActionResult AgregarMaestro(Maestro nuevo, string confirmaContrasena)
        {
            try
            {
               if(nuevo.Password!=confirmaContrasena)
                {
                    ModelState.AddModelError("", "Las contraseñas no coinciden");
                    return View(nuevo);
                }
                MaestroRepository repo = new MaestroRepository(context);

                //MailMessage message = new MailMessage();
                //message.From = new MailAddress("sistemascomputacionales7g@gmail.com", "Cuenta automatizada de ITESRC");
                //message.Bcc.Add(nuevo.Email);
                //message.Subject = "Datos de la cuenta";
                //string text = System.IO.File.ReadAllText(Environment.WebRootPath + "/EnviarDatos.html");
                //message.Body = text.Replace("{##username##}", nuevo.Username).Replace("{##password##}", nuevo.Password);

                //message.IsBodyHtml = true;
                nuevo.Password = HashingHelper.GetHash(nuevo.Password);
                repo.Insert(nuevo);

                //SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                //client.EnableSsl = true;
                //client.UseDefaultCredentials = false;
                //client.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas171");
                //client.Send(message);
                return RedirectToAction("VerMaestros");
            }
            catch (Exception m)
            {  
                
                ModelState.AddModelError("", m.Message);
                return View(nuevo);
            }
            //MANDA CORREO AL MAESTRO CON EL NOBRE DE USUARIO Y CONTRASEÑA
           
        }         

        [Authorize(Roles = "Director,Maestro")]
        public IActionResult EditarMaestro(int id)
        {
            MaestroRepository maestroRepos = new MaestroRepository(context);
            var maestro = maestroRepos.GetById(id);
            if (maestro == null)
            {
                return RedirectToAction("VerMaestros");
            }
            return View(maestro);
        }

        [HttpPost]
        [Authorize(Roles = "Director,Maestro")]
        public IActionResult EditarMaestro(Maestro m)
        {
            //    PUEDE ACTIVAR y DESACTIVAR UN MAESTRO
            //PUEDE EDITAR LA INFO DEL MAESTRO Y SU CONTRASEÑA
            try
            {
                MaestroRepository maestroRepos = new MaestroRepository(context);
                var usuario = maestroRepos.GetById(m.Id);
               
                if(usuario!=null)
                {
                    usuario.Nombre = m.Nombre;
                    usuario.Username = m.Username;
                    usuario.Clave = m.Clave;
                    usuario.Email = m.Email;                 
                    maestroRepos.Update(usuario);
                }
                if (User.IsInRole("Director"))
                { return RedirectToAction("VerMaestros"); }
                
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(m);
            }
        }



        //FUNCIONES DE AMBOS
        //---------------------------------------------------

        [Authorize(Roles = "Director, Maestro")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Director, Maestro")]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("IniciarSesion");
        }

     
        [Authorize(Roles = "Director,Maestro")]
        public IActionResult AgregarAlumno(int clave)
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Director,Maestro")]
        public IActionResult AgregarAlumno(Alumno nuevo)
        {
            try
            {
                AlumnoRepository repo = new AlumnoRepository(context);

                repo.Insert(nuevo);
                var claveMaestro = nuevo.ClaveMaestro;
                return RedirectToAction("VerAlumnos", new { clave = claveMaestro });
            }
            catch (Exception m)
            {
                ModelState.AddModelError("", m.Message);
                return View(nuevo);
            }
        }


        [Authorize(Roles = "Director,Maestro")]
        public IActionResult EditarAlumno(int id)
        {
            AlumnoRepository repos = new AlumnoRepository(context);
            var alumno = repos.GetById(id);

            if(alumno==null)
            {
                return RedirectToAction("Index");
            }
            return View(alumno);
        }

        [HttpPost]
        [Authorize(Roles = "Director,Maestro")]
        public IActionResult EditarAlumno(Alumno a)
        {
            
            try
            {
                AlumnoRepository AluRepos = new AlumnoRepository(context);
                var usuario = AluRepos.GetById(a.Id);

                if (usuario != null)
                {
                    usuario.Nombre = a.Nombre;
                    usuario.NumControl = a.NumControl;
                   // usuario.ClaveMaestro = a.ClaveMaestro;
                
                    AluRepos.Update(usuario);
                     
                }
                var claveMaestro = usuario.ClaveMaestro;
                return RedirectToAction("VerAlumnos", new { clave = claveMaestro });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(a);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Director,Maestro")]
        public IActionResult EliminarAlumno(int id)
        {
               Repository<Alumno> alumnoRepos = new Repository<Alumno>(context);
                var alu = alumnoRepos.GetById(id);
            var claveMaestro = alu.ClaveMaestro;
                if (alu != null)
                {
                    alumnoRepos.Delete(alu);
                    
                }
           return RedirectToAction("VerAlumnos", new { clave = claveMaestro });

        }



        //FUNCIONES MAETSRO
        //solo se encarga de un grupo
        //solo puede ver y modificar el grupo que le pertenece
        //agrega, edita, elimina
        //solo pude iniciar y cerrar sesion
        //si no esta activo no puede acceder

        [Authorize(Roles = "Director,Maestro")]
        [Route ("/Home/VerAlumnos/{clave}")]
        public IActionResult VerAlumnos(int clave)
        {
            IndexViewModel vm = new IndexViewModel();
            AlumnoRepository AlumnoRepository = new AlumnoRepository(context);
            MaestroRepository maestroRepository = new MaestroRepository(context);
            vm.Maestro = maestroRepository.GetMaestroByClave(clave);
            vm.ListaAlumnos = AlumnoRepository.GetAllByClaveMaestro(clave);

            return View(vm);
        }
      

        //PARA INICIAR SESION
        //----------------------------------------------------

        [AllowAnonymous]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IniciarSesion(string usuario, string contrasena, bool mantenerSesion)
        {
            try
            {
                Repository<Director> reposDirector = new Repository<Director>(context);
               MaestroRepository reposMaestro = new MaestroRepository(context);



                if (usuario == "director")
                {
                    var user = reposDirector.GetById(1);


                    if (user.Username == usuario && user.Password == HashingHelper.GetHash(contrasena))
                    {

                        List<Claim> informacion = new List<Claim>();
                        informacion.Add(new Claim(ClaimTypes.Name, user.Username));
                        informacion.Add(new Claim(ClaimTypes.Role, "Director"));
                        informacion.Add(new Claim("Nombre", user.Username));

                        var claimidentity = new ClaimsIdentity(informacion, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimprincipal = new ClaimsPrincipal(claimidentity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimprincipal, new AuthenticationProperties { IsPersistent = mantenerSesion });
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        ModelState.AddModelError("", "El usuario o la contraseña son incorrectas");
                        return View();
                    }
                }
                else
                {
                    var userMaestro = reposMaestro.GetMaestroByUsername(usuario);


                    if (userMaestro.Username == usuario && userMaestro.Password == HashingHelper.GetHash(contrasena)&& userMaestro.Activo==1)
                    {

                        List<Claim> informacion = new List<Claim>();
                        informacion.Add(new Claim(ClaimTypes.Name, userMaestro.Username));
                        informacion.Add(new Claim(ClaimTypes.Role, "Maestro"));
                        informacion.Add(new Claim("Id", userMaestro.Id.ToString()));
                        informacion.Add(new Claim("Nombre", userMaestro.Nombre));
                        informacion.Add(new Claim("Clave", userMaestro.Clave.ToString()));

                        var claimidentity = new ClaimsIdentity(informacion, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimprincipal = new ClaimsPrincipal(claimidentity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimprincipal, new AuthenticationProperties { IsPersistent = mantenerSesion });
                        return RedirectToAction("Index");

                    }
                    else if (userMaestro.Activo == 0)
                    {
                        ModelState.AddModelError("", "El maestro no se encuentra activo");
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError("", "El usuario o la contraseña son incorrectas");
                        return View();
                    }
                }



            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult Denegado()
        {
            return View();
        }
       


    }
}
