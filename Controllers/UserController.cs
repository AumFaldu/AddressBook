using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using AddressBook.Models;
using System.Reflection;
using OfficeOpenXml;

namespace AddressBook.Controllers
{
    public class UserController : Controller
    {
        private IConfiguration configuration;

        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Registration(int UserID)
        {
            UserModel usermodel = new UserModel();
            if (UserID == null || UserID<=0)
            {
                TempData["PageTitle"] = "User Registration";
                return View();
            }
            TempData["PageTitle"] = "User Edit";

            ViewBag.UserID = UserID;
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;

            command.CommandText = "PR_User_SelectByPK";

            command.Parameters.AddWithValue("@UserID", UserID);

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();

            table.Load(reader);
            foreach (DataRow row in table.Rows)
            {
                usermodel.UserID = Convert.ToInt32(row["UserID"]);
                usermodel.UserName = row["UserName"].ToString();
                usermodel.MobileNo = row["MobileNo"].ToString();
                usermodel.EmailID = row["EmailID"].ToString();
            }

            return View(usermodel);
        }
        public IActionResult UserSave(UserModel usermodel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (usermodel.UserID <= 0)
                {
                    command.CommandText = "PR_User_Insert";
                }
                else
                {
                    command.CommandText = "PR_User_UpdateByPK";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = usermodel.UserID;
                }
                command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = usermodel.UserName;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = usermodel.MobileNo;
                command.Parameters.Add("@EmailID", SqlDbType.VarChar).Value = usermodel.EmailID;
                command.ExecuteNonQuery();
                return RedirectToAction("User_List");
            }
            return View("Registration", usermodel);
        }
        public IActionResult UserLogin(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_ValidateLogin";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    sqlCommand.Parameters.Add("@EmailID", SqlDbType.VarChar).Value = userLoginModel.EmailID;
                    sqlCommand.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userLoginModel.MobileNo;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                            HttpContext.Session.SetString("MobileNo", dr["MobileNo"].ToString());
                            HttpContext.Session.SetString("EmailAddress", dr["EmailID"].ToString());
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "User is not found";
                        return RedirectToAction("Login", "User");
                    }

                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        public IActionResult UserExportToExcel()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_User_SelectAll";
            //sqlCommand.Parameters.Add("@CityID", SqlDbType.Int).Value = CityID;

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "UserID";
                worksheet.Cells[1, 2].Value = "UserName";
                worksheet.Cells[1, 3].Value = "MobileNo";
                worksheet.Cells[1, 4].Value = "EmailID";
                worksheet.Cells[1, 5].Value = "CreationDate";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["UserID"];
                    worksheet.Cells[row, 2].Value = item["UserName"];
                    worksheet.Cells[row, 3].Value = item["MobileNo"];
                    worksheet.Cells[row, 4].Value = item["EmailID"];
                    worksheet.Cells[row, 5].Value = item["CreationDate"];
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Data-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
        public IActionResult User_List()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_User_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult UserDelete(int UserID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_User_DeleteByPK";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;


                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction("User_List");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the user: " + ex.Message;
                return RedirectToAction("User_List");
            }
        }
    }
}
