using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;

namespace ContactUJBlockchain.Pages.users
{
    public class CreateModel : PageModel
    {
        public UserInfo userInfo = new UserInfo();
        public string errorMessage = "";
		public string successMessage = "";

		public void OnGet()
        {
        }

        public void OnPost()
        {
            userInfo.name = Request.Form["name"];
            userInfo.email = Request.Form["email"];
            userInfo.message = Request.Form["message"];

            if (userInfo.email.Length == 0 || userInfo.name.Length == 0 || userInfo.message.Length == 0)
            {
                errorMessage = "All the fields are required";
                return;
            }

			//save data into the database

			int currentId = userExists(userInfo.email);

			if (currentId != 0)
			{
				Response.Redirect("/users/Edit?id=" + currentId);
			}
			else
			{
				try
				{
					String connectionString = "Server=TANKISO-LTP\\SQLEXPRESS;Database=contactusDb;Trusted_Connection=True;";
					using (SqlConnection connection = new SqlConnection(connectionString))
					{
						connection.Open();
						String sql = "INSERT INTO Users(name, email, message) VALUES(@name, @email, @message);";

						using (SqlCommand command = new SqlCommand(sql, connection))
						{
							command.Parameters.AddWithValue("@name", userInfo.name);
							command.Parameters.AddWithValue("@email", userInfo.email);
							command.Parameters.AddWithValue("@message", userInfo.message);

							command.ExecuteNonQuery();

						}
					}

				}
				catch (Exception ex)
				{
					errorMessage = ex.Message;

				}
				finally
				{
					try
					{
						var email = new MimeMessage();
						email.From.Add(MailboxAddress.Parse("your email"));
						email.To.Add(MailboxAddress.Parse(userInfo.email));
						email.Subject = "UJ Blockchain";
						String body = "We have received your message,  thank you for contacting UJ Blockchain";
						email.Body = new TextPart(TextFormat.Html) { Text = body };


						using var smtp = new MailKit.Net.Smtp.SmtpClient();
						smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
						smtp.Authenticate("your email", "your password");
						smtp.Send(email);
						smtp.Disconnect(true);

					}
					catch (Exception ex)
					{
						errorMessage = ex.Message;
					}

				}

				userInfo.name = ""; userInfo.email = ""; userInfo.message = "";
				if(errorMessage.Length !> 0)
				{
					successMessage = "Message has been successfully delivered, thank you.";

				}				

			}

        }


		private int userExists(string userEmail)
		{
			try
			{
				String connectionString = "Server=TANKISO-LTP\\SQLEXPRESS;Database=contactusDb;Trusted_Connection=True;";

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					String sql = "SELECT * FROM Users";
					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								if (userEmail == reader.GetString(2))
								{
									return reader.GetInt32(0);
								}
							}
						}
					}

				}
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return 0;

		}
	}
}
