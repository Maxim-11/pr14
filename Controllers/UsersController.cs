using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Class.Models;
using System.Security.Cryptography;

namespace Class.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ClassContext _context;
        private readonly JwtTokenService _jwtTokenService;

        public UsersController(ClassContext context, JwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;

        }

        public class TokenResponse
        {
            public string Token { get; set; }
            public int UserId { get; set; }

            public string UserType { get; set; }
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginModel loginModel)
        {
            // Ищем пользователя по email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);

            if (user == null)
            {
                // Пользователь с таким email не найден
                return Unauthorized("Неверные учетные данные");
            }

            // Расшифровываем сохранённый пароль пользователя
            var isPasswordCorrect = VerifyPassword(loginModel.Password, Convert.FromBase64String(user.Password), Convert.FromBase64String(user.Salt));

            if (!isPasswordCorrect)
            {
                // Пароль неверный
                return Unauthorized("Неверные учетные данные");
            }

            // Получаем тип пользователя
            var userType = user.UserType; // Предположим, что у вас есть поле UserType в вашей модели пользователя

            // Пароль верный, генерируем токен для пользователя
            var token = _jwtTokenService.GenerateToken(user);

            // Возвращаем полные данные о пользователе вместе с токеном
            var tokenResponse = new TokenResponse { Token = token, UserId = user.UserID, UserType = userType };
            return Ok(tokenResponse);
        }


        // Метод для проверки пароля
        private bool VerifyPassword(string password, byte[] savedPasswordHash, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                var hash = pbkdf2.GetBytes(20); // Получаем хеш введённого пароля
                return hash.SequenceEqual(savedPasswordHash); // Сравниваем хеш введённого пароля с хешем пароля пользователя из базы данных
            }
        }



        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            // Генерация соли
            byte[] salt = GenerateSalt();

            // Хеширование пароля
            byte[] passwordHash = HashPassword(user.Password, salt);

            // Сохранение соли и хешированного пароля в модели пользователя
            user.Password = Convert.ToBase64String(passwordHash);
            user.Salt = Convert.ToBase64String(salt);

            // Сохранение пользователя в базу данных
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Создание JWT-токена для пользователя
            var token = _jwtTokenService.GenerateToken(user);

            // Возвращение созданного пользователя и токена вместе с ответом
            return CreatedAtAction("GetUser", new { id = user.UserID }, new { user, token });
        }

        // Метод для генерации случайной соли
        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16]; // Размер соли в байтах
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // Метод для хеширования пароля с использованием соли
        private byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                return pbkdf2.GetBytes(20); // 20 - длина хешированного пароля в байтах
            }
        }



        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'ClassContext.Users'  is null.");
          }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserID == id)).GetValueOrDefault();
        }
    }
}
