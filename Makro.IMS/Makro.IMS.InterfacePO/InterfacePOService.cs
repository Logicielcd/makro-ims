using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace Makro.IMS.InterfacePO
{
    internal class InterfacePOService
    {

        private string imsDb;
        private string wmsDb;
        private string logPath;

        public InterfacePOService() {           
            try
            {
                var config = new ConfigurationBuilder()
                  .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                  .AddJsonFile("appsettings.json")
                  .Build();

                imsDb = config.GetConnectionString("IMSConnection");
                wmsDb = config.GetConnectionString("WMSConnection");
                logPath = config.GetConnectionString("Log");

                DataTable dt = GetPO();
                SavePO(dt);
                InterfacePO();
            }
            catch(Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(logPath + "log.txt", true))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }

        public DataTable GetPO()
        {
            try
            {                
                string sqlCmd = "CDC_SP001_INB_BOOKING_GET_PO";

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

        public void SavePO(DataTable dt)
        {
            try
            {
                string sqlCmd = "";
                string sqlInsert = "";
                string sqlCmdExec = "";

                sqlCmd = "insert into tmp_po_data (internal_po_no,company_code,warehouse_code,po_nbr,sup_code,merch_type,total_qty,plan_receive_date,create_date,remark,"
                    + "booker,full,non,con,cube_full,cube_con,cube_non,weight,company_name,warehouse_name,full_cs,half_cs,half,expire_date) values ({0})";

                OracleConnection connection = new OracleConnection(imsDb);

                connection.Open();


                foreach (DataRow row in dt.Rows)
                {
                    sqlInsert = "'" + row["INTERNAL_PO_NO"] + "',";
                    sqlInsert += "'" + row["COMPANY_CODE"] + "',";
                    sqlInsert += "'" + row["WAREHOUSE_CODE"] + "',";
                    sqlInsert += "'" + row["PO_NBR"] + "',";
                    sqlInsert += "'" + row["SUP_CODE"] + "',";
                    sqlInsert += "'" + row["MERCH_TYPE"] + "',";
                    sqlInsert += "'" + row["TOTAL_QTY"] + "',";
                    sqlInsert += "TO_DATE('" + Convert.ToDateTime(row["PLAN_RECEIVE_DATE"]).ToString("yyyy-MM-dd hh:mm:ss") + "','yyyy-mm-dd hh:mi:ss'),";
                    sqlInsert += "TO_DATE('" + Convert.ToDateTime(row["CREATE_DATE"]).ToString("yyyy-MM-dd hh:mm:ss") + "','yyyy-mm-dd hh:mi:ss'),";
                    sqlInsert += "'" + row["REMARK"] + "',";
                    sqlInsert += "'" + row["BOOKER"] + "',";
                    sqlInsert += "'" + row["FULL"] + "',";
                    sqlInsert += "'" + row["NON"] + "',";
                    sqlInsert += "'" + row["CON"] + "',";
                    sqlInsert += "'" + row["CUBE_FULL"] + "',";
                    sqlInsert += "'" + row["CUBE_CON"] + "',";
                    sqlInsert += "'" + row["CUBE_NON"] + "',";
                    sqlInsert += "'" + row["WEIGHT"] + "',";
                    sqlInsert += "'" + row["COMPANY_NAME"] + "',";
                    sqlInsert += "'" + row["WAREHOUSE_NAME"] + "',";                    
                    sqlInsert += "'" + row["FULL_CS"] + "',";
                    sqlInsert += "'" + row["HALF_CS"] + "',";
                    sqlInsert += "'" + row["HALF"] + "',";
                    sqlInsert += "TO_DATE('" + Convert.ToDateTime(row["EXPIRE_DATE"]).ToString("yyyy-MM-dd hh:mm:ss") + "','yyyy-mm-dd hh:mi:ss')";

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

        public void InterfacePO()
        {
            try
            {
                string sqlCmd = "MT_PO_LIST_INTERFACE";

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
