using Makro.IMS.InterfaceDHLApi.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Makro.IMS.InterfaceDHLApi
{
    internal class InterfaceDHLApiService
    {
        private string imsDb;        
        private string logPath;
        private string serverApi;
        private string userApi;
        private string passwordApi;
        private string logFileName;

        public InterfaceDHLApiService()
        {
            logFileName = "log-dhl-interface-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            try
            {
                var config = new ConfigurationBuilder()
                      .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                      .AddJsonFile("appsettings.json")
                      .Build();

                imsDb = config.GetConnectionString("IMSConnection");

                var logSection = config.GetSection("Log");

                logPath = logSection["LogPath"];

                var apiSection = config.GetSection("Api");

                serverApi = apiSection["ServerApi"].ToString();
                userApi = apiSection["UserApi"].ToString();
                passwordApi = apiSection["PasswordApi"].ToString();

                try
                {
                    DataTable dtSupGroup = GetSupGroup();
                    SendSupGroupApi(dtSupGroup);
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + ex.Message));
                    }
                }

                try
                {
                    DataTable dtPoComment = GetPoComment();
                    SendPoCommentApi(dtPoComment);
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + ex.Message));
                    }
                }

                try
                {
                    DataTable dtBooking = GetBookingInterface();
                    SendBookingApi(dtBooking);
                }
                catch(Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + ex.Message));
                }
            }
        }

        public DataTable GetSupGroup()
        {
            OracleConnection connection = new OracleConnection(imsDb);

            try
            {
                string sqlCmd = "select sg.internal_sup_group_id,sg.sup_name,address1,address2,address3,city,country,zipcode,mobile_number," +
                    "phone_number,contact_name,contact_e_mail,user_def1,user_def2,user_def3,user_def4,user_def5,user_def6," +
                    "s.sup_code,s.sup_name as sup_sup_name " +
                    "from supplier_group sg inner join supplier s on sg.internal_sup_group_id = s.internal_group_id " +
                    "where sg.interface_date < nvl(sg.mod_date, sysdate) or sg.interface_date < nvl(sg.create_date, sysdate) " +
                    "or sg.interface_date is null";
                
                connection.Open();

                OracleCommand command = connection.CreateCommand();
                command.CommandText = sqlCmd;
                command.CommandType = System.Data.CommandType.Text;

                OracleDataReader oracleDataReader = command.ExecuteReader();

                DataTable dtSupGroup = new DataTable();

                dtSupGroup.Load(oracleDataReader);

                return dtSupGroup;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }            
        }

        public async void SendSupGroupApi(DataTable dt)
        {
            int internalSupGroup = 0;
            
            var supGroups = dt.AsEnumerable()
                .GroupBy(row => new { id = row.Field<long>("internal_sup_group_id") })
                .Select(grp => grp.First())
                .ToList();

            foreach (DataRow dr in supGroups)
            {                               
                SupGroupDto supGroupDto = new SupGroupDto();
                supGroupDto.InternalSupGroupId = Convert.ToInt32(dr["INTERNAL_SUP_GROUP_ID"]);
                supGroupDto.SupName = dr["SUP_NAME"].ToString();
                supGroupDto.Address1 = dr["ADDRESS1"].ToString();
                supGroupDto.Address2 = dr["ADDRESS2"].ToString();
                supGroupDto.Address3 = dr["ADDRESS3"].ToString();
                supGroupDto.City = dr["CITY"].ToString();
                supGroupDto.Country = dr["COUNTRY"].ToString();
                supGroupDto.ZipCode = dr["ZIPCODE"].ToString();
                supGroupDto.ContactName = dr["CONTACT_NAME"].ToString();
                supGroupDto.ContactEmail = dr["CONTACT_E_MAIL"].ToString();
                supGroupDto.MobileNumber = dr["MOBILE_NUMBER"].ToString();
                supGroupDto.PhoneNumber = dr["PHONE_NUMBER"].ToString();
                supGroupDto.UserDef1 = dr["USER_DEF1"].ToString();
                supGroupDto.UserDef2 = dr["USER_DEF2"].ToString();
                supGroupDto.UserDef3 = dr["USER_DEF3"].ToString();
                supGroupDto.UserDef4 = dr["USER_DEF4"].ToString();
                supGroupDto.UserDef5 = dr["USER_DEF5"].ToString();
                supGroupDto.UserDef6 = dr["USER_DEF6"].ToString();
                supGroupDto.Supplier = new List<Supplier>();

                foreach (DataRow row in dt.Select("INTERNAL_SUP_GROUP_ID = " + dr["INTERNAL_SUP_GROUP_ID"].ToString()))
                {
                    Supplier sup = new Supplier();
                    sup.SupName = row["SUP_SUP_NAME"].ToString();
                    sup.SupCode = row["SUP_CODE"].ToString();
                    supGroupDto.Supplier.Add(sup);
                }

                string apiUrl = serverApi + "/api/supGroup";

                try
                {
                    await SendApi<SupGroupDto>(apiUrl, supGroupDto);

                   this.UpdateDatabase("update supplier_group set interface_date=sysdate where internal_sup_group_id=" + supGroupDto.InternalSupGroupId.ToString());
                }
                catch(Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + ex.Message));
                    }
                }
                
            }
        }

        public DataTable GetPoComment()
        {
            OracleConnection connection = new OracleConnection(imsDb);

            try
            {
                string sqlCmd = "select po.*,m.warehouse_code " + // case when w.warehouse_wms in ('883-FS','883-HL') then '883' else w.warehouse_wms end warehouse_code " +
                    "from po_comment po " +
                    "inner join mt_po_list m on po.po = m.po_nbr " +
                    //"inner join warehouse w on m.warehouse_code = w.warehouse_wms and m.company_code = w.company_code " +
                    "where interface_date is null or interface_date < date_time_stamp";

                connection.Open();

                OracleCommand command = connection.CreateCommand();
                command.CommandText = sqlCmd;
                command.CommandType = System.Data.CommandType.Text;

                OracleDataReader oracleDataReader = command.ExecuteReader();

                DataTable dtPoComment = new DataTable();

                dtPoComment.Load(oracleDataReader);

                return dtPoComment;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public async void SendPoCommentApi(DataTable dt)
        {
            string apiUrl = serverApi + "/api/poComment";
            foreach (DataRow dr in dt.Rows)
            {
                PoCommentDto poCommentDto = new PoCommentDto();
                poCommentDto.PoNbr = dr["PO"].ToString();
                poCommentDto.WarehouseCode = dr["WAREHOUSE_CODE"].ToString();
                poCommentDto.Comment1 = dr["COMMENT1"].ToString();
                poCommentDto.Comment2 = dr["COMMENT2"].ToString();
                poCommentDto.Comment3 = dr["COMMENT3"].ToString();
                poCommentDto.Comment4 = dr["COMMENT4"].ToString();
                poCommentDto.Comment5 = dr["COMMENT5"].ToString();
                poCommentDto.Comment6 = dr["COMMENT6"].ToString();
                poCommentDto.Comment7 = "";
                poCommentDto.Comment8 = "";

                try
                {
                    await SendApi<PoCommentDto>(apiUrl, poCommentDto);
                    this.UpdateDatabase("update po_comment set interface_date=sysdate where po='" + poCommentDto.PoNbr.ToString() + "'");
                }
                catch(Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + ex.Message));
                    }
                }
                
            }
        }

        public DataTable GetBookingInterface()
        {
           
            OracleConnection connection = new OracleConnection(imsDb);

            try
            {
                string sqlCmd = "select id,booking_id,status from booking_interface where is_interface = 0 and booking_id like 'DHL%' order by date_time_stamp";

                connection.Open();

                OracleCommand command = connection.CreateCommand();
                command.CommandText = sqlCmd;
                command.CommandType = System.Data.CommandType.Text;

                OracleDataReader oracleDataReader = command.ExecuteReader();

                DataTable dtBookingId = new DataTable();

                dtBookingId.Load(oracleDataReader);

                return dtBookingId;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public async void SendBookingApi(DataTable dtBookingId)
        {

            OracleConnection connection = new OracleConnection(imsDb);

            string sqlBookingHeaderCmd = "select bh.internal_header_key,w.warehouse_wms as warehouse_code,bh.booking_id,bh.sup_code,bh.sup_name,bk.booking_date,bh.booking_start,bh.booking_end," +
                "bh.mod_date,bh.user_stamp,bh.back_haul,bh.first_bookgin_start,bh.first_booking_end,bh.first_user_stamp,bh.postponed,bh.status,bh.total_qty,bh.merch_type,bh.remark,bh.remark_delay,bh.approve_condition " +
                "from booking_header bh inner join booking_key bk on bh.internal_key_id = bk.internal_key_id " +
                "inner join warehouse w on bh.warehouse_code = w.warehouse_code " +
                "where bh.booking_id = '{0}'";
            
            string sqlBookingDetailCmd = "select po_nbr,sum(weight) as weight,max(plan_rec) as plan_rec,sum(full_cs) as full_cs,sum(con) as con,sum(non) as non," +
                "sum(cube_full) as cube_full,sum(cube_con) as cube_con,sum(cube_non) as cube_non,delay_reason,is_delay,merch_type " +
                "from booking_detail where internal_header_key = {0} " +
                "group by po_nbr,delay_reason,is_delay,merch_type";

            string sqlBookingTruckCmd = "select tm.truck_code,bt.total_truck from booking_truck bt inner join truck_master tm on bt.internal_truck_id = tm.internal_truck_id " +
                "where bt.internal_header_key = {0}";

            string sqlTruckAssignCmd = "select bh.booking_id,bt.license_plate,bt.license_plate_2,bt.driver_name,bt.tel_no,bt.line_id,tm.truck_code,bt.internal_truck_check_in_id " +
                "from booking_header bh inner join booking_truck_check_in bt on bh.internal_header_key = bt.internal_header_key " +
                "inner join truck_master tm on bt.internal_truck_id = tm.internal_truck_id " +
                "where bh.booking_id = '{0}'";

            string sqlTruckPoCmd = "select po_nbr from booking_truck_check_in_detail where internal_truck_check_in_id = {0} ";


            try
            {
                connection.Open();

                foreach (DataRow row in dtBookingId.Rows)
                {

                    if (row["STATUS"].ToString() == "INTRANSIT")
                    {
                        OracleCommand command = connection.CreateCommand();
                        command.CommandText = string.Format(sqlTruckAssignCmd, row["BOOKING_ID"].ToString());
                        command.CommandType = System.Data.CommandType.Text;

                        OracleDataReader oracleDataReader = command.ExecuteReader();
                        DataTable dtTruckAssign = new DataTable();
                        dtTruckAssign.Load(oracleDataReader);

                        if (dtTruckAssign.Rows.Count > 0)
                        {
                            foreach (DataRow drTruck in dtTruckAssign.Rows)
                            {

                                TruckAssign truckAssign = new TruckAssign();
                                truckAssign.Pos = new List<Pos>();
                                truckAssign.BookingId = drTruck["BOOKING_ID"].ToString();
                                truckAssign.TruckType = drTruck["TRUCK_CODE"].ToString();
                                truckAssign.LicensePlate = drTruck["LICENSE_PLATE"].ToString();
                                truckAssign.LicensePlate2 = drTruck["LICENSE_PLATE_2"].ToString();
                                truckAssign.DriverName = drTruck["DRIVER_NAME"].ToString();
                                truckAssign.TelNo = drTruck["TEL_NO"].ToString();
                                truckAssign.LineId = drTruck["LINE_ID"].ToString();

                                command.CommandText = string.Format(sqlTruckPoCmd, drTruck["INTERNAL_TRUCK_CHECK_IN_ID"].ToString());
                                oracleDataReader = command.ExecuteReader();
                                DataTable dtPoDtl = new DataTable();
                                dtPoDtl.Load(oracleDataReader);
                                foreach (DataRow dr in dtPoDtl.Rows)
                                {
                                    Pos pos = new Pos();
                                    pos.PoNbr = dr["PO_NBR"].ToString();
                                    truckAssign.Pos.Add(pos);
                                }

                                string apiUrl = serverApi + "/api/TruckAssign";
                               // string apiUrl = "https://10.81.224.140/imsapi-sit/api/TruckAssign";
                                try
                                {
                                    await this.SendApi<TruckAssign>(apiUrl, truckAssign);
                                    this.UpdateDatabase("update booking_interface set is_interface=1 where id=" + row["ID"].ToString());
                                }
                                catch (Exception ex)
                                {
                                    using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                                    {
                                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + row["BOOKING_ID"].ToString() + "->" + ex.Message));
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        OracleCommand command = connection.CreateCommand();
                        command.CommandText = string.Format(sqlBookingHeaderCmd, row["BOOKING_ID"].ToString());
                        command.CommandType = System.Data.CommandType.Text;

                        OracleDataReader oracleDataReader = command.ExecuteReader();
                        DataTable dtBookingHdr = new DataTable();
                        dtBookingHdr.Load(oracleDataReader);

                        if (dtBookingHdr.Rows.Count > 0)
                        {
                            command.CommandText = string.Format(sqlBookingDetailCmd, dtBookingHdr.Rows[0]["INTERNAL_HEADER_KEY"].ToString());
                            oracleDataReader = command.ExecuteReader();
                            DataTable dtBookingDtl = new DataTable();
                            dtBookingDtl.Load(oracleDataReader);

                            command.CommandText = string.Format(sqlBookingTruckCmd, dtBookingHdr.Rows[0]["INTERNAL_HEADER_KEY"].ToString());
                            oracleDataReader = command.ExecuteReader();
                            DataTable dtBookingTruck = new DataTable();
                            dtBookingTruck.Load(oracleDataReader);

                            BookingDto bookingDto = new BookingDto();
                            bookingDto.booking_betails = new List<booking_betails>();
                            bookingDto.booking_Trucks = new List<Booking_trucks>();

                            bookingDto.WarehouseCode = dtBookingHdr.Rows[0]["WAREHOUSE_CODE"].ToString();
                            bookingDto.BookingId = dtBookingHdr.Rows[0]["BOOKING_ID"].ToString();
                            bookingDto.SupCode = dtBookingHdr.Rows[0]["SUP_CODE"].ToString();
                            bookingDto.SupName = dtBookingHdr.Rows[0]["SUP_NAME"].ToString();
                            bookingDto.Status = row["STATUS"].ToString();
                            bookingDto.DoorName = "";
                            bookingDto.BookingDate = Convert.ToDateTime(dtBookingHdr.Rows[0]["BOOKING_DATE"]);
                            bookingDto.BookingStart = Convert.ToDateTime(dtBookingHdr.Rows[0]["BOOKING_START"]);
                            bookingDto.BookingEnd = Convert.ToDateTime(dtBookingHdr.Rows[0]["BOOKING_END"]);
                            bookingDto.ModDate = Convert.ToDateTime(dtBookingHdr.Rows[0]["MOD_DATE"]);
                            bookingDto.UserStamp = dtBookingHdr.Rows[0]["USER_STAMP"].ToString();
                            bookingDto.BackHaul = dtBookingHdr.Rows[0]["BACK_HAUL"].ToString() == "0" ? "No" : "Yes";
                            bookingDto.FirstBookginStart = Convert.ToDateTime(dtBookingHdr.Rows[0]["FIRST_BOOKGIN_START"]);
                            bookingDto.FirstBookingEnd = Convert.ToDateTime(dtBookingHdr.Rows[0]["FIRST_BOOKING_END"]);
                            bookingDto.FirstUserStamp = dtBookingHdr.Rows[0]["FIRST_USER_STAMP"].ToString();
                            bookingDto.Postponed = dtBookingHdr.Rows[0]["POSTPONED"].ToString() == "0" ? "No" : "Yes";
                            bookingDto.TruckType = "";
                            bookingDto.UserDef1 = "";
                            bookingDto.UserDef2 = "";
                            bookingDto.UserDef3 = "";
                            bookingDto.UserDef4 = "";
                            bookingDto.TotalQTY = Convert.ToInt32(dtBookingHdr.Rows[0]["TOTAL_QTY"]);
                            bookingDto.Remark = dtBookingHdr.Rows[0].IsNull("REMARK") || string.IsNullOrWhiteSpace(dtBookingHdr.Rows[0]["REMARK"].ToString()) ? "" : dtBookingHdr.Rows[0]["REMARK"].ToString();
                            bookingDto.MerchType = dtBookingHdr.Rows[0].IsNull("MERCH_TYPE") || string.IsNullOrWhiteSpace(dtBookingHdr.Rows[0]["MERCH_TYPE"].ToString()) ? "" : dtBookingHdr.Rows[0]["MERCH_TYPE"].ToString();
                            bookingDto.RemarkPostponed = dtBookingHdr.Rows[0].IsNull("REMARK_DELAY") || string.IsNullOrWhiteSpace(dtBookingHdr.Rows[0]["REMARK_DELAY"].ToString()) ? "" : dtBookingHdr.Rows[0]["REMARK_DELAY"].ToString();
                            bookingDto.RemarkOvercap = dtBookingHdr.Rows[0].IsNull("APPROVE_CONDITION") || string.IsNullOrWhiteSpace(dtBookingHdr.Rows[0]["APPROVE_CONDITION"].ToString()) ? "" : dtBookingHdr.Rows[0]["APPROVE_CONDITION"].ToString();

                            if (row["STATUS"].ToString() != "DELETED")
                            {
                                foreach (DataRow dr in dtBookingDtl.Rows)
                                {
                                    booking_betails booking_Betail = new booking_betails();
                                    booking_Betail.PoNbr = dr["PO_NBR"].ToString();
                                    booking_Betail.TotalQty = Convert.ToInt32(dr["WEIGHT"]);
                                    booking_Betail.PlanRec = Convert.ToDateTime(dr["PLAN_REC"]);
                                    booking_Betail.Full = Convert.ToDecimal(dr["FULL_CS"]);
                                    booking_Betail.Con = Convert.ToDecimal(dr["CON"]);
                                    booking_Betail.Non = Convert.ToDecimal(dr["NON"]);
                                    booking_Betail.CubeFull = Convert.ToDecimal(dr["CUBE_FULL"]);
                                    booking_Betail.CubeCon = Convert.ToDecimal(dr["CUBE_CON"]);
                                    booking_Betail.CubeNon = Convert.ToDecimal(dr["CUBE_NON"]);
                                    booking_Betail.Cube = booking_Betail.CubeFull + booking_Betail.CubeCon + booking_Betail.CubeNon;
                                    if(Convert.ToDateTime(dtBookingHdr.Rows[0]["BOOKING_DATE"]).Date > Convert.ToDateTime(dr["PLAN_REC"]).Date)
                                    {
                                        booking_Betail.Postponed = "Y";
                                        booking_Betail.DelayReason = dr["DELAY_REASON"].ToString();
                                    }
                                    else
                                    {
                                        booking_Betail.Postponed = "N";
                                        booking_Betail.DelayReason = "";
                                    }
                                    booking_Betail.MerchType = dr.IsNull("MERCH_TYPE") || string.IsNullOrWhiteSpace(dr["MERCH_TYPE"].ToString()) ? "" : dr["MERCH_TYPE"].ToString();
                                    bookingDto.booking_betails.Add(booking_Betail);
                                }

                                foreach (DataRow dr in dtBookingTruck.Rows)
                                {
                                    Booking_trucks booking_Truck = new Booking_trucks();
                                    booking_Truck.Truck_Type = dr["TRUCK_CODE"].ToString();
                                    booking_Truck.Total_Truck = Convert.ToInt32(dr["TOTAL_TRUCK"]);
                                    bookingDto.booking_Trucks.Add(booking_Truck);
                                }
                            }
                            else
                            {

                            }
                            string apiUrl = serverApi + "/api/Booking";
                            try
                            {
                                await this.SendApi<BookingDto>(apiUrl, bookingDto);
                                this.UpdateDatabase("update booking_interface set is_interface=1 where id=" + row["ID"].ToString());
                            }
                            catch (Exception ex)
                            {
                                using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                                {
                                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + row["BOOKING_ID"].ToString() + "->" + ex.Message));
                                }
                            }
                        }
                        else
                        {
                            if (row["STATUS"].ToString() == "DELETED")
                            {
                                BookingDto bookingDto = new BookingDto();
                                bookingDto.booking_betails = new List<booking_betails>();
                                bookingDto.booking_Trucks = new List<Booking_trucks>();

                                bookingDto.WarehouseCode = "";
                                bookingDto.BookingId = row["BOOKING_ID"].ToString();
                                bookingDto.SupCode = "";
                                bookingDto.SupName = "";
                                bookingDto.Status = row["STATUS"].ToString();
                                bookingDto.DoorName = "";
                                bookingDto.BookingDate = Convert.ToDateTime(DateTime.Now);
                                bookingDto.BookingStart = Convert.ToDateTime(DateTime.Now);
                                bookingDto.BookingEnd = Convert.ToDateTime(DateTime.Now);
                                bookingDto.ModDate = Convert.ToDateTime(DateTime.Now);
                                bookingDto.UserStamp = "";
                                bookingDto.BackHaul = "";
                                bookingDto.FirstBookginStart = Convert.ToDateTime(DateTime.Now);
                                bookingDto.FirstBookingEnd = Convert.ToDateTime(DateTime.Now);
                                bookingDto.FirstUserStamp = "";
                                bookingDto.Postponed = "";
                                bookingDto.TruckType = "";
                                bookingDto.UserDef1 = "";
                                bookingDto.UserDef2 = "";
                                bookingDto.UserDef3 = "";
                                bookingDto.UserDef4 = "";
                                bookingDto.TotalQTY = 0;
                                bookingDto.Remark = "";
                                bookingDto.RemarkPostponed = "";
                                bookingDto.MerchType = "";
                                bookingDto.RemarkOvercap = "";

                                string apiUrl = serverApi + "/api/Booking";
                                try
                                {
                                    await this.SendApi<BookingDto>(apiUrl, bookingDto);
                                    this.UpdateDatabase("update booking_interface set is_interface=1 where id=" + row["ID"].ToString());
                                }
                                catch (Exception ex)
                                {
                                    using (StreamWriter sw = new StreamWriter(logPath + logFileName , true))
                                    {
                                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + row["BOOKING_ID"].ToString() + "->" + ex.Message));
                                    }
                                }
                            }
                            else
                            {
                                this.UpdateDatabase("update booking_interface set is_interface=1 where id=" + row["ID"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + ex.Message));
                }
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }



        private async Task SendApi<T>(string url, T dto)
        {
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Bypass SSL for testing purposes (only for development)
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                            WriteIndented = true // Optional: Makes the JSON more readable
                        };

                        // Serialize to JSON
                        string json = System.Text.Json.JsonSerializer.Serialize(dto, options);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        Console.WriteLine(json);

                        // Add Basic Authentication                        
                        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userApi}:{passwordApi}"));
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                        // Send POST request
                        var response = client.PostAsync(url, content);

                        Task.WaitAll(response);

                        // Ensure success status
                        response.Result.EnsureSuccessStatusCode();

                        // Read response body
                        string responseBody = await response.Result.Content.ReadAsStringAsync();

                        dynamic resultJson = JsonConvert.DeserializeObject<dynamic>(responseBody);


                        if (resultJson["status"].ToString().ToUpper() == "ERROR")
                        {
                            throw new Exception(responseBody);
                            //using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                            //{
                            //    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + responseBody));
                            //}
                        }
                        Console.WriteLine("Response:");
                        Console.WriteLine(responseBody);
                    }
                    catch (HttpRequestException ex)
                    {
                        using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                        {
                            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + ex.Message));
                        }
                    }
                    catch (Exception ex)
                    {
                        using (StreamWriter sw = new StreamWriter(logPath + logFileName, true))
                        {
                            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss : " + ex.Message));
                        }
                    }
                }
            }
        }

        public void UpdateDatabase(string sqlCmd)
        {
            OracleConnection connection = new OracleConnection(imsDb);

            try
            {                
                connection.Open();

                OracleCommand command = connection.CreateCommand();
                command.CommandText = sqlCmd;
                command.CommandType = System.Data.CommandType.Text;

                var result = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }


    }
}
