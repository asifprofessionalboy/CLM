using CLMSAPP.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System;
    using Microsoft.Data.SqlClient;
    using System.Security.Cryptography;
    using System.Text;
    using Dapper;
    using System.Net;
    using System.Net.Mail;

namespace CLMSAPP.Controllers
{
    public class UserController : Controller
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("dbcs2");
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Message = "Username and Password are required.";
                return View();
            }

            try
            {
                // Generate Salt and Hash
                var salt = GenerateSalt();
                var hashedPassword = HashPassword(password, salt);

                using (var connection = new SqlConnection(_connectionString))
                {
                    // Check if username exists
                    var existingUser = connection.QueryFirstOrDefault<UserLogin>(
                        "SELECT * FROM UserLogin WHERE Username = @Username", new { Username = username });
                    if (existingUser != null)
                    {
                        ViewBag.Message = "Username already exists.";
                        return View();
                    }

                    // Insert into UserLogins
                    var userId = Guid.NewGuid();
                    var query1 = "INSERT INTO UserLogin (Id, Username) VALUES (@Id, @Username)";
                    connection.Execute(query1, new { Id = userId, Username = username });

                    // Insert into UserLoginMembers
                    var query2 = "INSERT INTO UserLoginMember (Id, MasterId, Password, PasswordSalt) VALUES (@Id, @MasterId, @Password, @PasswordSalt)";
                    connection.Execute(query2, new
                    {
                        Id = Guid.NewGuid(),
                        MasterId = userId,
                        Password = hashedPassword,
                        PasswordSalt = salt
                    });
                }

                ViewBag.Message = "Registration successful!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

    

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Message = "Username and Password are required.";
                return View();
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Retrieve user by username
                    var user = connection.QueryFirstOrDefault<UserLogin>(
                        "SELECT * FROM UserLogin WHERE Username = @Username", new { Username = username });

                    if (user == null)
                    {
                        ViewBag.Message = "Invalid username or password.";
                        return View();
                    }

                    // Retrieve password details
                    var userDetails = connection.QueryFirstOrDefault<UserLoginMember>(
                        "SELECT * FROM UserLoginMember WHERE MasterId = @MasterId", new { MasterId = user.Id });

                    if (userDetails == null)
                    {
                        ViewBag.Message = "Invalid username or password.";
                        return View();
                    }

                    // Verify password
                    var hashedPassword = HashPassword(password, userDetails.PasswordSalt);
                    if (hashedPassword != userDetails.Password)
                    {
                        ViewBag.Message = "Invalid username or password.";
                        return View();
                    }

                    // Set session after successful login
                    HttpContext.Session.SetString("Username", user.Username);

                    // Redirect to the Index page
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View();
            }
        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear(); // Clear session
            return RedirectToAction("Login", "User"); // Redirect to login page
        }






        public IActionResult ForgotPassword()
        {

            return View();
        }


        [HttpPost]
        public IActionResult ForgotPassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Message = "Username is required.";
                return View();
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var user = connection.QueryFirstOrDefault<UserLogin>(
                        "SELECT * FROM UserLogin WHERE Username = @Username", new { Username = username });

                    if (user == null)
                    {
                        ViewBag.Message = "Username not found.";
                        return View();
                    }

                    // Generate a new password
                    string newPassword = GeneratePassword(10);

                    // Generate a new salt and hash the password
                    string newSalt = GenerateSalt();
                    string hashedPassword = HashPassword(newPassword, newSalt);

                    // Update the password and salt in the UserLoginMember table
                    connection.Execute(
                        "UPDATE UserLoginMember SET Password = @Password, PasswordSalt = @PasswordSalt WHERE MasterId = @MasterId",
                        new { Password = hashedPassword, PasswordSalt = newSalt, MasterId = user.Id });

                    // Send the new password to the user's email
                    SendEmail("asifprofessionalboy@gmail.com", "Password Reset", $"Your new password is: {newPassword}");

                    ViewBag.Message = "Success! A new password has been sent to your email.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return View();
        }





        public IActionResult ChangePassword()
        {

            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View();  
        }


        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.Message = "All fields are required.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Message = "New password and confirm password do not match.";
                return View();
            }

            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "User");
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Fetch user by username
                    var user = connection.QueryFirstOrDefault<UserLogin>(
                        "SELECT * FROM UserLogin WHERE Username = @Username", new { Username = username });

                    if (user == null)
                    {
                        ViewBag.Message = "User not found.";
                        return View();
                    }

                    // Retrieve user details for password comparison
                    var userDetails = connection.QueryFirstOrDefault<UserLoginMember>(
                        "SELECT * FROM UserLoginMember WHERE MasterId = @MasterId", new { MasterId = user.Id });

                    if (userDetails == null)
                    {
                        ViewBag.Message = "User details not found.";
                        return View();
                    }

                    // Verify current password
                    var hashedCurrentPassword = HashPassword(currentPassword, userDetails.PasswordSalt);
                    if (hashedCurrentPassword != userDetails.Password)
                    {
                        ViewBag.Message = "Current password is incorrect.";
                        return View();
                    }

                    // Hash the new password and update it
                    var newPasswordHash = HashPassword(newPassword, userDetails.PasswordSalt);
                    connection.Execute(
                        "UPDATE UserLoginMember SET Password = @Password WHERE MasterId = @MasterId",
                        new { Password = newPasswordHash, MasterId = user.Id });

                    ViewBag.Message = "Password changed successfully!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View();
            }
        }


        public IActionResult UserPermission()
        {
            return View();
        }




        private void SendEmail(string toEmail, string subject, string body)
        {
            var fromEmail = "asifprofessionalboy@gmail.com";
            var fromPassword = "dqtzrktvntnoujit"; // Your Gmail App Password (2FA-specific)

            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error sending email: {ex.Message}";
            }
        }



        private string GeneratePassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var sb = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < length; i++)
            {
                sb.Append(validChars[random.Next(validChars.Length)]);
            }

            return sb.ToString();
        }


   

    private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            var combinedPasswordSalt = password + salt;

            using (var rfc2898 = new Rfc2898DeriveBytes(combinedPasswordSalt, Encoding.UTF8.GetBytes(salt), 10000, HashAlgorithmName.SHA256))
            {
                return Convert.ToBase64String(rfc2898.GetBytes(32));
            }
        }
    }

}
