using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net;
using System.Net.Mail;

namespace ContactUJBlockchain.Pages.users
{
    public class EditModel : PageModel
    {
		public UserInfo userInfo = new UserInfo();
		public String errorMessage = "";
        public String successMessage = "";

		public void OnGet()
        {
           int id = int.Parse(Request.Query["id"]);

            try
            {
				String connectionString = "Server=TANKISO-LTP\\SQLEXPRESS;Database=contactusDb;Trusted_Connection=True;";
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Users WHERE id=@id;";
                    using(SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                userInfo.id =  reader.GetInt32(0);
                                userInfo.name = reader.GetString(1);
                                userInfo.email = reader.GetString(2); 
                                userInfo.message = reader.GetString(3);
                            }
                        }
                    }
                }

			}
            catch(Exception ex)
            {
                successMessage = ex.Message;
            }

        }

        public void OnPost() 
        {
            userInfo.id = int.Parse(Request.Form["id"]);
            userInfo.name = Request.Form["name"];
            userInfo.email = Request.Form["email"];
			userInfo.message = Request.Form["message"];

			if (userInfo.name.Length == 0 || userInfo.email.Length == 0 || userInfo.message.Length == 0)
            {
                errorMessage = "All fields are required";
                return;
            }

            try
            {
				String connectionString = "Server=TANKISO-LTP\\SQLEXPRESS;Database=contactusDb;Trusted_Connection=True;";
				using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE Users SET name=@name, email=@email, message=@message WHERE id=@id";

                    using (SqlCommand command = new SqlCommand(sql, connection)) 
                    {
						command.Parameters.AddWithValue("@id", userInfo.id);
						command.Parameters.AddWithValue("@name", userInfo.name);
                        command.Parameters.AddWithValue("@email", userInfo.email);
                        command.Parameters.AddWithValue("@message", userInfo.message);

                        command.ExecuteNonQuery();
					}
                }
            }
            catch(Exception ex) 
            {
                errorMessage = ex.Message;
                return;
            }

			if(HttpContext.Session.GetString("sessionName") == "Admin")
            {
                Response.Redirect("/users/Index");
            }

			successMessage = "Data has been successfully edited";
        }
    }
}
 