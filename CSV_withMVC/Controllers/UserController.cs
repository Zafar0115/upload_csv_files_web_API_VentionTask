using CSV_withMVC.Models;
using CSV_withMVC.Persistence;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace CSV_withMVC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async ValueTask<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file, [FromServices] IHostingEnvironment hostingEnvironment)
        {

            List<User> users = new List<User>();
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            try
            {
              users = GetUsers(file.FileName);
            }
            catch (Exception)
            {
                return BadRequest("Incorrect file format. Could not parse this file");
            }
            int countUpdate = 0, countAdd = 0;

            foreach (User user in users)
            {
                User? foundUser = await _dbContext.Users.FindAsync(user.UserIdentifier);
                if (foundUser == null)
                {
                    await _dbContext.Users.AddAsync(user);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    foundUser.Username = user.Username;
                    foundUser.Age = user.Age;
                    foundUser.PhoneNumber = user.PhoneNumber;
                    foundUser.Email = user.Email;
                    foundUser.City = user.City;
                    await _dbContext.SaveChangesAsync();
                }
            }

            return Ok($"{countAdd} users added\n{countUpdate} users updated");

        }

        private List<User> GetUsers(string fileName)
        {
            List<User> users = new List<User>();
            var path = $"{Directory.GetCurrentDirectory()}\\wwwroot\\files\\{fileName}";
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var user = csv.GetRecord<User>();
                    users.Add(user);
                }
            }

            return users;
        }



        [HttpGet]
        [Route("retrieveAll")]
        public async Task<IActionResult> RetrieveAll(string? sortingOrder, int page, int pagesize)
        {
            var users = _dbContext.Users.AsQueryable();

            users.Skip((page - 1) * pagesize);
            users.Take(pagesize);

            if (sortingOrder!=null&& sortingOrder.ToLower() == "desc")
            {
                users = users.OrderByDescending(x => x.Username);
            }
            else
            {
                users = users.OrderBy(x => x.Username);
            }

            return users.Count() > 0? Ok(users.ToList()):  Ok("Database does not contain any user yet");
        }
    }
}
