using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    [Authorize(Roles = "Owner")]
    public class TenantsController : Controller
    {
        private readonly PropertyRentalManagementDbContext _context;

        // Create a dictionary for the search options
        private readonly Dictionary<string, string> searchOptions = new Dictionary<string, string>
        {
            { "UserId", "User ID" },
            { "FirstName", "First Name" },
            { "LastName", "Last Name" },
            { "Email", "Email" },
            { "Username", "Username" },
            { "PhoneNumber", "Phone Number" }
        };

        public TenantsController(PropertyRentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Tenants
        public async Task<IActionResult> Index()
        {
            var propertyRentalManagementDbContext = _context.Users.Include(u => u.Role).Where((u) => u.Role.Role == "Tenant");
            ViewData["SearchByList"] = new SelectList(searchOptions, "Key", "Value");
            return View(await propertyRentalManagementDbContext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string searchTerm, string searchBy, string strictEquality)
        {
            // Initialize a query to fetch all users with the role of "Tenant"
            var propertyRentalManagementDbContext = _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.Role == "Tenant");

            // Convert the strictEquality parameter from string to bool
            bool isStrictEquality = !string.IsNullOrEmpty(strictEquality) && strictEquality.Equals("on", StringComparison.OrdinalIgnoreCase);

            // Check if both search term and search by fields are provided
            if (string.IsNullOrEmpty(searchTerm) || string.IsNullOrEmpty(searchBy))
            {
                // Add a validation message to ViewBag
                ViewBag.ErrorMessage = "Please provide both Search By and Search Term.";
                ViewBag.SearchByList = new SelectList(searchOptions, "Key", "Value");
                return View(await propertyRentalManagementDbContext.ToListAsync());
            }

            // Validate UserId if searchBy is UserId
            if (searchBy == "UserId" && (!int.TryParse(searchTerm, out int userId) || userId <= 0))
            {
                ViewBag.ErrorMessage = "User ID must be a positive integer.";
                ViewBag.SearchByList = new SelectList(searchOptions, "Key", "Value");
                return View(await propertyRentalManagementDbContext.ToListAsync());
            }

            // Create a dictionary that maps each search criteria to a filtering expression
            Dictionary<string, Func<string, bool, IQueryable<User>>> searchExpressions = new Dictionary<string, Func<string, bool, IQueryable<User>>>
            {
                { "UserId", (term, strict) => propertyRentalManagementDbContext.Where(u => strict ? u.UserId.ToString() == term : u.UserId.ToString().Contains(term)) },
                { "FirstName", (term, strict) => propertyRentalManagementDbContext.Where(u => strict ? u.FirstName == term : u.FirstName.Contains(term)) },
                { "LastName", (term, strict) => propertyRentalManagementDbContext.Where(u => strict ? u.LastName == term : u.LastName.Contains(term)) },
                { "Email", (term, strict) => propertyRentalManagementDbContext.Where(u => strict ? u.Email == term : u.Email.Contains(term)) },
                { "Username", (term, strict) => propertyRentalManagementDbContext.Where(u => strict ? u.Username == term : u.Username.Contains(term)) },
                { "PhoneNumber", (term, strict) => propertyRentalManagementDbContext.Where(u => strict ? u.PhoneNumber == term : u.PhoneNumber.Contains(term)) }
            };

            // Check if the selected search field is valid
            if (searchExpressions.ContainsKey(searchBy))
            {
                // Apply the corresponding filtering expression to the query
                propertyRentalManagementDbContext = searchExpressions[searchBy](searchTerm, isStrictEquality);
            }

            // Create a SelectList for the search options
            ViewBag.SearchByList = new SelectList(searchOptions, "Key", "Value", searchBy);

            // Execute the query and retrieve the filtered results from the database
            return View(await propertyRentalManagementDbContext.ToListAsync());
        }

        // GET: Tenants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Tenants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            // ViewData["RoleId"] = new SelectList(_context.UserRoles, "RoleId", "RoleId", user.RoleId);
            return View(user);
        }

        // POST: Tenants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,FirstName,LastName,Email,PhoneNumber,Password,RoleId")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                try
                {
                    if ((await _context.Users.FirstOrDefaultAsync((u) => u.Username == user.Username)) != null)
                    {
                        ModelState.AddModelError("Username", "Username already exists");
                    }
                    else
                    {
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // ViewData["RoleId"] = new SelectList(_context.UserRoles, "RoleId", "RoleId", user.RoleId);
            return View(user);
        }

        // GET: Tenants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Tenants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
