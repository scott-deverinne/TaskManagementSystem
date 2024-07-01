using TaskManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace TaskManagementSystem.Controllers;

public class AssignTicketController : Controller
{
	private readonly UserManager<AppUser> _userManager;
	private readonly TaskManagementContext _context;

	public AssignTicketController(UserManager<AppUser> userManager, TaskManagementContext context)
	{
		_userManager = userManager;
		_context = context;
	}

	public async Task<ActionResult> IndexAsync()
	{
		var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

		// Find the user based on the email address
		var user = await _userManager.FindByEmailAsync(userEmail);
		var tickets = _context.Tickets.Include(x => x.AssignedEmployee)
						.Where(x => (x.State == 0 && x.AssignedEmployee==null) || x.AssignedEmployee.Id == user.Id).ToList();
		return View(tickets);
	}

	// GET: Ticket/Edit/5
	public IActionResult Edit(int? id)
	{
		if (id == 0 || id == null)
		{
			return View(new Ticket());
		}

		var ticket = _context.Tickets.Where(x => x.Id == id).FirstOrDefault();

		return View(ticket);
	}

	[HttpPost]
	public async Task<IActionResult> SaveChanges(Ticket ticket)
	{
		if (ModelState.IsValid)
		{
			var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			// Find the user based on the email address
			var user = await _userManager.FindByEmailAsync(userEmail);

			ticket.AssignedEmployee = user;

			// Attempt to update the ticket
			_context.Tickets.Update(ticket);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index"); // Return success message
		}
		return View("Edit", ticket);
	}
}