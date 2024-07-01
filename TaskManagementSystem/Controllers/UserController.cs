using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using TaskManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Controllers;

[Authorize(Roles = "Admin")] // restrict access to admin users only
public class UserController : Controller
{
	private readonly SignInManager<AppUser> _signInManager;
	private readonly UserManager<AppUser> _userManager;
	private readonly TaskManagementContext _context;

	public UserController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, TaskManagementContext context)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_context = context;
	}
	public async Task<IActionResult> Index()
	{
		var users = _context.AppUsers.ToList();
		foreach (var user in users)
		{
			var roles = await _userManager.GetRolesAsync(user);
			user.Role = roles.FirstOrDefault();
		}
		return View(users);
	}

	// GET: Vendor/Edit/5
	public async Task<IActionResult> Edit(string? id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return View(new AppUser());
		}

		var user = await _context.AppUsers.FindAsync(id);
		var roles = await _userManager.GetRolesAsync(user);
		user.Role = roles.FirstOrDefault();
		return View(user);
	}

	[HttpPost]
	public async Task<IActionResult> SaveChanges([FromBody] AppUser user)
	{
		if (user.FirstName != null && user.LastName != null && user.PasswordHash != null
			&& user.Email != null && user.PhoneNumber != null && user.Designation != null)
		{
			var existingUser = await _userManager.FindByEmailAsync(user.Email);

			if (existingUser == null)
			{
				// Create a new user
				var pwd = user.PasswordHash;
				user.NormalizedEmail = user.Email.ToUpper();
				user.UserName = user.Email; // Setting the username as the email address
				user.NormalizedUserName = user.Email.ToUpper();
				user.PasswordHash = null;

				// Attempt to create the user
				var result = await _userManager.CreateAsync(user, pwd);

				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync(user.Email), user.Role);
					return Ok("New User saved successfully"); // Return success message
				}
				return Ok("User saving unsuccessfull. " + result.Errors.First().Description); // Return failure message
			}
			else
			{
				existingUser.FirstName = user.FirstName;
				existingUser.LastName = user.LastName;
				existingUser.Email = user.Email;
				existingUser.NormalizedEmail = user.Email.ToUpper();
				existingUser.UserName = user.Email; // Setting the username as the email address
				existingUser.NormalizedUserName = user.Email.ToUpper();
				existingUser.PhoneNumber = user.PhoneNumber;
				existingUser.BirthDate = user.BirthDate;
				existingUser.Designation = user.Designation;

				// Attempt to create the user
				var result = await _userManager.UpdateAsync(existingUser);

				if (result.Succeeded)
				{
					return Ok("User saved successfully"); // Return success message
				}
				return Ok("User saving unsuccessfull"); // Return failure message
			}
		}
		return BadRequest("Invalid data. Fill all fields"); // Return bad request if model state is invalid
	}

	private bool UserExists(string email)
	{
		return _context.AppUsers.Any(e => e.Email == email);
	}

	[HttpPost]
	public async Task<IActionResult> Delete(string id)
	{
		try
		{
			// Retrieve the vendor from the database
			var vendor = await _context.AppUsers.FindAsync(id);
			if (vendor == null)
			{
				return NotFound();
			}

			// Remove the vendor from the database
			_context.AppUsers.Remove(vendor);
			await _context.SaveChangesAsync();

			// Return success message
			return Ok("User deleted successfully");
		}
		catch (Exception ex)
		{
			// Return error message
			return BadRequest(ex.Message);
		}
	}

	[HttpGet]
	public IActionResult GetUsers()
	{
		var users = _context.AppUsers.Select(v => new
		{
			UserID = v.Id,
			Name = v.FirstName
		}).ToList();

		return Json(users);
	}
}
