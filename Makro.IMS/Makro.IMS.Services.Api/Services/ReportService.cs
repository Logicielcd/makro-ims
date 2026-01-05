using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;

namespace Makro.IMS.Services.Api.Services
{
    public class ReportService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;

        public ReportService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
        }


        public async Task<List<ReportGatePass>> GetGatePass(int internalHeaderKey)
        {

            return unitOfWork.ReportRepository.GetGatePass(internalHeaderKey).ToList();
        }

        public async Task<ReportSummaryBooking?> GetSummaryBooking(string warehouseCode,string companyCode)
        {

            return unitOfWork.ReportRepository.GetBookingSummary(warehouseCode,companyCode);
        }

        public async Task<List<ReportTruckStatus>> GetTruckStatus(string warehouseCode, string companyCode,DateTime fromDate,DateTime toDate)
        {

            return unitOfWork.ReportRepository.GetTruckStatuses(warehouseCode, companyCode,fromDate,toDate);
        }

        public async Task<List<ReportTransactionTrack>> GetTransactionTrack(string warehouseCode, string companyCode,DateTime fromDate,DateTime toDate)
        {

            return unitOfWork.ReportRepository.GetTransactionTrack(warehouseCode, companyCode,fromDate,toDate);
        }
        public async Task<List<ReportSlottimeBooking>> GetSlottimeBooking(string warehouseCode, string companyCode, DateTime fromDate)
        {

            return unitOfWork.ReportRepository.GetSlottimeBooking(warehouseCode, companyCode, fromDate);
        }
        public async Task<List<ReportSlottimeBooking>> GetSlottimeBookingActual(string warehouseCode, string companyCode, DateTime fromDate)
        {

            return unitOfWork.ReportRepository.GetSlottimeBookingActual(warehouseCode, companyCode, fromDate);
        }

        public async Task<List<ReportSlottimeBooking>> GetSlottimeBookingPending(string warehouseCode, string companyCode, DateTime fromDate)
        {

            return unitOfWork.ReportRepository.GetSlottimeBookingPending(warehouseCode, companyCode, fromDate);
        }


        public async Task<List<ReportDockDoorControl>> GetDockDoorControl(string warehouseCode, string doorRange)
        {

            return unitOfWork.ReportRepository.GetDockDoorControl(warehouseCode, doorRange);
        }
    }
}
