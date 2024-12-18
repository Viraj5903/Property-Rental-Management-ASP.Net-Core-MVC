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
    [Authorize(Roles = "Manager")]
    public class BuildingsController : Controller
    {
        private readonly PropertyRentalManagementDbContext _context;

        public BuildingsController(PropertyRentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Buildings
        public async Task<IActionResult> Index()
        {
            var propertyRentalManagementDbContext = _context.Buildings.Include(b => b.Manager).Include(b => b.Owner);
            return View(await propertyRentalManagementDbContext.ToListAsync());
        }

        // GET: Buildings/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await _context.Buildings
                .Include(b => b.Manager)
                .Include(b => b.Owner)
                .Include(b => b.Apartments)
                .FirstOrDefaultAsync(m => m.BuildingCode == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // GET: Buildings/Create
        public IActionResult Create()
        {
            PopulateViewData();
            return View();
        }

        // POST: Buildings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BuildingCode,OwnerId,ManagerId,Name,Description,Address,City,Province,ZipCode")] Building building)
        {
            ModelState.Remove("Owner");
            if (ModelState.IsValid)
            {
                _context.Add(building);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateViewData(building);
            return View(building);
        }

        // GET: Buildings/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await _context.Buildings.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }

            PopulateViewData(building);
            return View(building);
        }

        // POST: Buildings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BuildingCode,OwnerId,ManagerId,Name,Description,Address,City,Province,ZipCode")] Building building)
        {
            if (id != building.BuildingCode)
            {
                return NotFound();
            }

            ModelState.Remove("Owner");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(building);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuildingExists(building.BuildingCode))
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

            PopulateViewData(building);
            return View(building);
        }

        // GET: Buildings/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await _context.Buildings
                .Include(b => b.Manager)
                .Include(b => b.Owner)
                .FirstOrDefaultAsync(m => m.BuildingCode == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // POST: Buildings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building != null)
            {
                _context.Buildings.Remove(building);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BuildingExists(string id)
        {
            return _context.Buildings.Any(e => e.BuildingCode == id);
        }

        /// <summary>
        /// Populates the ViewData dictionary with select lists for Manager and Owner dropdowns in the Building view.
        /// This method provides dropdowns that display User IDs along with full names for managers and owners,
        /// and pre-selects the values based on the provided building object.
        /// </summary>
        /// <param name="building">An optional Building object used to pre-select values in the dropdowns.</param>
        private void PopulateViewData(Building? building = null)
        {
            // Populate the ManagerId dropdown with UserId and full name for Managers.
            // If a building is provided, pre-select its ManagerId.
            ViewData["ManagerId"] = new SelectList(
                _context.Users.Where(u => u.Role.Role == "Manager")
                .Select(u => new
                {
                    u.UserId,
                    DisplayName = $"{u.UserId} - {u.FirstName} {u.LastName}" // Combine UserId with full name for display.
                }),
                "UserId",
                "DisplayName",
                building?.ManagerId); // Pre-select ManagerId if provided.

            // Populate the OwnerId dropdown with UserId and full name for Owners.
            // If a building is provided, pre-select its OwnerId.
            ViewData["OwnerId"] = new SelectList(
                _context.Users.Where(u => u.Role.Role == "Owner")
                .Select(u => new
                {
                    u.UserId,
                    DisplayName = $"{u.UserId} - {u.FirstName} {u.LastName}" // Combine UserId with full name for display.
                }),
                "UserId",
                "DisplayName",
                building?.OwnerId); // Pre-select OwnerId if provided.
        }
    }
}
