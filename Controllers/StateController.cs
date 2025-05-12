using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using AddressBook.Models;
using static AddressBook.Models.StateModel;
using System.Reflection;
using OfficeOpenXml;

namespace AddressBook.Controllers
{
    public class StateController : Controller
    {
        private IConfiguration configuration;

        public StateController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult State_Add_Edit(int StateID)
        {
            if (StateID==0)
            {
                TempData["PageTitle"] = "State Add";
            }
            else
            {
                TempData["PageTitle"] = "State Edit";
            }
            ViewBag.StateID = StateID;
            StateModel statemodel = new StateModel();
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

            SqlCommand command = connection.CreateCommand();

            command.CommandType = CommandType.StoredProcedure;

            command.CommandText = "PR_State_SelectByPK";

            command.Parameters.AddWithValue("@StateID", StateID);

            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();

            table.Load(reader);
            foreach (DataRow row in table.Rows)
            {
                statemodel.StateID = Convert.ToInt32(row["StateID"]);
                statemodel.StateName = row["StateName"].ToString();
                statemodel.StateCode = row["StateCode"].ToString();
                statemodel.UserID = Convert.ToInt32(row["UserID"]);
                statemodel.CountryID = Convert.ToInt32(row["CountryID"]);
            }
            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            SqlCommand command3 = connection.CreateCommand();
            command3.CommandType = CommandType.StoredProcedure;
            command3.CommandText = "PR_User_SelectAll";
            SqlDataReader reader3 = command3.ExecuteReader();
            DataTable dataTable3 = new DataTable();
            dataTable3.Load(reader3);
            reader3.Close();
            foreach (DataRow data in dataTable3.Rows)
            {
                UserDropDownModel userDropDownModel = new UserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;
            connection.Close();
            return View(statemodel);
        }
        public IActionResult StateSave(StateModel statemodel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (statemodel.StateID <= 0)
                {
                    command.CommandText = "PR_State_Insert";
                }
                else
                {
                    command.CommandText = "PR_State_UpdateByPK";
                    command.Parameters.Add("@StateID", SqlDbType.Int).Value = statemodel.StateID;
                }
                command.Parameters.Add("@StateName", SqlDbType.VarChar).Value = statemodel.StateName;
                command.Parameters.Add("@StateCode", SqlDbType.VarChar).Value = statemodel.StateCode;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = statemodel.UserID;
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = statemodel.CountryID;
                command.ExecuteNonQuery();
                return RedirectToAction("State_List");
            }
            return View("State_Add_Edit",statemodel);
        }
        public IActionResult StateExportToExcel()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_State_SelectAll";
            //sqlCommand.Parameters.Add("@CityID", SqlDbType.Int).Value = CityID;

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "StateID";
                worksheet.Cells[1, 2].Value = "StateName";
                worksheet.Cells[1, 3].Value = "StateCode";
                worksheet.Cells[1, 4].Value = "CreationDate";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["StateID"];
                    worksheet.Cells[row, 2].Value = item["StateName"];
                    worksheet.Cells[row, 3].Value = item["StateCode"];
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
        public IActionResult State_List()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_State_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult StateDelete(int StateID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_State_DeleteByPK";
                    command.Parameters.Add("@StateID", SqlDbType.Int).Value = StateID;


                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "State deleted successfully.";
                return RedirectToAction("State_List");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the state: " + ex.Message;
                return RedirectToAction("State_List");
            }
        }
    }
}
