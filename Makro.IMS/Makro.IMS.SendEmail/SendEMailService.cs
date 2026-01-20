using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.SendEmail
{
    internal class SendEMailService
    {
        private string imsDb;
        private string logPath;
        private string smtp;
        private int port;
        private string fromMail;
        private string ccMail;

        public SendEMailService()
        {
            try
            {
                var config = new ConfigurationBuilder()
                  .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                  .AddJsonFile("appsettings.json")
                  .Build();

                imsDb = config.GetConnectionString("IMSConnection");

                var logSection = config.GetSection("Log");
                logPath = logSection["LogPath"];

                var apiSection = config.GetSection("Mail");

                smtp = apiSection["Smtp"].ToString();
                port = Convert.ToInt32(apiSection["Port"]);
                fromMail = apiSection["FromMail"].ToString();
                ccMail = apiSection["CcMail"].ToString();

                DataTable dt1 = GetPoNoBook(7);

                SendMailPoNoBook(dt1, "[ติดตามครั้งที่ 1] ข้อมูล PO ที่ยังไม่ได้ทำการนัดหมายลงสินค้า", "เงื่อนไขการแจ้งเตือน\r\n1. PO ที่จะครบกำหนดส่งภายใน 7 วัน");

                DataTable dt2 = GetPoNoBook(2);

                SendMailPoNoBook(dt2, "[ติดตามครั้งที่ 2] ข้อมูล PO ที่ยังไม่ได้ทำการนัดหมายลงสินค้า", "เงื่อนไขการแจ้งเตือน\r\n1. PO ที่จะครบกำหนดส่งภายใน 2 วัน");

                DataTable dt3 = GetPoNoBook(-2);

                SendMailPoNoBook(dt3, "[ติดตามครั้งที่ 3] ข้อมูล PO ที่ยังไม่ได้ทำการนัดหมายลงสินค้า", "เงื่อนไขการแจ้งเตือน\r\n1. PO ที่เกินกำหนดส่งแล้ว 2 วัน");

                DataTable dt4 = GetPoNoBook(-4);

                SendMailPoNoBook(dt4, "[ติดตามครั้งที่ 4] ข้อมูล PO ที่ยังไม่ได้ทำการนัดหมายลงสินค้า", "เงื่อนไขการแจ้งเตือน\r\n1. PO ที่เกินกำหนดส่งแล้ว 4 วัน");

            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(logPath + "log-email.txt", true))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }


        public DataTable GetPoNoBook(int dayNo)
        {
            try
            {
                string sqlCmd = "RPT_PO_NOT_BOOKING";

                OracleConnection connection = new OracleConnection(imsDb);

                connection.Open();

                OracleCommand command = connection.CreateCommand();
                command.CommandText = sqlCmd;
                command.CommandType = System.Data.CommandType.StoredProcedure;

                var vDayNo = new OracleParameter("v_date_no", OracleDbType.Int64, ParameterDirection.Input);
                vDayNo.Value = dayNo;

                var vCursor = new OracleParameter("v_cursor", OracleDbType.RefCursor, ParameterDirection.Output);

                command.Parameters.Add(vDayNo);
                command.Parameters.Add(vCursor);
                

                OracleDataReader oracleDataReader = command.ExecuteReader();

                DataTable dt = new DataTable();

                dt.Load(oracleDataReader);

                connection.Close();
                connection.Dispose();

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public void SendMailPoNoBook(DataTable dt,string subject,string footer)
        {
            List<string> supGroups = dt.Rows.OfType<DataRow>().Select(x => x["internal_sup_group_id"].ToString()).ToList();

            supGroups = supGroups.Distinct().ToList();
            try
            {
                foreach (var supGroup in supGroups)
                {
                    var drPos = dt.Select("internal_sup_group_id = " + supGroup.ToString());
                    string body = GenerateHtmlTable(drPos,footer);

                    SendEmail(drPos[0]["contact_e_mail"].ToString(), body, subject + " - " + drPos[0]["sup_name"]);
                    //SendEmail("chitisan@logicielcd.com", body, subject + " - " + drPos[0]["sup_name"]);
                }
            } 
            catch(Exception ex)
            {

            }
        }

        private string GenerateHtmlTable(DataRow[] dr,string footer)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"
            <!DOCTYPE html>
            <html lang='th'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>PO Booking Reminder</title>
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        line-height: 1.6;
                        color: #333333;
                    }
                    .container {
                        margin: 20px auto;
                        padding: 20px;
                        max-width: 1200px;
                        border: 1px solid #dddddd;
                        border-radius: 8px;
                        background-color: #f9f9f9;
                    }
                    h1 {
                        font-size: 20px;
                        color: #004085;
                    }
                    p {
                        font-size: 16px;
                    }
                    .highlight {
                        color: #d9534f;
                        font-weight: bold;
                    }
                    .cta {
                        margin: 20px 0;
                        
                    }
                    .cta a {
                        background-color: #007bff;
                        color: white;
                        padding: 12px 24px;
                        border-radius: 6px;
                        text-decoration: none;
                        font-size: 16px;
                    }
                    .cta a:hover {
                        background-color: #0056b3;
                    }
                    .footer {
                        margin-top: 30px;
                        font-size: 14px;         
                        color: red;
                    }
                    table {
                        width: 100%;
                        border-collapse: collapse;
                    }
                    table th, table td {
                        padding: 2px;
                        border: 1px solid #ddd;
                        text-align: center;
                        font-size: 12px;
                    }
                    table th {
                        background-color: #f2f2f2;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>เรียน " + dr[0]["sup_name"].ToString() + @"</h1>
                    <p style='text-indent: 40px;'>
                        รบกวนทางซัพพลายเออร์ตรวจสอบ PO ตามรายละเอียดด้านล่าง ที่ท่านยังไม่ได้ทำการนัดหมาย Booking วันจัดส่งสินค้า 
                        ที่จะจัดส่งสินค้าให้กับทางคลังสินค้าอาหารแห้ง บริษัท ซีพี แอ็กซ์ตร้า จำกัด (มหาชน)  กรุณาช่วยตรวจสอบรายละเอียดที่แจ้งด้านล่าง                         
                    </p>
                    <p>
                        
                        <div class='cta'>
                            โดยสามารถ Booking ผ่านระบบ IMS เพื่อทำการนัดหมายส่งสินค้าตามเงื่อนไขที่กำหนดด้วยคะ
                            <a href='https://bookingprd.siammakro.co.th/ims/auth/login' target='_blank'>
                                นัดหมายการส่งสินค้า
                            </a>
                        </div>                        
                    </p>
                    <table>
                        <tr><th>Warehouse Code</th><th>Company Code</th><th>Sup Code</th><th>Sup Name</th><th>Po No</th><th>Qty (CS)</th><th>Plan Received Date</th><th>Expired Date</th></tr>"
            );

            foreach (DataRow po in dr)
            {
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5:C}</td><td>{6}</td><td>{7}</td></tr>",
                    po["warehouse_code"].ToString(), po["company_code"].ToString(), po["sup_code"].ToString(),
                    po["sup_name"].ToString(), po["po_nbr"].ToString(), po["totalQty"].ToString(),
                    Convert.ToDateTime(po["plan_receive_date"]).ToString("dd/MM/yyyy"), Convert.ToDateTime(po["expire_date"]).ToString("dd/MM/yyyy")
                    );
            }

            sb.AppendFormat(@"
                    </table>                                       
                    <div class='footer'>
                        {0}
                    </div>
                </div>
            </body>
            </html>",footer);

            /*
            sb.AppendFormat("<h2>เรียน {0} </h2>", dr[0]["sup_name"].ToString());
            sb.Append("<p>รบกวนทางซัพพลายเออร์ตรวจสอบ PO ตามรายละเอียดด้านล่าง ที่ท่านยังไม่ได้ทำการนัดหมาย Booking วันจัดส่งสินค้า ที่จะจัดส่งสินค้าให้กับทางคลังสินค้าอาหารแห้ง บริษัท ซีพี แอ็กซ์ตร้า จำกัด (มหาชน)");
            sb.Append("กรุณาช่วยตรวจสอบรายละเอียดที่แจ้งด้านล่าง <br>โดยแจ้งสามารถ Booking ผ่านระบบ IMS :<a href='https://bookingprd.siammakro.co.th/ims/auth/login' target='_blank'> นัดหมายการส่งสินค้า </a> เพื่อทำการนัดหมายส่งสินค้าตามเงื่อนไขที่กำหนดด้วยคะ</p>");
            sb.Append("<table border='1' cellpadding='5' cellspacing='0'>");
            sb.Append("<tr><th>Warehouse Code</th><th>Company Code</th><th>Sup Code</th><th>Sup Name</th><th>Po No</th><th>Qty (CS)</th><th>Plan Received Date</th><th>Expired Date</th></tr>");

            foreach (DataRow po in dr)
            {
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5:C}</td><td>{6}</td><td>{7}</td></tr>",
                    po["warehouse_code"].ToString(), po["company_code"].ToString(), po["sup_code"].ToString(),
                    po["sup_name"].ToString(), po["po_nbr"].ToString(),po["totalQty"].ToString(),
                    Convert.ToDateTime(po["plan_receive_date"]).ToString("dd/MM/yyyy"),Convert.ToDateTime(po["expire_date"]).ToString("dd/MM/yyyy")
                    );
            }

            sb.Append("</table>");

            sb.AppendFormat("<h3>{0}</h3>",footer);
            */

            return sb.ToString();
        }

        private void SendEmail(string to, string body, string subject)
        {

            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            SmtpClient smtpServer = new SmtpClient(smtp);
            smtpServer.Port = port;

            //SmtpServer.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            // Encrypt the connection
            //SmtpServer.EnableSsl = true;

            // Callback to validate server certificate
            try
            {
                mail.From = new MailAddress(fromMail);

                foreach (var mailTo in to.Split(';'))
                {
                    mail.To.Add(mailTo);
                }

                foreach (var mailCc in ccMail.Split(';'))
                {
                    mail.CC.Add(mailCc);
                }

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                smtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                smtpServer.Dispose();
            }
        }
    }
}
