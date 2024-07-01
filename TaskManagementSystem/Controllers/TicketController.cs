using TaskManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace TaskManagementSystem.Controllers;

public class TicketController : Controller
{
	private readonly UserManager<AppUser> _userManager;
	private readonly TaskManagementContext _context;

	public TicketController(UserManager<AppUser> userManager, TaskManagementContext context)
	{
		_userManager = userManager;
		_context = context;
	}

	public ActionResult Index()
	{
		var tickets = _context.Tickets.Include(x => x.AssignedEmployee).ToList();
		return View(tickets);
	}

	// GET: Ticket/Edit/5
	public IActionResult Edit(int? id)
	{
		if (id == 0 || id == null)
		{
			return View(new Ticket());
		}

		var ticket = _context.Tickets.Find(id);
		return View(ticket);
	}

	[HttpPost]
	public async Task<IActionResult> SaveChanges([FromBody] Ticket ticket)
	{
        if (ticket == null)
        {
            return BadRequest("Ticket is null");
        }

        if ( !string.IsNullOrEmpty(ticket.Description) && !string.IsNullOrEmpty(ticket.Title))
		{
			if (ticket.Id == null || ticket.Id == 0)
			{
				// Create a new Task
				ticket.TicketCreatedDate = DateTime.Now;
				ticket.State = TicketState.Created;

				// Attempt to create the Task
				await _context.Tickets.AddAsync(ticket);
				await _context.SaveChangesAsync();
				return Ok("New Task saved successfully"); // Return success message
			}
			else
			{
				var exisitingTicket = await _context.Tickets.FindAsync(ticket.Id);
				exisitingTicket.Title = ticket.Title;
				exisitingTicket.Description = ticket.Description;
				exisitingTicket.DueDate  = ticket.DueDate;
				exisitingTicket.TicketCreatedDate = DateTime.Now;

				// Attempt to update the Task
				_context.Tickets.Update(exisitingTicket);
				await _context.SaveChangesAsync();
				return Ok("Task updated successfully"); // Return success message
			}
		}
		return BadRequest("Invalid data. Fill all fields"); // Return bad request if model state is invalid
	}

	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			// Retrieve the ticket from the database
			var ticket = await _context.Tickets.Where(x=>x.Id==id).FirstOrDefaultAsync();
			if (ticket == null)
			{
				return NotFound();
			}

			// Remove the Task from the database
			_context.Tickets.Remove(ticket);
			await _context.SaveChangesAsync();

			// Return success message
			return Ok("Task deleted successfully");
		}
		catch (Exception ex)
		{
			// Return error message
			return BadRequest(ex.Message);
		}
	}
}
