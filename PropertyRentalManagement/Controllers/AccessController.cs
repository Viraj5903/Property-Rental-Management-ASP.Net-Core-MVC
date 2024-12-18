using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using PropertyRentalManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PropertyRentalManagement.Controllers
{
    public class AccessController : Controller
    {
        private readonly PropertyRentalManagementDbContext _context;

        public AccessController(PropertyRentalManagementDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This action handles the GET request to the Login page.
        /// It checks if the user is already authenticated. If the user is authenticated, 
        /// they will be redirected to the "Index" page of the "Home" controller.
        /// </summary>
        /// <returns>A view displaying the login form if the user is not authenticated, 
        /// otherwise, the user is redirected to the home page.</returns>
        public IActionResult Login()
        {
            // Retrieve the ClaimsPrincipal (user information) from the current HTTP context.
            ClaimsPrincipal claimUser = HttpContext.User;

            // Check if the user is authenticated. 
            // The ?. operator ensures that it will not throw a NullReferenceException if claimUser is null.
            // It checks both claimUser and claimUser.Identity for null values before trying to access IsAuthenticated.
            if (claimUser?.Identity?.IsAuthenticated == true)
            {
                // If the user is authenticated, redirect them to the "Index" action of the "Home" controller.
                return RedirectToAction("Index", "Home");
            }
            // If the user is not authenticated, return the Login view for them to log in.
            return View();
        }

        /// <summary>
        /// Handles the HTTP POST request for user login. 
        /// This method checks the credentials provided by the user, 
        /// and if valid, signs the user in using cookie-based authentication. 
        /// If authentication is successful, the user is redirected to the "Index" action of the "Home" controller. 
        /// If authentication fails, appropriate error messages are added to the ModelState and the login view is returned with validation messages.
        ///
        /// <para>
        /// The method performs the following steps:
        /// 1. Retrieves the user from the database based on the provided username.
        /// 2. Validates the password provided by the user against the stored password in the database.
        /// 3. If the password is correct, creates authentication claims (user identifier and role) and generates a claims identity.
        /// 4. Creates authentication properties, including whether the session is persistent (i.e., whether the user should remain logged in across sessions).
        /// 5. Signs the user in by setting the authentication cookie with the generated claims and properties.
        /// 6. Redirects the user to the "Index" page of the "Home" controller.
        /// 7. If authentication fails, adds an appropriate error message to the ModelState, which is displayed to the user in the login view.
        /// </para>
        /// 
        /// <param name="modelLogin">The login view model containing the user's input for username, password, and a flag indicating whether the user wants to remain logged in (KeepLoggedIn).</param>
        /// <returns>
        /// If the credentials are valid, redirects to the "Home" controller's "Index" action.
        /// If the credentials are invalid, returns the login view with validation error messages.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Login(VMLogin modelLogin)
        {
            if (ModelState.IsValid)
            {
                // Using the PropertyRentalManagementDbContext to interact with the database
                
                // Using LINQ to query the Users table and fetch the user based on the provided username.
                // The 'FirstOrDefaultAsync' is an asynchronous version of 'FirstOrDefault' that retrieves the first user 
                // matching the username, or 'null' if no match is found.
                User? user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync((u) => u.Username == modelLogin.Username);
                // User? user = await _context.Users.Include((User u) => u.Role).FirstOrDefaultAsync((u) => u.Username == modelLogin.Username);

                // If the user is found (user is not null), proceed with password validation
                if (user != null)
                {

                    // Check if the provided password matches the user's stored password
                    if (user.Password == modelLogin.Password)
                    {

                        // Create a list of claims to be associated with the authenticated user
                        // Claims represent the user's identity and roles, and will be used to generate the authentication cookie.
                        List<Claim> claims = new List<Claim>()
                        {
                            // Add a claim for the user's unique identifier (in this case, the Username)
                            new Claim(ClaimTypes.NameIdentifier, modelLogin.Username),

                            // Add a claim for the user's role (for authorization purposes)
                            // Assuming that the user entity has a "Role" property that links to a "Role" object containing the role name.
                            new Claim(ClaimTypes.Role, user.Role.Role), // user.Role.Role contains the user's role name.

                            // Add a claim for the user's ID
                            new Claim("UserId", user.UserId.ToString()) // "UserId" is a custom claim type. You can use any string as the claim type.
                        };

                        // Create a ClaimsIdentity to represent the user's identity
                        // The ClaimsIdentity object is initialized with the list of claims and the authentication scheme.
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);

                        // AuthenticationProperties object allows configuring the properties of the authentication session.
                        // It includes whether the session should be persistent (e.g., remain active after closing the browser).
                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true, // Whether the authentication cookie can be refreshed
                            IsPersistent = modelLogin.KeepLoggedIn // Set the persistence of the session based on the "KeepLoggedIn" flag
                        };

                        // Sign in the user by creating an authentication cookie containing their claims.
                        // HttpContext.SignInAsync creates and adds the cookie to the user's browser.
                        // The ClaimsPrincipal represents the user and includes the claims and the authentication identity.
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), properties);

                        // Redirect the user to the "Index" action of the "Home" controller after successful login.
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // If the password doesn't match, add a model error to notify the user
                        // that the provided password is incorrect.
                        ModelState.AddModelError("Password", "Wrong Password");
                    }
                }
                else
                {
                    // If no user with the provided username is found, add a model error
                    // that the user does not exist in the database.
                    ModelState.AddModelError("Username", "User not found.");
                }
            }
 
            // Return the view with the validation errors (e.g., "Wrong Password" or "User not found")
            // and allow the user to correct their input.
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("Username,Password,RoleId,FirstName,LastName,Email,PhoneNumber")] User model)
        {
            ModelState.Remove("Role");

            if (ModelState.IsValid)
            {
                // Start a new transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {

                        // Check if the username already exists in the database
                        User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                        // If the username is already taken, add an error and return the view
                        if (existingUser != null)
                        {
                            ModelState.AddModelError("Username", "Username is already taken. Please enter a unique username.");
                            return View(); // Return view with error message
                        }

                        // Create the new User object
                        User user = new User()
                        {
                            Username = model.Username,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            Password = model.Password,  // Store the hashed password, not plain text
                            RoleId = model.RoleId,  // Assuming 3 is the RoleId for tenants
                        };

                        // Add the new user to the context
                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();  // Save the new user to the database

                        // If everything is successful, commit the transaction
                        await transaction.CommitAsync();

                        // Redirect to the login page after successful signup
                        return RedirectToAction("Login", "Access");
                    }
                    catch (Exception ex)
                    {
                        // Log the exception (optional)
                        //_logger.LogError(ex, "An error occurred while processing the signup.");

                        // Rollback the transaction if anything goes wrong
                        await transaction.RollbackAsync();

                        // Add a generic error message to ViewData
                        ViewData["ErrorMessage"] = "An error occurred while processing your request. Please try again." + ex.Message;
                        return View();  // Return the view with an error message
                    }
                }
            }

            // If ModelState is not valid, return the view with validation errors
            return View();
        }

        [HttpGet]
        public IActionResult Denied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
