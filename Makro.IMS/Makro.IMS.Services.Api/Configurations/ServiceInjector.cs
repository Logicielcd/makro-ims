using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Services;

namespace Makro.IMS.Services.Api.Configurations
{
    public static class ServiceInjector
    {

        public static void RegisterServices(this IServiceCollection services)
        {
            // User Master
            services.AddScoped<UserMasterService>();
            services.AddScoped<BookingKeyService>();
            services.AddScoped<PoService>();
            services.AddScoped<BookingService>();
            services.AddScoped<BookingHeaderService>();
            services.AddScoped<BookingKeyService>();
            services.AddScoped<WarehouseCapacityService>();
            services.AddScoped<WarehouseOperationCapacityService>();
            services.AddScoped<WarehouseService>();
            services.AddScoped<DoorService>();
            services.AddScoped<SupplierService>();
            services.AddScoped<TruckMasterService>();
            services.AddScoped<EstTimeService>();
            services.AddScoped<CreateBookingExcelService>();
            services.AddScoped<PreCheckInExcelService>();
            services.AddScoped<GuardCheckInOutService>();
            services.AddScoped<ManageQueueService>();
            services.AddScoped<OperationService>();
            services.AddScoped<OperationCapacityService>();
            services.AddScoped<OperationFixSlotService>();
            services.AddScoped<QueueSequenceService>();
            services.AddScoped<SmsService>();
            services.AddScoped<PoCheckInOutService>();
            services.AddScoped<SupplierGroupService>();
            services.AddScoped<DriverService>();
            services.AddScoped<OperationTimeService>();
            services.AddScoped<ReportService>();
            services.AddScoped<InterfaceService>();
            services.AddScoped<InboundBookingService>();
            services.AddScoped<SlotCapacityService>();
            services.AddScoped<PoCommentService>();
            services.AddScoped<TrailerService>();
            services.AddScoped<ShuntService>();
            services.AddScoped<YardService>();
            services.AddScoped<JobService>();
            services.AddScoped<JobOnDockService>();
        }

    }
}
