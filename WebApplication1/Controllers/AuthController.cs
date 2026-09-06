using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using WebApplication1.data;
using WebApplication1.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AuthController(AppDbContext context) : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
            ViewBag.SuccessMessage = TempData["Success Message"];
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> CreateUser(UserDto dto)
        {
            if (dto == null ||
                string.IsNullOrEmpty(dto.Username) ||
                string.IsNullOrEmpty(dto.Password) ||
                string.IsNullOrEmpty(dto.Email))
            {
                ViewBag.ErrorMessage = "All fields are required";
                return View("Register");
            }

            var existingUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser == null)
            {
                var passwordHasher = new PasswordHasher<User>();

                var user = new User
                {
                    Email = dto.Email,
                    Username = dto.Username
                };

                user.Password = passwordHasher.HashPassword(user, dto.Password);

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
            else
            {
                ViewBag.ErrorMessage =
                    "User with this email already exists";

                return View("Register");
            }

            TempData["Success Message"] = "User created successfully";

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(UserDto dto)
        {
            if (dto == null ||
                string.IsNullOrEmpty(dto.Password) ||
                string.IsNullOrEmpty(dto.Email))
            {
                ViewBag.ErrorMessage = "All fields are required";
                return View("Login");
            }

            var existingUser = await context.Users
                .FirstOrDefaultAsync(h => h.Email == dto.Email);

            if (existingUser == null)
            {
                ViewBag.ErrorMessage = "This user does not exist";
                return View("Login");
            }

            var passwordHasher = new PasswordHasher<User>();

            var result = passwordHasher.VerifyHashedPassword(
                existingUser,
                existingUser.Password,
                dto.Password
            );

            if (result == PasswordVerificationResult.Success)
            {
                var token = GenerateJWTtoken(dto);

                Response.Cookies.Append("jwt_key", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                return RedirectToAction("Index", "Dash");
            }

            ViewBag.ErrorMessage = "Password is not correct";
            return View("Login");
        }

        public string GenerateJWTtoken(UserDto dto)
        {
            var jwtHandler = new JsonWebTokenHandler();

            var key = Encoding.UTF8.GetBytes(
                "rU9OlMpC4glenNRVhRni63FNvpmqLIE76hi1WXziK8e"
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, dto.Email)
                }),

                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            return jwtHandler.CreateToken(tokenDescriptor);
        }

        public IActionResult LogOut()
        {
            Response.Cookies.Delete("jwt_key");
            return RedirectToAction("Login");

        }
    }
}