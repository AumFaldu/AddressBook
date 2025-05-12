using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using AddressBook.Models;
using static AddressBook.Models.CountryModel;
using System.Reflection;
using OfficeOpenXml;

namespace AddressBook.Controllers
{
    public class CountryController : Controller
    {
        private IConfiguration configuration;
        public CountryController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult Country_Add_Edit(int CountryID)
        {
            if (CountryID == 0)
            {
                TempData["PageTitle"] = "Country Add";
            }
            else
            {
                TempData["PageTitle"] = "Country Edit";
            }
            ViewBag.CountryID = CountryID;

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
           
            SqlConnection connection = new SqlConnection(connectionString);
            
            connection.Open();
            
            SqlCommand command = connection.CreateCommand();
            
            command.CommandType = CommandType.StoredProcedure;

            command.CommandText = "PR_Country_SelectByPK";

            command.Parameters.AddWithValue("@CountryID", CountryID);

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();

            table.Load(reader);

            CountryModel countrymodel = new CountryModel();

            foreach(DataRow row in table.Rows)
            {
                countrymodel.CountryID = Convert.ToInt32(row["CountryID"]);
                countrymodel.CountryName = row["CountryName"].ToString();
                countrymodel.CountryCode = row["CountryCode"].ToString();
                countrymodel.UserID = Convert.ToInt32(row["UserID"]);
            }
            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            SqlCommand command2 = connection.CreateCommand();
            command2.CommandType = CommandType.StoredProcedure;
            command2.CommandText = "PR_User_SelectAll";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            foreach (DataRow row in dataTable2.Rows)
            {
                UserDropDownModel user = new UserDropDownModel();
                user.UserID = Convert.ToInt32(row["UserID"]);
                user.UserName = row["UserName"].ToString();
                userList.Add(user);
            }
            ViewBag.UserList = userList;
            return View(countrymodel);
        }
        public IActionResult CountrySave(CountryModel countrymodel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (countrymodel.CountryID <= 0)
                {
                    command.CommandText = "PR_Country_Insert";
                }
                else
                {
                    command.CommandText = "PR_Country_UpdateByPK";
                    command.Parameters.Add("@CountryID", SqlDbType.Int).Value = countrymodel.CountryID;
                }
                    command.Parameters.Add("@CountryName", SqlDbType.VarChar).Value = countrymodel.CountryName;
                    command.Parameters.Add("@CountryCode", SqlDbType.VarChar).Value = countrymodel.CountryCode;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = countrymodel.UserID;
                    command.ExecuteNonQuery();
                return RedirectToAction("Country_List");
            }
            return View("Country_Add_Edit",countrymodel);
        }
        public IActionResult CountryExportToExcel()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Country_SelectAll";
            //sqlCommand.Parameters.Add("@CityID", SqlDbType.Int).Value = CityID;

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "CountryID";
                worksheet.Cells[1, 2].Value = "CountryName";
                worksheet.Cells[1, 3].Value = "CountryCode";
                worksheet.Cells[1, 4].Value = "CreationDate";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["CountryID"];
                    worksheet.Cells[row, 2].Value = item["CountryName"];
                    worksheet.Cells[row, 3].Value = item["CountryCode"];
                    worksheet.Cells[row, 4].Value = item["CreationDate"];
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Data-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
        public IActionResult Country_List()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Country_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }

        public IActionResult CountryDelete(int CountryID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Country_DeleteByPK";
                    command.Parameters.Add("@CountryID", SqlDbType.Int).Value = CountryID;


                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Country deleted successfully.";
                return RedirectToAction("Country_List");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the country: " + ex.Message;
                return RedirectToAction("Country_List");
            }
        }
    }
}
