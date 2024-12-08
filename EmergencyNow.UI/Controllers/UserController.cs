using EmergencyNow.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmergencyNow.UI.Controllers
{
    public class UserController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this._userManager = userManager; 
            this._roleManager = roleManager;

        }
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.Telefono,
                    Apellido1 = user.Apellido1,
                    Apellido2 = user.Apellido2,
                    Ubicacion = user.Ubicacion,
                    
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, user.AgregarRol);
                    ViewBag.Message = "Usuario creado con exito";
                }else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description); 
                    }
                }
                

            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = userRole.RoleName });

                if (result.Succeeded)
                {
                    ViewBag.Message = "Rol creado con exito";

                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }
            return View();
        }


    }

    
}
