using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using Newtonsoft.Json;

namespace TPTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IConfiguration _config;
        public UsersController(IConfiguration config)
        {
            _config = config;

        }
        [HttpGet("Role")]
        [Authorize]
        public IActionResult GetRole() {
            var currentUser = GetCurrentUser();
            return Ok(currentUser.Role);
                }
        [HttpPost("Submit")]
        [Authorize]
        public IActionResult SubmitData(string FullName,string Email,string Phone ,string Country)
        {
            var currentUser = GetCurrentUser();
            var query = @"insert into dbo.Data (CreatedBy,FullName,Email,Phone,Country) values (@CreatedBy,@FullName,@Email,@Phone,@Country) ";
            DataTable table = new DataTable();
            string ConnectionString = _config.GetConnectionString("DbConnection");
            SqlDataReader reader;
            using (SqlConnection co = new SqlConnection(ConnectionString))
            {
                co.Open();
                using (SqlCommand command = new SqlCommand(query, co))
                {
                    command.Parameters.AddWithValue("@CreatedBy", currentUser.UserName);
                    command.Parameters.AddWithValue("@FullName", FullName);
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Country", Country);
                    reader = command.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    co.Close();

                }

            }
           
            return Ok(currentUser.Role);
        }
        [HttpDelete("Delete")]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteData(int Id)
        {
            var currentUser = GetCurrentUser();
            var query = @"delete from dbo.Data  where Id=@ID";
            DataTable table = new DataTable();
            string ConnectionString = _config.GetConnectionString("DbConnection");
            SqlDataReader reader;
            using (SqlConnection co = new SqlConnection(ConnectionString))
            {
                co.Open();
                using (SqlCommand command = new SqlCommand(query, co))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    command.Parameters.AddWithValue("@CreatedBy", currentUser.UserName);
                   
                    reader = command.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    co.Close();

                }

            }

            return Ok(currentUser.Role);
        }
        [HttpPut("Edit")]
        [Authorize(Roles = "admin")]
        public IActionResult updateData(int Id,string FullName, string Email, string Phone, string Country)
        {
            var currentUser = GetCurrentUser();
            var query = @"update dbo.Data set CreatedBy=@CreatedBy,FullName=@FullName,Email=@Email,Phone=@Phone,Country=@Country where Id=@ID ";
            DataTable table = new DataTable();
            string ConnectionString = _config.GetConnectionString("DbConnection");
            SqlDataReader reader;
            using (SqlConnection co = new SqlConnection(ConnectionString))
            {
                co.Open();
                using (SqlCommand command = new SqlCommand(query, co))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    command.Parameters.AddWithValue("@CreatedBy", currentUser.UserName);
                    command.Parameters.AddWithValue("@FullName", FullName);
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Country", Country);
                    reader = command.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    co.Close();

                }

            }

            return Ok(currentUser.Role);
        }
        [HttpGet("Submited")]
        [Authorize(Roles ="admin")]
        public IActionResult GetData()
        {
            var currentUser = GetCurrentUser();
            var query = @"select * from dbo.Data";
            DataTable table = new DataTable();
            string ConnectionString = _config.GetConnectionString("DbConnection");
            SqlDataReader reader;
            using (SqlConnection co = new SqlConnection(ConnectionString))
            {
                co.Open();
                using (SqlCommand command = new SqlCommand(query, co))
                {
                   
                    reader = command.ExecuteReader();
                    table.Load(reader);
                    reader.Close();
                    co.Close();

                }

            }
            return Ok(JsonConvert.SerializeObject(table));
            //return new JsonResult(table);
        }
        private UserData GetCurrentUser() {
            var id = HttpContext.User.Identity as ClaimsIdentity;
            if (id!=null) {
                var claims = id.Claims;
                return new UserData {
                    UserName = claims.FirstOrDefault(user=>user.Type==ClaimTypes.NameIdentifier)?.Value,
                    Email = claims.FirstOrDefault(user => user.Type == ClaimTypes.Email)?.Value,
                    Role = claims.FirstOrDefault(user => user.Type == ClaimTypes.Role)?.Value


            };
            }
            return null;
        }
    }
}
