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
    public class ApartmentsController : Controller
    {
        private readonly PropertyRentalManagementDbContext _context;

        public ApartmentsController(PropertyRentalManagementDbContext context)
        {
            _context = context;
        }

        // GET: Apartments
        public async Task<IActionResult> Index()
        {
            var propertyRentalManagementDbContext = _context.Apartments.Include(a => a.ApartmentType).Include(a => a.Building).Include(a => a.Status);
            return View(await propertyRentalManagementDbContext.ToListAsync());
        }

        // GET: Apartments/Details/5
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

            return View(apartment);
        }

        // GET: Apartments/Create
        public IActionResult Create()
        {
            PopulateViewData();
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Creates a new apartment record in the database.
        /// </summary>
        /// <param name="apartment">An instance of the Apartment model containing the details of the apartment to be created.</param>
        /// <param name="ApartmentImage">An IFormFile representing the image of the apartment to be uploaded.</param>
        /// <returns>
        /// An IActionResult that redirects to the Index view if the creation is successful,
        /// or returns the Create view with validation errors if the model state is invalid.
        /// </returns>
        /// <remarks>
        /// The method performs the following steps:
        /// 1. Removes certain properties from model validation that are not needed during creation.
        /// 2. Checks if the model state is valid; if valid, it attempts to process the apartment creation.
        /// 3. Validates the uploaded image file, ensuring it meets the required types (jpg, jpeg, png, gif).
        /// 4. If the image is valid, it reads the file into a memory stream and stores it as a byte array in the apartment entity.
        /// 5. If all validations pass, it adds the apartment entity to the database context and saves changes.
        /// 6. If there are errors during the process, it catches exceptions, logs the error, and adds a user-friendly error message.
        /// 7. If the model state is invalid or an error occurs, it repopulates the view data for dropdowns and returns the view with the apartment model.
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApartmentId,ApartmentCode,BuildingCode,ApartmentTypeId,Description,Rent,StatusId")] Apartment apartment, IFormFile ApartmentImage)
        {
            /* IFormFile:
            IFormFile is an interface provided by ASP.NET Core that represents a file uploaded in an HTTP request.
            It contains properties and methods to work with the uploaded file, such as its name, content type, and the ability to read its contents.
            The CopyToAsync method is used to transfer the contents of the uploaded file into another stream (like MemoryStream).
            */

            // Remove properties that are not needed for model validation
            ModelState.Remove("Building");
            ModelState.Remove("Status");
            ModelState.Remove("ApartmentType");
            ModelState.Remove("ApartmentImage");

            // Check if the model state is valid (all required fields are filled correctly)
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if an image file has been uploaded
                    if (ApartmentImage != null && ApartmentImage.Length > 0)
                    {
                        // Define the allowed image file extensions
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        // Get the file extension of the uploaded file
                        var fileExtension = Path.GetExtension(ApartmentImage.FileName).ToLowerInvariant();

                        // Validate the file extension against the allowed ones
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            // If the file type is invalid, add an error message to the model state
                            ModelState.AddModelError("ApartmentImage", "Please upload a valid image file (jpg, jpeg, png, gif).");
                        }
                        else
                        {
                            /* MemoryStream:
                            A MemoryStream is a class in the System.IO namespace that provides a stream of data stored in memory (RAM).
                            It is particularly useful for scenarios where we want to manipulate data in memory rather than writing it to a file on disk.
                            In this context, it temporarily holds the binary data of the uploaded image before it is saved to the database.
                            */

                            // If the file is valid, read it into a memory stream
                            using (var memoryStream = new MemoryStream())
                            {
                                /* await ApartmentImage.CopyToAsync(memoryStream);:
                                This line of code performs an asynchronous operation to copy the contents of the ApartmentImage (the uploaded file) into the memoryStream.
                                It allows the application to continue processing other requests while the file is being read, which is especially important for web applications where responsiveness is key.
                                */
                                // The ApartmentImage parameter is of type IFormFile, which represents a file sent with the HttpRequest.
                                // This is commonly used in ASP.NET Core for handling file uploads from forms.
                                // The CopyToAsync method asynchronously copies the contents of the IFormFile to the specified stream, in this case, the memoryStream we just created.
                                await ApartmentImage.CopyToAsync(memoryStream);

                                // Store the image data as a byte array in the apartment entity
                                apartment.ApartmentImage = memoryStream.ToArray();
                                // After the image data has been copied to the memory stream, we convert the contents of the stream into a byte array.
                                // The ToArray method reads all the bytes from the current position to the end of the stream and returns them as an array.
                                // This byte array will be stored in the Apartment entity's ApartmentImage property.
                            }
                        }
                    }

                    // Re-check the model state after image validation
                    if (ModelState.IsValid)
                    {
                        // If everything is valid, add the new apartment entity to the context
                        _context.Add(apartment);
                        // Save changes to the database asynchronously
                        await _context.SaveChangesAsync();
                        // Redirect to the index action to display the list of apartments
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception message to the console for debugging
                    Console.WriteLine(ex.Message);
                    // Add a generic error message to the model state for user feedback
                    ModelState.AddModelError("", "An error occurred while saving the apartment. Please try again.");
                }
            }

            // If the model state is not valid or if an error occurred, repopulate the view data
            PopulateViewData(apartment);
            // Return the view with the apartment model to show validation errors
            return View(apartment);
        }

        // GET: Apartments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }

            PopulateViewData(apartment);
            return View(apartment);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

         /// <summary>
         /// Edits an existing apartment record in the database.
         /// </summary>
         /// <param name="id">The ID of the apartment to edit.</param>
         /// <param name="apartment">An instance of the Apartment model containing the updated details.</param>
         /// <param name="ApartmentImage">An IFormFile representing the new image of the apartment, if uploaded.</param>
         /// <returns>
         /// An IActionResult that redirects to the Index view if the edit is successful,
         /// or returns the Edit view with validation errors if the model state is invalid.
         /// </returns>
         /// <remarks>
         /// The method performs the following steps:
         /// 1. Verifies that the ID from the route matches the ApartmentId of the submitted apartment entity.
         /// 2. Removes unnecessary properties from model validation to avoid errors during editing.
         /// 3. Checks if the model state is valid; if valid, it processes the edit operation.
         /// 4. Retrieves the existing apartment record from the database to update its properties.
         /// 5. Validates the uploaded image file, ensuring it meets the required types (jpg, jpeg, png, gif).
         /// 6. If a new image is uploaded and valid, it reads the file into a memory stream and stores it as a byte array in the apartment entity.
         /// 7. Saves changes to the database and handles potential concurrency exceptions.
         /// 8. If the model state is invalid or an error occurs, it repopulates the view data for dropdowns and returns the view with the apartment model.
         /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApartmentId,ApartmentCode,BuildingCode,ApartmentTypeId,Description,Rent,StatusId")] Apartment apartment, IFormFile ApartmentImage)
        {
            /* IFormFile:
            IFormFile is an interface provided by ASP.NET Core that represents a file uploaded in an HTTP request.
            It contains properties and methods to work with the uploaded file, such as its name, content type, and the ability to read its contents.
            The CopyToAsync method is used to transfer the contents of the uploaded file into another stream (like MemoryStream).
            */

            // Check if the provided ID matches the ApartmentId of the submitted apartment entity.
            if (id != apartment.ApartmentId)
            {
                return NotFound(); // Return a 404 Not Found if the IDs do not match.
            }

            // Remove properties that are not needed for model validation.
            ModelState.Remove("Building");
            ModelState.Remove("Status");
            ModelState.Remove("ApartmentType");
            ModelState.Remove("ApartmentImage");

            // Check if the ModelState is valid (all required fields are filled correctly).
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing apartment entity from the database.
                    var existingApartment = await _context.Apartments.FindAsync(id);
                    if (existingApartment == null)
                    {
                        return NotFound(); // Return a 404 Not Found if the apartment doesn't exist.
                    }

                    // Update the existing apartment's properties with the new values from the submitted form.
                    existingApartment.ApartmentCode = apartment.ApartmentCode;
                    existingApartment.BuildingCode = apartment.BuildingCode;
                    existingApartment.ApartmentTypeId = apartment.ApartmentTypeId;
                    existingApartment.Description = apartment.Description;
                    existingApartment.Rent = apartment.Rent;
                    existingApartment.StatusId = apartment.StatusId;

                    // Check if a new image file is uploaded.
                    if (ApartmentImage != null && ApartmentImage.Length > 0)
                    {
                        // Validate the uploaded image file's extension.
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(ApartmentImage.FileName).ToLowerInvariant();

                        // If the file extension is valid, process the image.
                        if (allowedExtensions.Contains(fileExtension))
                        {
                            /* MemoryStream:
                            A MemoryStream is a class in the System.IO namespace that provides a stream of data stored in memory (RAM).
                            It is particularly useful for scenarios where we want to manipulate data in memory rather than writing it to a file on disk.
                            In this context, it temporarily holds the binary data of the uploaded image before it is saved to the database.
                            */
                            // Create a MemoryStream to temporarily hold the uploaded image data.
                            using (var memoryStream = new MemoryStream())
                            {
                                /* await ApartmentImage.CopyToAsync(memoryStream);:
                                This line of code performs an asynchronous operation to copy the contents of the ApartmentImage (the uploaded file) into the memoryStream.
                                It allows the application to continue processing other requests while the file is being read, which is especially important for web applications where responsiveness is key.
                                */
                                // The ApartmentImage parameter is of type IFormFile, which represents a file sent with the HttpRequest.
                                // This is commonly used in ASP.NET Core for handling file uploads from forms.
                                // The CopyToAsync method asynchronously copies the contents of the IFormFile to the specified stream, in this case, the memoryStream we just created.

                                // Asynchronously copy the contents of the uploaded file to the MemoryStream.
                                await ApartmentImage.CopyToAsync(memoryStream);

                                // Store the image data as a byte array in the existing apartment entity.
                                existingApartment.ApartmentImage = memoryStream.ToArray();
                                // After the image data has been copied to the memory stream, we convert the contents of the stream into a byte array.
                                // The ToArray method reads all the bytes from the current position to the end of the stream and returns them as an array.
                                // This byte array will be stored in the Apartment entity's ApartmentImage property.
                            }
                        }
                        else
                        {
                            // If the file extension is not valid, add a model error.
                            ModelState.AddModelError("ApartmentImage", "Please upload a valid image file (jpg, jpeg, png, gif).");
                        }
                    }

                    // Save changes made to the apartment entity back to the database.
                    await _context.SaveChangesAsync();
                    // Redirect to the Index action if successful.
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency exceptions if the apartment was modified by another user.
                    if (!ApartmentExists(apartment.ApartmentId))
                    {
                        return NotFound(); // Return a 404 Not Found if the apartment doesn't exist.
                    }
                    else
                    {
                        throw; // Rethrow the exception for further handling.
                    }
                }
            }

            // Populate ViewData for dropdown lists if the ModelState is invalid.
            PopulateViewData(apartment);
            // Return the view with the apartment data to display errors.
            return View(apartment);
        }

        // GET: Apartments/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment != null)
            {
                _context.Apartments.Remove(apartment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateViewData(Apartment? apartment = null)
        {
            ViewData["ApartmentTypeId"] = new SelectList(_context.ApartmentTypes, "ApartmentTypeId", "ApartmentTypeDescription", apartment?.ApartmentTypeId);

            ViewData["BuildingCode"] = new SelectList(
                _context.Buildings
                    .Select(b => new 
                    {
                        b.BuildingCode,
                        DisplayName = $"{b.BuildingCode} - {b.Name}",
                    }), 
                "BuildingCode", 
                "DisplayName", 
                apartment?.BuildingCode);

            ViewData["StatusId"] = new SelectList(_context.Statuses.Where((s) => s.Category == "Apartments"), "StatusId", "Description", apartment?.StatusId);
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartments.Any(e => e.ApartmentId == id);
        }
    }
}
