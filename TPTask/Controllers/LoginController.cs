using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TPTask.Controllers
{
    [ApiController]
    [Route("login/[controller]")]
    //[EnableCors(origins: "http://www.example.com", headers: "*", methods: "*")]
    
    public class LoginController : Controller
    {
        private IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;

        }
       /* public IActionResult Index()
        {
            return View();
        }*/
       [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(string userName,string password)
        {
            var user = Authenticate(userName,password);
            if (user!= null) {
                var token = GenerateToken(user);
               // Response.Headers.Add("Access-Control-Allow-Origin", "*");
                return Ok(token);
            }
            return NotFound("Incorrect password or user name");
        }

        private string GenerateToken(UserData user)
        {
            //search symmetric 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier,user.UserName),
                                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,user.Role)



            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: DateTime.Now.AddMinutes(5), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
                }

        private UserData Authenticate(string userName, string password)
        {
            var query = @"select UserName, Password, Role from dbo.Users where UserName=@UserName and Password=@password ";
            DataTable table = new DataTable();
           // string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\bmt\OneDrive\المستندات\TpDb.mdf;Integrated Security=True;Persist Security Info=False;Connect Timeout=30";//_config.GetConnectionString("DbConnection");
            string ConnectionString = _config.GetConnectionString("DbConnection");
            SqlDataReader reader;
            using (SqlConnection co = new SqlConnection(ConnectionString)) {
                co.Open();
                using (SqlCommand command=new SqlCommand(query,co)) {
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@password", password);
                    reader = command.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    co.Close();

                }

            }


            //  var loggingUser = DummyData.Users.FirstOrDefault(user => user.UserName == userName && user.Password == password);
            // dumy data class is not dummy now ..
            var loggingUser = DummyData.user(table);
            if (loggingUser!=null) { return loggingUser; }return null;
        }
    }
}
