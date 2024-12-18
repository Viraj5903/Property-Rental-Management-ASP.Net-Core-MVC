using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    public class EventsController : Controller
    {
        private readonly PropertyRentalManagementDbContext _context;

        public EventsController(PropertyRentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Events
        [Authorize (Roles = "Manager, Owner")]
        public async Task<IActionResult> Index()
        {
            var propertyRentalManagementDbContext = _context.Events.Include(e => e.Apartment).Include(e => e.Manager).Include(e => e.Status);
            return View(await propertyRentalManagementDbContext.ToListAsync());
        }

        // GET: Events/Details/5
        [Authorize (Roles = "Manager, Owner")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Apartment)
                .Include(e => e.Manager)
                .Include(e => e.Status)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            PopulateViewData();
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,ManagerId,ApartmentId,Description,EventDate,StatusId")] Event @event)
        {

            @event.ManagerId = GetCurrentUserId();

            ModelState.Remove("Apartment");
            ModelState.Remove("Manager");
            ModelState.Remove("Status");

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateViewData(@event);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            PopulateViewData(@event);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,ManagerId,ApartmentId,Description,EventDate,StatusId")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            ModelState.Remove("Apartment");
            ModelState.Remove("Manager");
            ModelState.Remove("Status");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
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

            PopulateViewData(@event);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Apartment)
                .Include(e => e.Manager)
                .Include(e => e.Status)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }

        private int GetCurrentUserId()
        {
            // Attempt to find the "UserId" claim in the current user's claims.
            var userIdClaim = HttpContext.User.FindFirst("UserId");

            // Return the UserId as an integer, or 0 if the claim is not present.
            return userIdClaim != null ? Convert.ToInt32(userIdClaim.Value) : 0;
        }

        private void PopulateViewData(Event? @event = null)
        {

            int managerId = GetCurrentUserId();

            ViewData["ApartmentId"] = new SelectList(_context.Apartments.Include(a => a.Building).Where(a => a.Building.ManagerId == managerId).Select(a => new
                {
                    a.ApartmentId,
                    DisplayName = $"{a.ApartmentId} - {a.ApartmentCode} - {a.BuildingCode} {a.Building.Name}"
                }), "ApartmentId", "DisplayName", @event?.ApartmentId);

            ViewData["StatusId"] = new SelectList(_context.Statuses.Where(s => s.Category == "Events"), "StatusId", "Description", @event?.StatusId);
        }
    }
}
