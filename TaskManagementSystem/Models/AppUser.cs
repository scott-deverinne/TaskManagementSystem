using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Models
{
	public class AppUser : IdentityUser
	{
		// Additional properties

		[Required(ErrorMessage = "First Name is required")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last Name is required")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Destination is required")]
		public string Designation { get; set; }

		[NotMapped]
		[Required(ErrorMessage = "Role is required")]
		public string Role { get; set; }

		[Required(ErrorMessage = "Birth Date is required")]
		public DateOnly BirthDate { get; set; }
	}
}
