using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using AddressBook.Models;
using static AddressBook.Models.CityModel;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace AddressBook.Controllers
{
    public class CityController : Controller
    {
        private IConfiguration configuration;

        public CityController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult City_Add_Edit(int CityID)
        {
            if (CityID==0)
            {
                TempData["PageTitle"] = "City Add";
            }
            else
            {
                TempData["PageTitle"] = "City Edit";
            }
            ViewBag.CityID = CityID;
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command2 = connection.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_Country_SelectAll";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            reader2.Close();

            List<CountryDropDownModel> countryList = new List<CountryDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                CountryDropDownModel countryDropDownModel = new CountryDropDownModel();
                countryDropDownModel.CountryID = Convert.ToInt32(data["CountryID"]);
                countryDropDownModel.CountryName = data["CountryName"].ToString();
                countryList.Add(countryDropDownModel);
            }
            ViewBag.CountryList = countryList;

            SqlCommand command3 = connection.CreateCommand();
            command3.CommandType = System.Data.CommandType.StoredProcedure;
            command3.CommandText = "PR_State_SelectAll";
            SqlDataReader reader3 = command3.ExecuteReader();
            DataTable dataTable3 = new DataTable();
            dataTable3.Load(reader3);
            reader3.Close();

            List<StateDropDownModel> stateList = new List<StateDropDownModel>();
            foreach (DataRow data in dataTable3.Rows)
            {
                StateDropDownModel stateDropDownModel = new StateDropDownModel();
                stateDropDownModel.StateID = Convert.ToInt32(data["StateID"]);
                stateDropDownModel.StateName = data["StateName"].ToString();
                stateList.Add(stateDropDownModel);
            }
            ViewBag.StateList = stateList;
            SqlCommand command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;

            command.CommandText = "PR_City_SelectByPK";

            command.Parameters.AddWithValue("@CityID", CityID);

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();

            table.Load(reader);
            CityModel citymodel = new CityModel();
            foreach (DataRow row in table.Rows)
            {
                citymodel.CityID = Convert.ToInt32(row["CityID"]);
                citymodel.CityName = row["CityName"].ToString();
                citymodel.STDCode = row["STDCode"].ToString();
                citymodel.PinCode = row["PinCode"].ToString();
                citymodel.UserID = Convert.ToInt32(row["UserID"]);
                citymodel.CountryID = Convert.ToInt32(row["CountryID"]);
                citymodel.StateID = Convert.ToInt32(row["StateID"]);
            }
            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            SqlCommand command1 = connection.CreateCommand();
            command1.CommandType = CommandType.StoredProcedure;
            command1.CommandText = "PR_User_SelectAll";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            foreach (DataRow data in dataTable1.Rows)
            {
                UserDropDownModel userDropDownModel = new UserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;
            return View(citymodel);
        }
        public IActionResult CitySave(CityModel citymodel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (citymodel.CityID <= 0)
                {
                    command.CommandText = "PR_City_Insert";
                }
                else
                {
                    command.CommandText = "PR_City_UpdateByPK";
                    command.Parameters.Add("@CityID", SqlDbType.Int).Value = citymodel.CityID;
                }
                command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = citymodel.CityName;
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = citymodel.CountryID;
                command.Parameters.Add("@StateID", SqlDbType.Int).Value = citymodel.StateID;
                command.Parameters.Add("@STDCode", SqlDbType.VarChar).Value = citymodel.STDCode;
                command.Parameters.Add("@PinCode", SqlDbType.VarChar).Value = citymodel.PinCode;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = citymodel.UserID;
                command.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction("City_List");
            }
            return View("City_Add_Edit",citymodel);
        }
        public IActionResult CityExportToExcel()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_City_SelectAll";
            //sqlCommand.Parameters.Add("@CityID", SqlDbType.Int).Value = CityID;

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "CityID";
                worksheet.Cells[1, 2].Value = "CityName";
                worksheet.Cells[1, 3].Value = "STDCode";
                worksheet.Cells[1, 4].Value = "PinCode";
                worksheet.Cells[1, 5].Value = "CreationDate";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["CityID"];
                    worksheet.Cells[row, 2].Value = item["CityName"];
                    worksheet.Cells[row, 3].Value = item["STDCode"];
                    worksheet.Cells[row, 4].Value = item["PinCode"];
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
        public IActionResult City_List(int? CityID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_City_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult CityDelete(int CityID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_City_DeleteByPK";
                    command.Parameters.Add("@CityID", SqlDbType.Int).Value = CityID;


                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "City deleted successfully.";
                return RedirectToAction("City_List");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the city: " + ex.Message;
                return RedirectToAction("City_List");
            }
        }
    }
}
