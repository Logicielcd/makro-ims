using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace Makro.IMS.InterfaceSup
{
    internal class InterfaceSupplierService
    {

        private string imsDb;
        private string wmsDb;
        private string logPath;

        public InterfaceSupplierService() {           
            try
            {
                var config = new ConfigurationBuilder()
                  .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                  .AddJsonFile("appsettings.json")
                  .Build();

                imsDb = config.GetConnectionString("IMSConnection");
                wmsDb = config.GetConnectionString("WMSConnection");
                logPath = config.GetConnectionString("Log");

                DataTable dt = GetSupplier();
                SaveSupplier(dt);
                InterfaceSupplier();
            }
            catch(Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(logPath + "log.txt", true))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }

        public DataTable GetSupplier()
        {
            try
            {                
                string sqlCmd = "CDC_SP003_INB_BOOKING_GET_SUPPLIER";

                OracleConnection connection = new OracleConnection(wmsDb);

                connection.Open();

                OracleCommand command = connection.CreateCommand();
                command.CommandText = sqlCmd;
                command.CommandType = System.Data.CommandType.StoredProcedure;

                var vCursor = new OracleParameter("v_cursor", OracleDbType.RefCursor, ParameterDirection.Output);

                command.Parameters.Add(vCursor);

                OracleDataReader oracleDataReader = command.ExecuteReader();

                DataTable dt = new DataTable();

                dt.Load(oracleDataReader);

                connection.Close();
                connection.Dispose();

                return dt;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
                      
        }

        public void SaveSupplier(DataTable dt)
        {
            try
            {
                string sqlCmd = "";
                string sqlInsert = "";
                string sqlCmdExec = "";

                sqlCmd = "insert into tmp_sup_data (sup_code,sup_name) values ({0})";

                OracleConnection connection = new OracleConnection(imsDb);

                connection.Open();


                foreach (DataRow row in dt.Rows)
                {
                    sqlInsert = "'" + row["SUP_CODE"] + "',";
                    sqlInsert += "'" + row["SUP_NAME"] + "'";
                    
                    sqlCmdExec = sqlCmd.Replace("{0}", sqlInsert);

                    OracleCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = sqlCmdExec;
                    
                    command.ExecuteNonQuery();
                }

                connection.Close();
                connection.Dispose();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void InterfaceSupplier()
        {
            try
            {
                string sqlCmd = "SUP_INTERFACE";

                OracleConnection connection = new OracleConnection(imsDb);

                connection.Open();

                OracleCommand command = connection.CreateCommand();
                command.CommandText = sqlCmd;
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.ExecuteNonQuery();
                
                connection.Close();
                connection.Dispose();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
