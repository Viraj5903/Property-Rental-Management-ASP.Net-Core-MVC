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
    [Authorize(Roles = "Tenant")]
    public class AvailableApartmentsController : Controller
    {
        private readonly PropertyRentalManagementDbContext _context;

        public AvailableApartmentsController(PropertyRentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: AvailableApartments
        public async Task<IActionResult> Index()
        {
            var propertyRentalManagementDbContext = _context.Apartments.Include(a => a.ApartmentType).Include(a => a.Building).Include(a => a.Status).Where(a => a.StatusId == 1);
            return View(await propertyRentalManagementDbContext.ToListAsync());
        }

        // GET: AvailableApartments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.ApartmentType)
                .Include(a => a.Building)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }

            if (apartment.StatusId != 1) 
            {
                Forbid();
            }

            return View(apartment);
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartments.Any(e => e.ApartmentId == id);
        }
    }
}
