using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Services;
using TaskManager.Models;
using System.Threading.Tasks;

namespace TaskManager.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly JwtTokenService _jwtTokenService;
        private readonly TaskDbContext _context;

        public AuthController(JwtTokenService jwtTokenService, TaskDbContext context)
        {
            _jwtTokenService = jwtTokenService;
            _context = context;
        }

        // MVC Actions for Views (these don't need to be async as they don't perform I/O)
        [HttpGet]
        [Route("/Auth/Login")]
        public IActionResult LoginView()
        {
            return View("Login");
        }

        [HttpGet]
        [Route("/Auth/Signup")]
        public IActionResult SignupView()
        {
            return View("Signup");
        }

        // API Endpoints - Updated with proper async/await
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { Message = "Username and Password are required." });
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == request.Username)
                .ConfigureAwait(false);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            var token = _jwtTokenService.GenerateToken(user.Username, "User");
            return Ok(new { Token = token });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { Message = "Username and Password are required." });
            }

            bool userExists = await _context.Users
                .AnyAsync(u => u.Username == request.Username)
                .ConfigureAwait(false);

            if (userExists)
            {
                return BadRequest(new { Message = "Username already exists" });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var newUser = new User
            {
                Username = request.Username,
                Password = hashedPassword
            };

            await _context.Users.AddAsync(newUser).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return Ok(new { Message = "User registered successfully" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class SignupRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}