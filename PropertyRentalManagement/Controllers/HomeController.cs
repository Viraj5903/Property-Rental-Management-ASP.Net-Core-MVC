using Microsoft.AspNetCore.Mvc;
using PropertyRentalManagement.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace PropertyRentalManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Retrieve the ClaimsPrincipal (user information) from the current HTTP context.
            ClaimsPrincipal claimUser = HttpContext.User;

            // Check if the user is authenticated. 
            // The ?. operator ensures that it will not throw a NullReferenceException if claimUser is null.
            // It checks both claimUser and claimUser.Identity for null values before trying to access IsAuthenticated.
            if (claimUser?.Identity?.IsAuthenticated == true)
            {
                // If the user is authenticated, redirect them to the "Index" action of the "Home" controller.
                // return RedirectToAction("Index", "Home");

                // Get the user's role from claims
                string? userRole = claimUser.FindFirst(ClaimTypes.Role)?.Value;

                if (!string.IsNullOrEmpty(userRole))
                {
                    // Redirect user based on their role
                    switch (userRole)
                    {
                        case "Owner":
                            // Redirect to the Owner dashboard
                            return RedirectToAction("Owner", "Dashboard");

                        case "Manager":
                            // Redirect to the Manager dashboard
                            return RedirectToAction("Manager", "Dashboard");

                        case "Tenant":
                            // Redirect to the Tenant dashboard
                            return RedirectToAction("Tenant", "Dashboard");

                        default:
                            // Default to a generic dashboard or an error page if no role matches
                            return RedirectToAction("Index", "Home");
                    }

                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public new IActionResult NotFound()
        {
            return View("NotFound");
        }
    }
}
