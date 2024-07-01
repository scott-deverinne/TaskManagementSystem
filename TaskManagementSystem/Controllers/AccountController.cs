using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.Models;

public class AccountController : Controller
{
	private readonly SignInManager<AppUser> _signInManager;
	private readonly UserManager<AppUser> _userManager;
	private readonly TaskManagementContext _context;

	public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, TaskManagementContext context)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_context = context;
	}

	[HttpGet]
	public IActionResult Login()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(Login model)
	{
		if (ModelState.IsValid)
		{
			AppUser signedUser = await _userManager.FindByEmailAsync(model.Email);

			if (signedUser != null)
			{
				var result = await _signInManager.PasswordSignInAsync(signedUser.UserName, model.Password, true, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					await _userManager.AddClaimAsync(signedUser, new Claim(ClaimTypes.Email, model.Email));
					return RedirectToAction("Index", "Home");
				}
			}
			ModelState.AddModelError("Email", "Invalid login attempt.");
			return View(model);
		}
		return View(model);
	}

	public async Task<IActionResult> Logout()
	{
		await _signInManager.SignOutAsync();
		return RedirectToAction(nameof(AccountController.Login), "Account");
	}

	private IActionResult RedirectToLocal(string returnUrl)
	{
		if (Url.IsLocalUrl(returnUrl))
		{
			return Redirect(returnUrl);
		}
		return RedirectToAction(nameof(HomeController.Index), "Home");
	}
}
