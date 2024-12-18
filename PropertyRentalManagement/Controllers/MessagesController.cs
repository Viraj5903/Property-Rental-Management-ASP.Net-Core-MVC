using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    [Authorize(Roles = "Manager, Tenant")]
    public class MessagesController : Controller
    {
        private readonly PropertyRentalManagementDbContext _context;

        public MessagesController(PropertyRentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();

            var propertyRentalManagementDbContext = _context.Messages.Include(m => m.ReceiverUser).Include(m => m.SenderUser).Include(m => m.Status).Where(m => m.ReceiverUserId == userId || m.SenderUserId == userId);
            return View(await propertyRentalManagementDbContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.ReceiverUser)
                .Include(m => m.SenderUser)
                .Include(m => m.Status)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            if (message.SenderUserId != GetCurrentUserId() || message.ReceiverUserId != GetCurrentUserId())
            {
                Forbid();
            }

            if (message.ReceiverUserId == GetCurrentUserId())
            {
                try
                {
                    message.StatusId = 7; // Read
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            PopulateViewData();
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,SenderUserId,ReceiverUserId,Subject,MessageBody,MessageDateTime,StatusId")] Message message)
        {

            message.SenderUserId = GetCurrentUserId();
            message.MessageDateTime = DateTime.Now;
            message.StatusId = 8;

            ModelState.Remove("Status");
            ModelState.Remove("ReceiverUser");
            ModelState.Remove("SenderUser");
            
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateViewData(message);
            return View(message);
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
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

        private void PopulateViewData(Message? message = null) 
        {
            string? userRoleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRoleClaim != null)
            {
                string roleToDisplay = userRoleClaim == "Manager" ? "Tenant" : "Manager";

                ViewData["ReceiverUserId"] = new SelectList(_context.Users
                    .Where(u => u.Role.Role == roleToDisplay)
                    .Select(u => new
                    {
                        u.UserId,
                        DisplayName = $"{u.UserId} - {u.FirstName} {u.LastName}"
                    }), "UserId", "DisplayName", message?.ReceiverUserId);
            }
        }
    }
}
