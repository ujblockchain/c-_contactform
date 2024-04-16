
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;


namespace ContactUJBlockchain.Pages.users
{
	public class AdminModel : PageModel
	{
		public AdminInfo adminInfo = new AdminInfo();
		public string errorMessage = "";

		public void OnGet()
		{
		}

		public void OnPost()
		{

			String name = Request.Form["name"];
			String email = Request.Form["email"];
			String password = Request.Form["password"];


			if (email.Length == 0 || name.Length == 0 || password.Length == 0)
			{
				errorMessage = "All the fields are required";
				return;
			}

			try
			{


				String connectionString = "Server=TANKISO-LTP\\SQLEXPRESS;Database=contactusDb;Trusted_Connection=True;";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					String sql = "SELECT * FROM Admin";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								AdminInfo admin = new AdminInfo();

								admin.id = reader.GetInt32(0);
								admin.name = reader.GetString(1);
								admin.email = reader.GetString(2);
								admin.password = reader.GetString(3);

								adminInfo = admin;
							}
						}
					}

				}
				

				if (email == adminInfo.email && HashPassword(password) == adminInfo.password && name == adminInfo.name)
				{
					HttpContext.Session.SetString("sessionName", "Admin");
					Response.Redirect("/users/Index");
				}
				else
				{
					errorMessage = "Wrong Credentials";
					
				}
				
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				return;
			}
		}


		//Hashing the administrator password
		private string HashPassword(string password)
		{
			string passHash = "";

			using (SHA256 sha256Hash = SHA256.Create())
			{
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				passHash = builder.ToString();
			}

			return passHash;
		}

		private void createAdmin()
		{
			adminInfo.name = Request.Form["name"];
			adminInfo.email = Request.Form["email"];
			adminInfo.password = Request.Form["password"];

			String connectionString = "Server=TANKISO-LTP\\SQLEXPRESS;Database=contactusDb;Trusted_Connection=True;";

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				String sql = "INSERT INTO Admin(name, email, password) VALUES(@name, @email, @password);";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@name", adminInfo.name);
					command.Parameters.AddWithValue("@email", adminInfo.email);
					command.Parameters.AddWithValue("@password", HashPassword(adminInfo.password.Trim()));

					command.ExecuteNonQuery();

				}

			}
		}

	}


	public class AdminInfo
	{
		public int id;
		public string name;
		public string email;
		public string password;
	}

}
