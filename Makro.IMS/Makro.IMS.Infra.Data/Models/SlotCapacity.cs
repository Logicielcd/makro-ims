using System.Reflection.Metadata.Ecma335;

namespace Makro.IMS.Infra.Data.Models
{

    public class SlotCapacity
    {        
        public string TimeSlot { get; set; }
        public string TimeSlotEnd { get; set; }
        public string OperationTime { get; set; }
        public decimal Cap { get; set; }
        public decimal CapTruck { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalTruck { get; set; }        
    }

}
