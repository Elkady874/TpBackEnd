using System.Data;

namespace TPTask
{
    public class DummyData
    {
        public static List<UserData> Users = new List<UserData>() {
          //  new UserData(){UserName="suberadmin",Email="suberadmin@email.com",Password="123456",Role="suberadmin" },
            new UserData(){UserName="admin",Email="admin@email.com",Password="123456",Role="Admin" },
            new UserData(){UserName="user",Email="user@email.com",Password="123456",Role="user" }

        };
        public static UserData user(DataTable table) {
            
                //DataRow selectedRow = table.Rows;
                UserData user = new UserData();
                user.UserName = (from DataRow dr in table.Rows
                                 select (string)dr["UserName"]).FirstOrDefault();
                user.Role = (from DataRow dr in table.Rows
                             select (string)dr["Role"]).FirstOrDefault();
                user.Email = "dummy@email.com";
            if (user.UserName!=null && user.Role!=null) {
                return user;
            }
            return null;
        }
    }
}
