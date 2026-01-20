using System;

namespace Makro.IMS.Services.Api.Dto
{
    public class Menus
    {
        public string Module { get; set; }
        public string PageName { get; set; }

        public List<Menus> GetMenus()
        {
            List<Menus> menus = new List<Menus>();

            menus.Add(new Menus { Module = "Dashboard", PageName = "Overview" });
            menus.Add(new Menus { Module = "Dashboard", PageName = "ScheduleDoor" });
            menus.Add(new Menus { Module = "Dashboard", PageName = "SummaryBooking" });
            menus.Add(new Menus { Module = "Dashboard", PageName = "SlottimeBooking" });

            menus.Add(new Menus { Module = "Master", PageName = "Warehouse" });
            menus.Add(new Menus { Module = "Master", PageName = "OperationType" });
            menus.Add(new Menus { Module = "Master", PageName = "Door" });
            menus.Add(new Menus { Module = "Master", PageName = "SupplierGroup" });
            menus.Add(new Menus { Module = "Master", PageName = "Supplier" });
            menus.Add(new Menus { Module = "Master", PageName = "WarehouseCapacity" });
            menus.Add(new Menus { Module = "Master", PageName = "WarehouseOperationCapacity" });

            menus.Add(new Menus { Module = "Booking", PageName = "BookingHeader" });
            menus.Add(new Menus { Module = "Booking", PageName = "CreateBooking" });
            menus.Add(new Menus { Module = "Booking", PageName = "CreateBookingExcel" });
            menus.Add(new Menus { Module = "Booking", PageName = "CreateBookingNopo" });
            menus.Add(new Menus { Module = "Booking", PageName = "CheckPo" });

            menus.Add(new Menus { Module = "PreCheckIn", PageName = "PreCheckInExcel" });
            menus.Add(new Menus { Module = "PreCheckIn", PageName = "PreCheckInCompleted" });

            menus.Add(new Menus { Module = "CheckInOut", PageName = "GuardCheckIn" });
            menus.Add(new Menus { Module = "CheckInOut", PageName = "GuardCheckOut" });

            menus.Add(new Menus { Module = "QueueManage", PageName = "ManageQueue" });
            menus.Add(new Menus { Module = "QueueManage", PageName = "GuardCreateBooking" });

            menus.Add(new Menus { Module = "OpsManage", PageName = "DockDoorList" });

            menus.Add(new Menus { Module = "Report", PageName = "TruckStatus" });
            menus.Add(new Menus { Module = "Report", PageName = "TransactionTrack" });

            menus.Add(new Menus { Module = "Transfer", PageName = "TransferTrailerEmpty" });
            menus.Add(new Menus { Module = "Transfer", PageName = "TransferTrailerFull" });
            menus.Add(new Menus { Module = "Transfer", PageName = "TransferList" });
            menus.Add(new Menus { Module = "Transfer", PageName = "TransferProcess" });
            menus.Add(new Menus { Module = "Transfer", PageName = "TransferReport" });

            menus.Add(new Menus { Module = "Account", PageName = "ResetPassword" });
            menus.Add(new Menus { Module = "Account", PageName = "UserMaster" });
            menus.Add(new Menus { Module = "Account", PageName = "Role" });


            return menus;
        }

    }


}
