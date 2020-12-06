using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProgramacionWebU4Seguridad.Controllers
{
   // [Authorize(Roles = "Alumno")]//lo ponemos aqui para que no tenga acceso los usuarios anomimos a las vistas que contiene este controller
    public class HomeController : Controller
    {
        [Authorize(Roles = "Alumno, Maestro")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Maestro")]
        public IActionResult Vista2()
        {
            return View();
        }

        [AllowAnonymous] //permite el acceso a los usuarios anonimos
        public IActionResult IniciarSesion()
        {
            //HttpContext.User.Identity.Name
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IniciarSesion(string username, string password)
        {
            if((username=="hector"|| username=="Nayla"|| username=="Alejandra") && password=="2020")
            {
                List<Claim> informacion = new List<Claim>();
                informacion.Add(new Claim(ClaimTypes.Name, "Alumno del ITESRC: " + username));
                if(username=="hector")
                {
                    informacion.Add(new Claim(ClaimTypes.Role, "Maestro"));
                }
                else
                {
                    informacion.Add(new Claim(ClaimTypes.Role, "Alumno"));//tiene que ir el rol para poder autenticar
                 
                }
                informacion.Add(new Claim("NombreCompleto", username));


                //guardar los claimns para saber los datos

                var claimidentity = new ClaimsIdentity(informacion, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimprincipal = new ClaimsPrincipal(claimidentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimprincipal, new AuthenticationProperties { IsPersistent=true});// lo de persisiten es para que la sesion se mantenga apesar de que se cierre el navegador
                return RedirectToAction("Index");

            }
            else
            {
                ModelState.AddModelError("", "El usuario o la contrasena son incorrectas");
                return View();
            }
            
        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }
    }
}
