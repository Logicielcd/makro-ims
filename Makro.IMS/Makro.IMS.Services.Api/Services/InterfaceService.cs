using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Reflection;

namespace Makro.IMS.Services.Api.Services
{
    public class InterfaceService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;

        public InterfaceService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
        }

        public async Task<bool> ImportPO(PoDtos po)
        {
            try
            {
                var config = new ConfigurationBuilder()
                 .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                 .AddJsonFile("appsettings.json")
                 .Build();

                var imsDb = config.GetConnectionString("DefaultConnection");
                string sqlCmd = "";
                string sqlInsert = "";
                string sqlCmdExec = "";

                sqlCmd = "insert into tmp_po_data (internal_po_no,company_code,warehouse_code,po_nbr,sup_code,merch_type,total_qty,plan_receive_date,create_date,remark,"
                    + "booker,full,full_cs,non,con,cube_full,cube_con,cube_non,weight,company_name,warehouse_name,user_def1,user_def2,user_def3,user_def4,expire_date) values ({0})";

                OracleConnection connection = new OracleConnection(imsDb);

                connection.Open();


                foreach (PoDto row in po.Pos)
                {
                    sqlInsert = "'" + row.InternalPoNo.ToString() + "',";
                    sqlInsert += "'" + row.CompanyCode + "',";
                    sqlInsert += "'" + row.WarehouseCode + "',";
                    sqlInsert += "'" + row.PoNo + "',";
                    sqlInsert += "'" + row.SupCode + "',";
                    sqlInsert += "'" + row.MerchType + "',";
                    sqlInsert += "'" + row.TotalQty.ToString() + "',";
                    sqlInsert += "TO_DATE('" + row.PlanReceiveDate.ToString("yyyy-MM-dd hh:mm:ss") + "','yyyy-mm-dd hh:mi:ss'),";
                    sqlInsert += "TO_DATE('" + row.CreateDate.ToString("yyyy-MM-dd hh:mm:ss") + "','yyyy-mm-dd hh:mi:ss'),";
                    sqlInsert += "'" + row.Remark + "',";
                    sqlInsert += "'" + row.Booker.ToString() + "',";
                    sqlInsert += "'" + row.FullPl.ToString() + "',";
                    sqlInsert += "'" + row.Full.ToString() + "',";
                    sqlInsert += "'" + row.Non.ToString() + "',";
                    sqlInsert += "'" + row.Con.ToString() + "',";
                    sqlInsert += "'" + row.CubeFull.ToString() + "',";
                    sqlInsert += "'" + row.CubeCon.ToString() + "',";
                    sqlInsert += "'" + row.CubeNon.ToString() + "',";
                    sqlInsert += "'" + row.Weight.ToString() + "',";
                    sqlInsert += "'" + row.CompanyName + "',";
                    sqlInsert += "'" + row.WarehouseName + "',";
                    sqlInsert += "'" + row.UserDef1 + "',";
                    sqlInsert += "'" + row.UserDef2 + "',";
                    sqlInsert += "'" + row.UserDef3 + "',";
                    sqlInsert += "'" + row.UserDef4 + "',";
                    sqlInsert += "TO_DATE('" + row.ExpireDate.ToString("yyyy-MM-dd hh:mm:ss") + "','yyyy-mm-dd hh:mi:ss') ";

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

            return true;
        }


    }
}
