using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;

namespace Makro.IMS.Services.Api.Services
{
    public class BookingService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        
        public BookingService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);        
        }

        public async Task<PoList> GetPoByPoNo(string supCode,string companyNo, string poNo)
        {
            var poLists = unitOfWork.PoListRepository.GetPoNotBooking(supCode, companyNo);

            var po = poLists.FirstOrDefault(x => x.Po_Nbr == poNo);

            return po!;
        }

        public async Task SaveBookingData(UserMaster user, string companyNo, DateTime bookingDate)
        {
            BookingKey bookingKey = new BookingKey();
            bookingKey.CreateDate = DateTime.Now;
            bookingKey.CompanyCode = companyNo;
            bookingKey.UserStamp = user.UserId;

            unitOfWork.BookingKeyRepository.Add(bookingKey);
            unitOfWork.Save();

            // create booking header

        }



    }
}
