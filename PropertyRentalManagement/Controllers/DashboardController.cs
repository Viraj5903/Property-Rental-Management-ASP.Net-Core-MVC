using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyRentalManagement.Models;
using System.Security.Claims;

namespace PropertyRentalManagement.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly PropertyRentalManagementDbContext _context;

        public DashboardController(PropertyRentalManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "Access");
        }

        [Authorize(Roles = "Owner")]
        public IActionResult Owner()
        {
            ViewData["NumberOfTenants"] = _context.Users.Where((u) => u.Role.Role == "Tenant").Count();
            ViewData["NumberOfManagers"] = _context.Users.Where((u) => u.Role.Role == "Manager").Count();
            ViewData["NumberOfEvents"] = _context.Events.Where((e) => e.Status.Description != "Solved").Count();

            return GetUserView("Owner");
        }

        [Authorize(Roles = "Tenant")]
        public IActionResult Tenant()
        {
            return GetUserView("Tenant");
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Manager()
        {
            return GetUserView("Manager");
        }

        /// <summary>
        /// Retrieves the user based on their role and returns the corresponding view.
        /// </summary>
        /// <param name="role">The role of the user (Owner, Tenant, Manager).</param>
        /// <returns>The view with the user details, or NotFound if the user does not exist.</returns>
        private IActionResult GetUserView(string role)
        {
            // Retrieve the ClaimsPrincipal (user information) from the current HTTP context.
            ClaimsPrincipal claimUser = HttpContext.User;

            // Check if the user is authenticated and has the expected role.
            if (claimUser?.Identity?.IsAuthenticated == true && claimUser.FindFirst(ClaimTypes.Role)?.Value == role)
            {
                int userId = Convert.ToInt32(claimUser.FindFirst("UserId")?.Value);
                User? user = _context.Users.Find(userId);

                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
            }

            return NotFound();
        }
    }
}
