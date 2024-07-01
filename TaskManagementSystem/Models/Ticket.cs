namespace TaskManagementSystem.Models;
using System;
using System.ComponentModel.DataAnnotations;

public enum TicketState
{
	Created,
	InProgress,
	Done,
	Closed
}

public class Ticket
{
	[Required(ErrorMessage ="Task ID is required")]
	public int Id { get; set; }

	[Required(ErrorMessage = "Title is required")]
	public string Title { get; set; }

	[Required(ErrorMessage = "Description is required")]
	public string Description { get; set; }

	[Required(ErrorMessage = "Due Date is required")]
	public DateOnly? DueDate { get; set; }

	public AppUser? AssignedEmployee { get; set; }

	[Required(ErrorMessage = "State is required")]
	public TicketState State { get; set; }

	public DateTime TicketCreatedDate { get; set; } = DateTime.Now;
}
