using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Makro.IMS.Infra.Data.Repository
{
    public class SlotCapacityRepository : IDisposable
    {
        private IMSContext context;

        public SlotCapacityRepository(IMSContext context)
        {
            this.context = context;           
        }

        public IEnumerable<SlotCapacity> GetSlotCapacities(DateTime bookingDate, string warehouse, string operationType, int bookingKeyId)
        {
            var vCursor = new OracleParameter("V_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);
            var vWhse = new OracleParameter("v_WHSE", OracleDbType.NVarchar2, ParameterDirection.Input);            
            var vOperation = new OracleParameter("v_OPS", OracleDbType.NVarchar2, ParameterDirection.Input);
            var vBookingDate = new OracleParameter("v_BOOKING_DATE", OracleDbType.Date, ParameterDirection.Input);

            vWhse.Value = warehouse;
            vOperation.Value = operationType;
            vBookingDate.Value = bookingDate.ToLocalTime();

            var slotCapacities = this.context.Set<SlotCapacity>()
                .FromSqlRaw("BEGIN GET_SLOT_CAP_OPERATION_BY_DATE (:v_BOOKING_DATE,:v_WHSE,:v_OPS,:V_CURSOR);  END;", new object[] { vBookingDate,vWhse, vOperation, vCursor })
                .ToList();

            return slotCapacities;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
