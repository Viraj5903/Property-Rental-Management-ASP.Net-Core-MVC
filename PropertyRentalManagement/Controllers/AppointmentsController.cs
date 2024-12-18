using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    [Authorize(Roles = "Manager, Tenant")]
    public class AppointmentsController : Controller
    {
        private readonly PropertyRentalManagementDbContext _context;

        private readonly string appointmentStatusCategory = "Appointments";

        public AppointmentsController(PropertyRentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {

            int userId = 0;

            // Retrieve the ClaimsPrincipal (user information) from the current HTTP context.
            ClaimsPrincipal claimUser = HttpContext.User;

            // Check if the user is authenticated. 
            if (claimUser?.Identity?.IsAuthenticated == true)
            {
                // Safely get the UserId claim
                var userIdClaim = claimUser.FindFirst("UserId");

                // Check if the claim exists and is not null before converting
                if (userIdClaim != null)
                {
                    userId = Convert.ToInt32(userIdClaim.Value);
                }
            }

            var propertyRentalManagementDbContext = _context.Appointments.Include(a => a.Apartment).Include(a => a.Manager).Include(a => a.Status).Include(a => a.Tenant).Where(a => a.ManagerId == userId || a.TenantId == userId).OrderByDescending(a => a.AppointmentDateTime);
            return View(await propertyRentalManagementDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Apartment)
                .Include(a => a.Manager)
                .Include(a => a.Status)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            if (!IsUserAuthorized(appointment))
            {
                return Forbid(); // Return a 403 Forbidden response
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        [Authorize(Roles = "Tenant")]
        public IActionResult Create()
        {
            int apartmentID = 0;

            // Check if the apartmentId query parameter is present
            if (Request.Query.ContainsKey("apartmentId"))
            {
                // Try to parse the apartmentId from the query string
                if (!int.TryParse(Request.Query["apartmentId"], out apartmentID))
                {
                    // Handle invalid apartment ID (e.g., redirect or show a message)
                    return BadRequest("Invalid apartment ID.");
                }

                PopulateViewData(new Appointment { ApartmentId = apartmentID});
                return View();
            }

            PopulateViewData();
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Tenant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,TenantId,ManagerId,ApartmentId,AppointmentDateTime,Description,StatusId")] Appointment appointment)
        {

            ModelState.Remove("Status");
            ModelState.Remove("Tenant");
            ModelState.Remove("Manager");
            ModelState.Remove("Apartment");

            Apartment? apartment = await _context.Apartments.Include(a => a.Building).FirstOrDefaultAsync(a => a.ApartmentId == appointment.ApartmentId);

            if (apartment != null && apartment.Building.ManagerId.HasValue)
            {
                appointment.ManagerId = apartment.Building.ManagerId.Value;
            }

            if (GetCurrentUserId() != 0)
            {
                appointment.TenantId = GetCurrentUserId();
            }

            if (appointment.AppointmentDateTime <= DateTime.Now && appointment.AppointmentDateTime.Date == DateTime.Now.Date) 
            {
                ModelState.AddModelError("AppointmentDateTime", "Appointment Date Time must be not today or older.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            PopulateViewData(appointment);
            return View(appointment);
        }

        public async Task<IActionResult> Confirmed(int? id)
        {
            return await UpdateAppointmentStatus(id, 5); // 5 for Confirmed
        }

        public async Task<IActionResult> Canceled(int? id)
        {
            return await UpdateAppointmentStatus(id, 6); // 6 for Canceled
        }

        private async Task<IActionResult> UpdateAppointmentStatus(int? id, int statusId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.ManagerId != GetCurrentUserId())
            {
                return Forbid(); // Return 403 Forbidden response
            }

            try
            {
                appointment.StatusId = statusId;
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(appointment.AppointmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Re-throw the exception for unhandled cases
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }

        /// <summary>
        /// Populates the ViewData dictionary with select lists for the Appointment view.
        /// This method is used to fill dropdowns for Apartments, Managers, Statuses, and Tenants,
        /// optionally selecting values based on the provided appointment.
        /// </summary>
        /// <param name="appointment">An optional Appointment object used to pre-select values in the dropdowns.</param>
        private void PopulateViewData(Appointment? appointment = null)
        {
            // Populate the ApartmentId dropdown list using the ApartmentId as both value and display text.
            // The display text includes ApartmentId, ApartmentCode, and BuildingCode for clarity.
            // If an appointment is provided, pre-select its ApartmentId.
            ViewData["ApartmentId"] = new SelectList(
                _context.Apartments
                    .Include(a => a.Building) // Include related Building data for better display.
                    .Select(a => new
                    {
                        a.ApartmentId,
                        DisplayName = $"{a.ApartmentId} - {a.ApartmentCode} - {a.BuildingCode} {a.Building.Name}" // Display format.
                    }),
                "ApartmentId",
                "DisplayName",
                appointment?.ApartmentId); // Pre-select the ApartmentId if provided.

            // Populate the ManagerId dropdown list with users who have the role of Manager.
            // Each option displays the UserId along with the full name for clarity.
            // If an appointment is provided, pre-select its ManagerId.
            ViewData["ManagerId"] = new SelectList(
                _context.Users
                    .Where(u => u.Role.Role == "Manager")
                    .Select(u => new
                    {
                        u.UserId, // Value for the dropdown.
                        DisplayName = $"{u.UserId} - {u.FirstName} {u.LastName}" // Display UserId and full name.
                    }),
                "UserId",
                "DisplayName",
                appointment?.ManagerId); // Pre-select the ManagerId if provided.

            // Populate the StatusId dropdown list using the StatusId as the value 
            // and Description as the display text.
            // If an appointment is provided, pre-select its StatusId.
            ViewData["StatusId"] = new SelectList(
                _context.Statuses.Where(s => s.Category == appointmentStatusCategory),
                "StatusId",
                "Description",
                appointment?.StatusId); // Pre-select the StatusId if provided.

            // Populate the TenantId dropdown list with users who have the role of Tenant,
            // each option displays the UserId along with the full name for clarity.
            // If an appointment is provided, pre-select its TenantId.
            ViewData["TenantId"] = new SelectList(
                _context.Users
                    .Where(u => u.Role.Role == "Tenant")
                    .Select(u => new
                    {
                        u.UserId, // Value for the dropdown.
                        DisplayName = $"{u.UserId} - {u.FirstName} {u.LastName}" // Display UserId and full name.
                    }),
                "UserId",
                "DisplayName",
                appointment?.TenantId); // Pre-select the TenantId if provided.
        }

        /// <summary>
        /// Retrieves the current user's ID from the claims principal.
        /// This ID is used to identify the logged-in user in the system.
        /// </summary>
        /// <returns>
        /// The ID of the current user, or 0 if the user ID claim is not found.
        /// </returns>
        private int GetCurrentUserId()
        {
            // Attempt to find the "UserId" claim in the current user's claims.
            var userIdClaim = HttpContext.User.FindFirst("UserId");

            // Return the UserId as an integer, or 0 if the claim is not present.
            return userIdClaim != null ? Convert.ToInt32(userIdClaim.Value) : 0;
        }

        /// <summary>
        /// Checks if the current user is authorized to access a specific appointment.
        /// The user is considered authorized if they are either the Manager or the Tenant of the appointment.
        /// </summary>
        /// <param name="appointment">The appointment to check for authorization.</param>
        /// <returns>
        /// True if the current user is authorized (either as a Manager or Tenant); otherwise, false.
        /// </returns>
        private bool IsUserAuthorized(Appointment appointment)
        {
            // Get the current user's ID from claims.
            int userId = GetCurrentUserId();

            // Check if the current user ID matches either the ManagerId or TenantId of the appointment.
            return appointment.ManagerId == userId || appointment.TenantId == userId;
        }
    }
}
