using System;
using System.Collections.Generic;
using System.Reflection;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Makro.IMS.Infra.Data.Context;

public partial class IMSContext : DbContext
{
    private static readonly ILoggerFactory myLoggerFactory
         = LoggerFactory.Create(builder =>
         {
             //builder.AddConsole();
             builder.AddFilter((category, level) =>
                 category == DbLoggerCategory.Database.Command.Name
                 && level == LogLevel.Information)
             .AddDebug()
             .AddConsole();

         });

    public IMSContext()
    {
    }

    public IMSContext(DbContextOptions<IMSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<BookingHeader> BookingHeaders { get; set; }

    public virtual DbSet<BookingInterface> BookingInterfaces { get; set; }

    public virtual DbSet<BookingKey> BookingKeys { get; set; }

    public virtual DbSet<BookingTruck> BookingTrucks { get; set; }

    public virtual DbSet<BookingTruckCheckIn> BookingTruckCheckIns { get; set; }

    public virtual DbSet<BookingTruckCheckInDetail> BookingTruckCheckInDetails { get; set; }

    public virtual DbSet<BookingTruckLog> BookingTruckLogs { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Door> Doors { get; set; }

    public virtual DbSet<EstTime> EstTimes { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<Operation> Operations { get; set; }

    public virtual DbSet<OperationCapacity> OperationCapacities { get; set; }

    public virtual DbSet<OperationFixSlot> OperationFixSlots { get; set; }

    public virtual DbSet<OperationTime> OperationTimes { get; set; }

    public virtual DbSet<PoCheckInOut> PoCheckInOuts { get; set; }

    public virtual DbSet<PoComment> PoComments { get; set; }

    public virtual DbSet<PoLog> PoLogs { get; set; }

    public virtual DbSet<QueueSequence> QueueSequences { get; set; }

    public virtual DbSet<Shunt> Shunts { get; set; }

    public virtual DbSet<SupTypeMaster> SupTypeMasters { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierGroup> SupplierGroups { get; set; }

    public virtual DbSet<Trailer> Trailers { get; set; }

    public virtual DbSet<TruckCap> TruckCaps { get; set; }

    public virtual DbSet<TruckMaster> TruckMasters { get; set; }

    public virtual DbSet<TruckRule> TruckRules { get; set; }

    public virtual DbSet<UserMaster> UserMasters { get; set; }

    public virtual DbSet<UserSupplierGroup> UserSupplierGroups { get; set; }

    public virtual DbSet<UserWarehouse> UserWarehouses { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WarehouseCapacity> WarehouseCapacities { get; set; }

    public virtual DbSet<WarehouseOperationCapacity> WarehouseOperationCapacities { get; set; }

    public virtual DbSet<Yard> Yards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // set connection string on context
        // get the configuration from the app settings
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        optionsBuilder
            .UseLoggerFactory(myLoggerFactory)
            .UseOracle(connectionString,
            builder =>
            {
                builder.CommandTimeout(600);
                builder.MigrationsAssembly(typeof(IMSContext).GetTypeInfo().Assembly.GetName().Name);
                builder.UseOracleSQLCompatibility("11");
            });

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PoList>().ToView("PoList").HasNoKey();
        modelBuilder.Entity<BookingCapacity>().ToView("BookingCapacity").HasNoKey();
        modelBuilder.Entity<KeyId>().ToView("KeyId").HasNoKey();
        modelBuilder.Entity<BookingCheckInDto>().ToView("BookingCheckInDto").HasNoKey();
        modelBuilder.Entity<QueueManageDto>().ToView("QueueManageDto").HasNoKey();
        modelBuilder.Entity<DoorQueueDto>().ToView("DoorQueueDto").HasNoKey();
        modelBuilder.Entity<ReportGatePass>().ToView("ReportGatePass").HasNoKey();
        modelBuilder.Entity<ReportSummaryBooking>().ToView("ReportSummaryBooking").HasNoKey();
        modelBuilder.Entity<ReportTruckStatus>().ToView("ReportTruckStatus").HasNoKey();
        modelBuilder.Entity<ReportTransactionTrack>().ToView("ReportTransactionTrack").HasNoKey();
        modelBuilder.Entity<ReportSlottimeBooking>().ToView("ReportSlottimeBooking").HasNoKey();
        modelBuilder.Entity<ReportDockDoorControl>().ToView("ReportDockDoorControl").HasNoKey();
        modelBuilder.Entity<TruckCheckIn>().ToView("TruckCheckIn").HasNoKey();
        modelBuilder.Entity<SlotCapacity>().ToView("SlotCapacity").HasNoKey();
        modelBuilder.Entity<PoMonitor>().ToView("PoMonitor").HasNoKey();

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.HasKey(e => e.InternalDetailKey).HasName("BOOKING_DETAIL_PK");

            entity.ToTable("BOOKING_DETAIL");

            entity.HasIndex(e => e.InternalHeaderKey, "BOOKING_DETAIL_INDEX1");

            entity.HasIndex(e => e.PoNbr, "BOOKING_DETAIL_PO");

            entity.Property(e => e.InternalDetailKey)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_DETAIL_KEY");
            entity.Property(e => e.CheckIn)
                .HasColumnType("DATE")
                .HasColumnName("CHECK_IN");
            entity.Property(e => e.CheckOut)
                .HasColumnType("DATE")
                .HasColumnName("CHECK_OUT");
            entity.Property(e => e.Con)
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("CON");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.CubeCon)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("CUBE_CON");
            entity.Property(e => e.CubeFull)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("CUBE_FULL");
            entity.Property(e => e.CubeNon)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("CUBE_NON");
            entity.Property(e => e.DelayReason)
                .HasMaxLength(50)
                .HasColumnName("DELAY_REASON");
            entity.Property(e => e.DocumentCheckIn)
                .HasColumnType("DATE")
                .HasColumnName("DOCUMENT_CHECK_IN");
            entity.Property(e => e.ExpireDate)
                .HasColumnType("DATE")
                .HasColumnName("EXPIRE_DATE");
            entity.Property(e => e.FullCs)
                .HasColumnType("NUMBER")
                .HasColumnName("FULL_CS");
            entity.Property(e => e.FullPl)
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("FULL_PL");
            entity.Property(e => e.HalfCs)
                .HasColumnType("NUMBER")
                .HasColumnName("HALF_CS");
            entity.Property(e => e.HalfPl)
                .HasPrecision(18)
                .HasColumnName("HALF_PL");
            entity.Property(e => e.InternalHeaderKey)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_HEADER_KEY");
            entity.Property(e => e.IsDelay)
                .HasPrecision(1)
                .HasColumnName("IS_DELAY");
            entity.Property(e => e.MerchType)
                .HasMaxLength(50)
                .HasColumnName("MERCH_TYPE");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.Non)
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("NON");
            entity.Property(e => e.PlanRec)
                .HasColumnType("DATE")
                .HasColumnName("PLAN_REC");
            entity.Property(e => e.PoNbr)
                .HasMaxLength(50)
                .HasColumnName("PO_NBR");
            entity.Property(e => e.Postponed)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("POSTPONED");
            entity.Property(e => e.PreCheckIn)
                .HasColumnType("DATE")
                .HasColumnName("PRE_CHECK_IN");
            entity.Property(e => e.Remark)
                .HasMaxLength(550)
                .HasColumnName("REMARK");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("STATUS");
            entity.Property(e => e.TotalQty)
                .HasPrecision(10)
                .HasColumnName("TOTAL_QTY");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
            entity.Property(e => e.Weight)
                .HasColumnType("NUMBER(13,3)")
                .HasColumnName("WEIGHT");
        });

        modelBuilder.Entity<BookingHeader>(entity =>
        {
            entity.HasKey(e => e.InternalHeaderKey).HasName("BOOKING_HEADER_PK");

            entity.ToTable("BOOKING_HEADER");

            entity.HasIndex(e => e.SupCode, "BOOKING_HEADER_SUP");

            entity.HasIndex(e => e.WarehouseCode, "BOOKING_HEADER_WHSE");

            entity.Property(e => e.InternalHeaderKey)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_HEADER_KEY");
            entity.Property(e => e.Active)
                .HasPrecision(1)
                .ValueGeneratedOnAdd()
                .HasColumnName("ACTIVE");
            entity.Property(e => e.ApproveCondition)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("APPROVE_CONDITION");
            entity.Property(e => e.BackHaul)
                .HasPrecision(1)
                .ValueGeneratedOnAdd()
                .HasColumnName("BACK_HAUL");
            entity.Property(e => e.BookingEnd)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("BOOKING_END");
            entity.Property(e => e.BookingId)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("BOOKING_ID");
            entity.Property(e => e.BookingStart)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("BOOKING_START");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .HasColumnName("COMPANY_CODE");
            entity.Property(e => e.ContactEmail).HasColumnName("CONTACT_EMAIL");
            entity.Property(e => e.ContactName)
                .HasMaxLength(500)
                .HasColumnName("CONTACT_NAME");
            entity.Property(e => e.ContactTel)
                .HasMaxLength(100)
                .HasColumnName("CONTACT_TEL");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.FirstBookginStart)
                .HasColumnType("DATE")
                .HasColumnName("FIRST_BOOKGIN_START");
            entity.Property(e => e.FirstBookingEnd)
                .HasColumnType("DATE")
                .HasColumnName("FIRST_BOOKING_END");
            entity.Property(e => e.FirstUserStamp)
                .HasMaxLength(50)
                .HasColumnName("FIRST_USER_STAMP");
            entity.Property(e => e.InternalDoorId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_DOOR_ID");
            entity.Property(e => e.InternalKeyId)
                .HasPrecision(10)
                .ValueGeneratedOnAdd()
                .HasColumnName("INTERNAL_KEY_ID");
            entity.Property(e => e.InternalSupGroupId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_SUP_GROUP_ID");
            entity.Property(e => e.IsDelay)
                .HasPrecision(1)
                .HasColumnName("IS_DELAY");
            entity.Property(e => e.MerchType)
                .HasMaxLength(50)
                .HasColumnName("MERCH_TYPE");
            entity.Property(e => e.ModDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.OrgNeedDate)
                .HasColumnType("DATE")
                .HasColumnName("ORG_NEED_DATE");
            entity.Property(e => e.OriginalMerchType)
                .HasMaxLength(50)
                .HasColumnName("ORIGINAL_MERCH_TYPE");
            entity.Property(e => e.Postponed)
                .HasPrecision(1)
                .ValueGeneratedOnAdd()
                .HasColumnName("POSTPONED");
            entity.Property(e => e.Remark).HasColumnName("REMARK");
            entity.Property(e => e.RemarkCancel)
                .HasMaxLength(100)
                .HasColumnName("REMARK_CANCEL");
            entity.Property(e => e.RemarkDelay)
                .HasMaxLength(250)
                .HasColumnName("REMARK_DELAY");
            entity.Property(e => e.RevisionPrefix)
                .HasMaxLength(50)
                .HasColumnName("REVISION_PREFIX");
            entity.Property(e => e.RevisionRunning)
                .HasPrecision(10)
                .HasColumnName("REVISION_RUNNING");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("STATUS");
            entity.Property(e => e.SupCode)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("SUP_CODE");
            entity.Property(e => e.SupName)
                .HasMaxLength(150)
                .ValueGeneratedOnAdd()
                .HasColumnName("SUP_NAME");
            entity.Property(e => e.TotalPo)
                .HasPrecision(10)
                .ValueGeneratedOnAdd()
                .HasColumnName("TOTAL_PO");
            entity.Property(e => e.TotalQty)
                .HasPrecision(10)
                .ValueGeneratedOnAdd()
                .HasColumnName("TOTAL_QTY");
            entity.Property(e => e.UserCancel)
                .HasMaxLength(50)
                .HasColumnName("USER_CANCEL");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("USER_STAMP");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("WAREHOUSE_CODE");
        });

        modelBuilder.Entity<BookingInterface>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BOOKING_INTERFACE_PK");

            entity.ToTable("BOOKING_INTERFACE");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.BookingId)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("BOOKING_ID");
            entity.Property(e => e.DateTimeStamp)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("DATE_TIME_STAMP");
            entity.Property(e => e.IsInterface)
                .HasPrecision(1)
                .ValueGeneratedOnAdd()
                .HasColumnName("IS_INTERFACE");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("STATUS");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<BookingKey>(entity =>
        {
            entity.HasKey(e => e.InternalKeyId).HasName("BOOKING_KEY_PK");

            entity.ToTable("BOOKING_KEY");

            entity.HasIndex(e => new { e.InternalKeyId, e.BookingDate }, "BOOKING_KEY_INDEX1");

            entity.Property(e => e.InternalKeyId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_KEY_ID");
            entity.Property(e => e.Active)
                .HasPrecision(1)
                .HasColumnName("ACTIVE");
            entity.Property(e => e.BookingDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("BOOKING_DATE");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .HasColumnName("COMPANY_CODE");
            entity.Property(e => e.CreateDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.MailStatus)
                .HasMaxLength(50)
                .HasColumnName("MAIL_STATUS");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.RecMailId)
                .HasPrecision(10)
                .HasColumnName("REC_MAIL_ID");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<BookingTruck>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BOOKING_TRUCK");

            entity.HasIndex(e => new { e.InternalHeaderKey, e.InternalTruckId }, "BOOKING_TRUCK_PK");

            entity.Property(e => e.InternalHeaderKey)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_HEADER_KEY");
            entity.Property(e => e.InternalTruckId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_TRUCK_ID");
            entity.Property(e => e.Remark)
                .HasMaxLength(50)
                .HasColumnName("REMARK");
            entity.Property(e => e.TotalTruck)
                .HasPrecision(10)
                .HasColumnName("TOTAL_TRUCK");
        });

        modelBuilder.Entity<BookingTruckCheckIn>(entity =>
        {
            entity.HasKey(e => e.InternalTruckCheckInId).HasName("BOOKING_TRUCK_CHECK_IN_PK");

            entity.ToTable("BOOKING_TRUCK_CHECK_IN");

            entity.HasIndex(e => e.InternalHeaderKey, "BOOKING_TRUCK_CHECK_IN_INDEX1");

            entity.Property(e => e.InternalTruckCheckInId)
                .HasColumnType("NUMBER")
                .HasColumnName("INTERNAL_TRUCK_CHECK_IN_ID");
            entity.Property(e => e.ArrivedTime)
                .HasColumnType("DATE")
                .HasColumnName("ARRIVED_TIME");
            entity.Property(e => e.AssignQueueTime)
                .HasColumnType("DATE")
                .HasColumnName("ASSIGN_QUEUE_TIME");
            entity.Property(e => e.CalltruckTime)
                .HasColumnType("DATE")
                .HasColumnName("CALLTRUCK_TIME");
            entity.Property(e => e.CheckInTime)
                .HasColumnType("DATE")
                .HasColumnName("CHECK_IN_TIME");
            entity.Property(e => e.CheckoutTime)
                .HasColumnType("DATE")
                .HasColumnName("CHECKOUT_TIME");
            entity.Property(e => e.DateTimeStamp)
                .HasColumnType("DATE")
                .HasColumnName("DATE_TIME_STAMP");
            entity.Property(e => e.DepartureTime)
                .HasColumnType("DATE")
                .HasColumnName("DEPARTURE_TIME");
            entity.Property(e => e.DoccheckTime)
                .HasColumnType("DATE")
                .HasColumnName("DOCCHECK_TIME");
            entity.Property(e => e.Door)
                .HasMaxLength(50)
                .HasColumnName("DOOR");
            entity.Property(e => e.DriverName)
                .HasMaxLength(50)
                .HasColumnName("DRIVER_NAME");
            entity.Property(e => e.FinishUnloadTime)
                .HasColumnType("DATE")
                .HasColumnName("FINISH_UNLOAD_TIME");
            entity.Property(e => e.InternalHeaderKey)
                .HasColumnType("NUMBER")
                .HasColumnName("INTERNAL_HEADER_KEY");
            entity.Property(e => e.InternalTruckId)
                .HasColumnType("NUMBER")
                .HasColumnName("INTERNAL_TRUCK_ID");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(50)
                .HasColumnName("LICENSE_PLATE");
            entity.Property(e => e.LicensePlate2)
                .HasMaxLength(50)
                .HasColumnName("LICENSE_PLATE_2");
            entity.Property(e => e.LineId)
                .HasMaxLength(50)
                .HasColumnName("LINE_ID");
            entity.Property(e => e.OndockTime)
                .HasColumnType("DATE")
                .HasColumnName("ONDOCK_TIME");
            entity.Property(e => e.QueueSeq)
                .HasColumnType("NUMBER")
                .HasColumnName("QUEUE_SEQ");
            entity.Property(e => e.Remark)
                .HasMaxLength(500)
                .HasColumnName("REMARK");
            entity.Property(e => e.StartUnloadTime)
                .HasColumnType("DATE")
                .HasColumnName("START_UNLOAD_TIME");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("STATUS");
            entity.Property(e => e.SubmitdocTime)
                .HasColumnType("DATE")
                .HasColumnName("SUBMITDOC_TIME");
            entity.Property(e => e.TelNo)
                .HasMaxLength(50)
                .HasColumnName("TEL_NO");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
            entity.Property(e => e.WaitingDocument)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("WAITING_DOCUMENT");
            entity.Property(e => e.WaitingDocumentTime)
                .HasColumnType("DATE")
                .HasColumnName("WAITING_DOCUMENT_TIME");
            entity.Property(e => e.YardIn)
                .HasMaxLength(50)
                .HasColumnName("YARD_IN");
            entity.Property(e => e.YardOut)
                .HasMaxLength(50)
                .HasColumnName("YARD_OUT");
        });

        modelBuilder.Entity<BookingTruckCheckInDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("BOOKING_TRUCK_CHECK_IN_DETAIL");

            entity.HasIndex(e => e.InternalTruckCheckInId, "BOOKING_TRUCK_CHECK_IN_DETAIL_INDEX1");

            entity.Property(e => e.CheckInTime)
                .HasColumnType("DATE")
                .HasColumnName("CHECK_IN_TIME");
            entity.Property(e => e.InternalDetailKey)
                .HasColumnType("NUMBER")
                .HasColumnName("INTERNAL_DETAIL_KEY");
            entity.Property(e => e.InternalTruckCheckInId)
                .HasColumnType("NUMBER")
                .HasColumnName("INTERNAL_TRUCK_CHECK_IN_ID");
            entity.Property(e => e.InternalTruckDetailId)
                .HasColumnType("NUMBER")
                .HasColumnName("INTERNAL_TRUCK_DETAIL_ID");
            entity.Property(e => e.PoNbr)
                .HasMaxLength(50)
                .HasColumnName("PO_NBR");
        });

        modelBuilder.Entity<BookingTruckLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BOOKING_TRUCK_LOG_PK");

            entity.ToTable("BOOKING_TRUCK_LOG");

            entity.HasIndex(e => new { e.InternalTruckCheckInId, e.Action }, "BOOKING_TRUCK_LOG_INDEX1");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("ACTION");
            entity.Property(e => e.DateTimeStamp)
                .HasColumnType("DATE")
                .HasColumnName("DATE_TIME_STAMP");
            entity.Property(e => e.InternalTruckCheckInId)
                .HasColumnType("NUMBER")
                .HasColumnName("INTERNAL_TRUCK_CHECK_IN_ID");
            entity.Property(e => e.Remark)
                .HasMaxLength(500)
                .HasColumnName("REMARK");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyCode).HasName("COMPANY_PK");

            entity.ToTable("COMPANY");

            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .HasColumnName("COMPANY_CODE");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(150)
                .HasColumnName("COMPANY_NAME");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.EMailGateway)
                .HasMaxLength(50)
                .HasColumnName("E_MAIL_GATEWAY");
            entity.Property(e => e.EMailSender)
                .HasMaxLength(50)
                .HasColumnName("E_MAIL_SENDER");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.PdfPath)
                .HasMaxLength(550)
                .HasColumnName("PDF_PATH");
            entity.Property(e => e.ReadEMailFolder)
                .HasMaxLength(550)
                .HasColumnName("READ_E_MAIL_FOLDER");
            entity.Property(e => e.RecAttPath)
                .HasMaxLength(550)
                .HasColumnName("REC_ATT_PATH");
            entity.Property(e => e.ReportPath)
                .HasMaxLength(550)
                .HasColumnName("REPORT_PATH");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<Door>(entity =>
        {
            entity.HasKey(e => e.InternalDoorId).HasName("DOOR_PK");

            entity.ToTable("DOOR");

            entity.Property(e => e.InternalDoorId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_DOOR_ID");
            entity.Property(e => e.Active)
                .HasPrecision(1)
                .HasColumnName("ACTIVE");
            entity.Property(e => e.BookingHeaderKey)
                .HasColumnType("NUMBER")
                .HasColumnName("BOOKING_HEADER_KEY");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.DoorArea)
                .HasMaxLength(50)
                .HasColumnName("DOOR_AREA");
            entity.Property(e => e.DoorName)
                .HasMaxLength(50)
                .HasColumnName("DOOR_NAME");
            entity.Property(e => e.LoadingType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("LOADING_TYPE");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.Sequence)
                .HasMaxLength(50)
                .HasColumnName("SEQUENCE");
            entity.Property(e => e.TruckType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TRUCK_TYPE");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .HasColumnName("WAREHOUSE_CODE");
        });

        modelBuilder.Entity<EstTime>(entity =>
        {
            entity.HasKey(e => e.InternalEstId).HasName("EST_TIME_PK");

            entity.ToTable("EST_TIME");

            entity.Property(e => e.InternalEstId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_EST_ID");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.HourEst)
                .HasPrecision(10)
                .HasColumnName("HOUR_EST");
            entity.Property(e => e.InternalSupGroupId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_SUP_GROUP_ID");
            entity.Property(e => e.InternalTruckId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_TRUCK_ID");
            entity.Property(e => e.MinEst)
                .HasPrecision(10)
                .HasColumnName("MIN_EST");
            entity.Property(e => e.OperationType)
                .HasMaxLength(50)
                .HasColumnName("OPERATION_TYPE");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .HasColumnName("WAREHOUSE_CODE");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("JOB_PK");

            entity.ToTable("JOB");

            entity.Property(e => e.Id)
                .HasPrecision(10)
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.JobDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("JOB_DATE");
            entity.Property(e => e.JobId)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("JOB_ID");
            entity.Property(e => e.JobType)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("JOB_TYPE");
            entity.Property(e => e.LocationId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("LOCATION_ID");
            entity.Property(e => e.LocationType)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("LOCATION_TYPE");
            entity.Property(e => e.ModDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.ProcessTime)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("PROCESS_TIME");
            entity.Property(e => e.ShuntId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("SHUNT_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("STATUS");
            entity.Property(e => e.TrailerId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("TRAILER_ID");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => new { e.OperationName, e.WarehouseCode }).HasName("OPERATION_PK");

            entity.ToTable("OPERATION");

            entity.Property(e => e.OperationName)
                .HasMaxLength(50)
                .HasColumnName("OPERATION_NAME");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(20)
                .HasColumnName("WAREHOUSE_CODE");
            entity.Property(e => e.Capacity)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("CAPACITY");
            entity.Property(e => e.CapacityLarge)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("CAPACITY_LARGE");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Overcap)
                .HasPrecision(1)
                .HasColumnName("OVERCAP");
            entity.Property(e => e.Overcutoff)
                .HasPrecision(1)
                .HasColumnName("OVERCUTOFF");
        });

        modelBuilder.Entity<OperationCapacity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OPERATION_CAPACITY_PK");

            entity.ToTable("OPERATION_CAPACITY");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.FriCap)
                .HasColumnType("NUMBER")
                .HasColumnName("FRI_CAP");
            entity.Property(e => e.FriTruck)
                .HasColumnType("NUMBER")
                .HasColumnName("FRI_TRUCK");
            entity.Property(e => e.MonCap)
                .HasColumnType("NUMBER")
                .HasColumnName("MON_CAP");
            entity.Property(e => e.MonTruck)
                .HasColumnType("NUMBER")
                .HasColumnName("MON_TRUCK");
            entity.Property(e => e.OperationType)
                .HasMaxLength(50)
                .HasColumnName("OPERATION_TYPE");
            entity.Property(e => e.SatCap)
                .HasColumnType("NUMBER")
                .HasColumnName("SAT_CAP");
            entity.Property(e => e.SatTruck)
                .HasColumnType("NUMBER")
                .HasColumnName("SAT_TRUCK");
            entity.Property(e => e.SunCap)
                .HasColumnType("NUMBER")
                .HasColumnName("SUN_CAP");
            entity.Property(e => e.SunTruck)
                .HasColumnType("NUMBER")
                .HasColumnName("SUN_TRUCK");
            entity.Property(e => e.ThuCap)
                .HasColumnType("NUMBER")
                .HasColumnName("THU_CAP");
            entity.Property(e => e.ThuTruck)
                .HasColumnType("NUMBER")
                .HasColumnName("THU_TRUCK");
            entity.Property(e => e.Time)
                .HasPrecision(6)
                .HasColumnName("TIME");
            entity.Property(e => e.TueCap)
                .HasColumnType("NUMBER")
                .HasColumnName("TUE_CAP");
            entity.Property(e => e.TueTruck)
                .HasColumnType("NUMBER")
                .HasColumnName("TUE_TRUCK");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(100)
                .HasColumnName("WAREHOUSE_CODE");
            entity.Property(e => e.WedCap)
                .HasColumnType("NUMBER")
                .HasColumnName("WED_CAP");
            entity.Property(e => e.WedTruck)
                .HasColumnType("NUMBER")
                .HasColumnName("WED_TRUCK");
        });

        modelBuilder.Entity<OperationFixSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TABLE1_PK");

            entity.ToTable("OPERATION_FIX_SLOT");

            entity.HasIndex(e => new { e.WarehouseCode, e.OperationType }, "OPERATION_FIX_SLOT_INDEX1");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.DaysOfWeek)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DAYS_OF_WEEK");
            entity.Property(e => e.EndTime)
                .HasPrecision(6)
                .HasColumnName("END_TIME");
            entity.Property(e => e.IsVip)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_VIP");
            entity.Property(e => e.OperationType)
                .HasMaxLength(50)
                .HasColumnName("OPERATION_TYPE");
            entity.Property(e => e.StartTime)
                .HasPrecision(6)
                .HasColumnName("START_TIME");
            entity.Property(e => e.SupGroupId)
                .HasPrecision(10)
                .HasColumnName("SUP_GROUP_ID");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(100)
                .HasColumnName("WAREHOUSE_CODE");

            entity.HasOne(d => d.SupGroup).WithMany(p => p.OperationFixSlots)
                .HasForeignKey(d => d.SupGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OPERATION_FIX_SLOT_SUP_GROUP");
        });

        modelBuilder.Entity<OperationTime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OPERATION_TIME_PK");

            entity.ToTable("OPERATION_TIME");

            entity.HasIndex(e => new { e.WarehouseCode, e.OperationType }, "OPERATION_TIME_INDEX1");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.EndTime)
                .HasPrecision(6)
                .HasColumnName("END_TIME");
            entity.Property(e => e.OperationType)
                .HasMaxLength(50)
                .HasColumnName("OPERATION_TYPE");
            entity.Property(e => e.StartTime)
                .HasPrecision(6)
                .HasColumnName("START_TIME");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .HasColumnName("WAREHOUSE_CODE");
        });

        modelBuilder.Entity<PoCheckInOut>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PO_CHECK_IN_OUT_PK");

            entity.ToTable("PO_CHECK_IN_OUT");

            entity.HasIndex(e => new { e.InternalTruckCheckinId, e.Status }, "PO_CHECK_IN_OUT_INDEX1");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.BookingId)
                .HasMaxLength(50)
                .HasColumnName("BOOKING_ID");
            entity.Property(e => e.DateTimeStamp)
                .HasColumnType("DATE")
                .HasColumnName("DATE_TIME_STAMP");
            entity.Property(e => e.InternalTruckCheckinId)
                .HasColumnType("NUMBER")
                .HasColumnName("INTERNAL_TRUCK_CHECKIN_ID");
            entity.Property(e => e.PoNbr)
                .HasMaxLength(50)
                .HasColumnName("PO_NBR");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("STATUS");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<PoComment>(entity =>
        {
            entity.HasKey(e => e.Po).HasName("PO_COMMENT_PK");

            entity.ToTable("PO_COMMENT");

            entity.Property(e => e.Po)
                .HasMaxLength(50)
                .HasColumnName("PO");
            entity.Property(e => e.Comment1)
                .HasMaxLength(255)
                .HasColumnName("COMMENT1");
            entity.Property(e => e.Comment2)
                .HasMaxLength(255)
                .HasColumnName("COMMENT2");
            entity.Property(e => e.Comment3)
                .HasMaxLength(255)
                .HasColumnName("COMMENT3");
            entity.Property(e => e.Comment4)
                .HasMaxLength(255)
                .HasColumnName("COMMENT4");
            entity.Property(e => e.Comment5)
                .HasMaxLength(255)
                .HasColumnName("COMMENT5");
            entity.Property(e => e.Comment6)
                .HasMaxLength(255)
                .HasColumnName("COMMENT6");
            entity.Property(e => e.Comment7)
                .HasMaxLength(255)
                .HasColumnName("COMMENT7");
            entity.Property(e => e.Comment8)
                .HasMaxLength(255)
                .HasColumnName("COMMENT8");
            entity.Property(e => e.DateTimeStamp)
                .HasColumnType("DATE")
                .HasColumnName("DATE_TIME_STAMP");
            entity.Property(e => e.InterfaceDate)
                .HasColumnType("DATE")
                .HasColumnName("INTERFACE_DATE");
        });

        modelBuilder.Entity<PoLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PO_LOG_PK");

            entity.ToTable("PO_LOG");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.DateTimeStamp)
                .HasColumnType("DATE")
                .HasColumnName("DATE_TIME_STAMP");
            entity.Property(e => e.Log)
                .HasMaxLength(500)
                .HasColumnName("LOG");
            entity.Property(e => e.PoNbr)
                .HasMaxLength(50)
                .HasColumnName("PO_NBR");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<QueueSequence>(entity =>
        {
            entity.HasKey(e => new { e.WarehouseCode, e.OperationType }).HasName("QUEUE_SEQUENCE_PK");

            entity.ToTable("QUEUE_SEQUENCE");

            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .HasColumnName("WAREHOUSE_CODE");
            entity.Property(e => e.OperationType)
                .HasMaxLength(50)
                .HasColumnName("OPERATION_TYPE");
            entity.Property(e => e.LastSequence)
                .HasColumnType("NUMBER")
                .HasColumnName("LAST_SEQUENCE");
            entity.Property(e => e.QueueDate)
                .HasColumnType("DATE")
                .HasColumnName("QUEUE_DATE");
        });

        modelBuilder.Entity<Shunt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SHUNT_PK");

            entity.ToTable("SHUNT");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.DriverName)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("DRIVER_NAME");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("LICENSE_PLATE");
            entity.Property(e => e.LocationId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("LOCATION_ID");
            entity.Property(e => e.ModDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("STATUS");
            entity.Property(e => e.TelNo)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("TEL_NO");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<SupTypeMaster>(entity =>
        {
            entity.HasKey(e => e.InternalSupTypeId).HasName("SUP_TYPE_MASTER_PK");

            entity.ToTable("SUP_TYPE_MASTER");

            entity.Property(e => e.InternalSupTypeId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_SUP_TYPE_ID");
            entity.Property(e => e.BackColor)
                .HasPrecision(10)
                .HasColumnName("BACK_COLOR");
            entity.Property(e => e.Backhaul)
                .HasPrecision(1)
                .HasColumnName("BACKHAUL");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .HasColumnName("COMPANY_CODE");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.FontColor)
                .HasPrecision(10)
                .HasColumnName("FONT_COLOR");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.SupType)
                .HasMaxLength(50)
                .HasColumnName("SUP_TYPE");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.InternalSupId).HasName("SUPPLIER_PK");

            entity.ToTable("SUPPLIER");

            entity.HasIndex(e => e.SupCode, "SUPPLIER_SUP_CODE").IsUnique();

            entity.Property(e => e.InternalSupId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_SUP_ID");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .HasColumnName("COMPANY_CODE");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.InterfaceDate)
                .HasColumnType("DATE")
                .HasColumnName("INTERFACE_DATE");
            entity.Property(e => e.InternalGroupId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_GROUP_ID");
            entity.Property(e => e.InternalSupGroupId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_SUP_GROUP_ID");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.RefDef1)
                .HasMaxLength(50)
                .HasColumnName("REF_DEF1");
            entity.Property(e => e.RefDef2)
                .HasMaxLength(50)
                .HasColumnName("REF_DEF2");
            entity.Property(e => e.RefDef3)
                .HasMaxLength(50)
                .HasColumnName("REF_DEF3");
            entity.Property(e => e.RefDef4)
                .HasMaxLength(50)
                .HasColumnName("REF_DEF4");
            entity.Property(e => e.SupCode)
                .HasMaxLength(50)
                .HasColumnName("SUP_CODE");
            entity.Property(e => e.SupName)
                .HasMaxLength(50)
                .HasColumnName("SUP_NAME");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<SupplierGroup>(entity =>
        {
            entity.HasKey(e => e.InternalSupGroupId).HasName("SUPPLIER_GROUP_PK");

            entity.ToTable("SUPPLIER_GROUP");

            entity.Property(e => e.InternalSupGroupId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_SUP_GROUP_ID");
            entity.Property(e => e.Address1)
                .HasMaxLength(150)
                .HasColumnName("ADDRESS1");
            entity.Property(e => e.Address2)
                .HasMaxLength(150)
                .HasColumnName("ADDRESS2");
            entity.Property(e => e.Address3)
                .HasMaxLength(150)
                .HasColumnName("ADDRESS3");
            entity.Property(e => e.BuyerCode)
                .HasMaxLength(50)
                .HasColumnName("BUYER_CODE");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("CITY");
            entity.Property(e => e.ComfirmGatePass)
                .HasPrecision(1)
                .HasColumnName("COMFIRM_GATE_PASS");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .HasColumnName("COMPANY_CODE");
            entity.Property(e => e.ContactEMail).HasColumnName("CONTACT_E_MAIL");
            entity.Property(e => e.ContactName)
                .HasMaxLength(60)
                .HasColumnName("CONTACT_NAME");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("COUNTRY");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.FixTime)
                .HasPrecision(1)
                .HasColumnName("FIX_TIME");
            entity.Property(e => e.FixTimeEnd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("FIX_TIME_END");
            entity.Property(e => e.FixTimeStart)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("FIX_TIME_START");
            entity.Property(e => e.InterfaceDate)
                .HasColumnType("DATE")
                .HasColumnName("INTERFACE_DATE");
            entity.Property(e => e.InternalSupTypeId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_SUP_TYPE_ID");
            entity.Property(e => e.IsUpCreateBook)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_UP_CREATE_BOOK");
            entity.Property(e => e.IsUpPreCheckin)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_UP_PRE_CHECKIN");
            entity.Property(e => e.IsVip)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_VIP");
            entity.Property(e => e.MaxBookingPreHour)
                .HasColumnType("NUMBER")
                .HasColumnName("MAX_BOOKING_PRE_HOUR");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(50)
                .HasColumnName("MOBILE_NUMBER");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.PkSupCode)
                .HasMaxLength(50)
                .HasColumnName("PK_SUP_CODE");
            entity.Property(e => e.Postpond)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("POSTPOND");
            entity.Property(e => e.Remark).HasColumnName("REMARK");
            entity.Property(e => e.Remark2)
                .HasMaxLength(550)
                .HasColumnName("REMARK2");
            entity.Property(e => e.RemarkCreateDate)
                .HasColumnType("DATE")
                .HasColumnName("REMARK_CREATE_DATE");
            entity.Property(e => e.RemarkToSup).HasColumnName("REMARK_TO_SUP");
            entity.Property(e => e.RmsCode)
                .HasMaxLength(20)
                .HasColumnName("RMS_CODE");
            entity.Property(e => e.SizeType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SIZE_TYPE");
            entity.Property(e => e.SupName)
                .HasMaxLength(150)
                .HasColumnName("SUP_NAME");
            entity.Property(e => e.UserDef1)
                .HasMaxLength(150)
                .HasColumnName("USER_DEF1");
            entity.Property(e => e.UserDef2)
                .HasMaxLength(150)
                .HasColumnName("USER_DEF2");
            entity.Property(e => e.UserDef3)
                .HasMaxLength(150)
                .HasColumnName("USER_DEF3");
            entity.Property(e => e.UserDef4)
                .HasMaxLength(150)
                .HasColumnName("USER_DEF4");
            entity.Property(e => e.UserDef5)
                .HasMaxLength(150)
                .HasColumnName("USER_DEF5");
            entity.Property(e => e.UserDef6)
                .HasMaxLength(150)
                .HasColumnName("USER_DEF6");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
            entity.Property(e => e.WarehouseCutoff)
                .HasMaxLength(500)
                .HasColumnName("WAREHOUSE_CUTOFF");
            entity.Property(e => e.Warehouses)
                .HasMaxLength(250)
                .HasColumnName("WAREHOUSES");
            entity.Property(e => e.Zipcode)
                .HasMaxLength(10)
                .HasColumnName("ZIPCODE");
        });

        modelBuilder.Entity<Trailer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TRAILER_PK");

            entity.ToTable("TRAILER");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("LICENSE_PLATE");
            entity.Property(e => e.LocationId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("LOCATION_ID");
            entity.Property(e => e.ModDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.OutDcDriver)
                .HasMaxLength(100)
                .ValueGeneratedOnAdd()
                .HasColumnName("OUT_DC_DRIVER");
            entity.Property(e => e.OutDcLicensePlate)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("OUT_DC_LICENSE_PLATE");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("STATUS");
            entity.Property(e => e.TrailerGroup)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("TRAILER_GROUP");
            entity.Property(e => e.TrailerSize)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TRAILER_SIZE");
            entity.Property(e => e.TrailerType)
                .HasMaxLength(20)
                .ValueGeneratedOnAdd()
                .HasColumnName("TRAILER_TYPE");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<TruckCap>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TRUCK_CAP");

            entity.Property(e => e.FullPl)
                .HasColumnType("NUMBER")
                .HasColumnName("FULL_PL");
            entity.Property(e => e.InternalTruckId)
                .HasColumnType("NUMBER")
                .HasColumnName("INTERNAL_TRUCK_ID");
            entity.Property(e => e.OperationType)
                .HasMaxLength(50)
                .HasColumnName("OPERATION_TYPE");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .HasColumnName("WAREHOUSE_CODE");
        });

        modelBuilder.Entity<TruckMaster>(entity =>
        {
            entity.HasKey(e => e.InternalTruckId).HasName("TRUCK_MASTER_PK");

            entity.ToTable("TRUCK_MASTER");

            entity.Property(e => e.InternalTruckId)
                .HasPrecision(10)
                .HasColumnName("INTERNAL_TRUCK_ID");
            entity.Property(e => e.Company)
                .HasMaxLength(50)
                .HasColumnName("COMPANY");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.Sequence)
                .HasMaxLength(50)
                .HasColumnName("SEQUENCE");
            entity.Property(e => e.TruckCode)
                .HasMaxLength(50)
                .HasColumnName("TRUCK_CODE");
            entity.Property(e => e.TruckGroup)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TRUCK_GROUP");
            entity.Property(e => e.TruckName)
                .HasMaxLength(50)
                .HasColumnName("TRUCK_NAME");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
        });

        modelBuilder.Entity<TruckRule>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TRUCK_RULE");

            entity.Property(e => e.InternalTruckId)
                .HasMaxLength(20)
                .HasColumnName("INTERNAL_TRUCK_ID");
            entity.Property(e => e.OperationType)
                .HasMaxLength(50)
                .HasColumnName("OPERATION_TYPE");
            entity.Property(e => e.QtyType)
                .HasMaxLength(20)
                .HasColumnName("QTY_TYPE");
            entity.Property(e => e.Warehouse)
                .HasMaxLength(50)
                .HasColumnName("WAREHOUSE");
        });

        modelBuilder.Entity<UserMaster>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("USER_MASTER_PK");

            entity.ToTable("USER_MASTER");

            entity.HasIndex(e => e.UserName, "USER_MASTER_INDEX1");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("USER_ID");
            entity.Property(e => e.Admin)
                .HasPrecision(1)
                .HasColumnName("ADMIN");
            entity.Property(e => e.Approved)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("APPROVED");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.ExpireDate)
                .HasColumnType("DATE")
                .HasColumnName("EXPIRE_DATE");
            entity.Property(e => e.InternalSupGroupId)
                .HasColumnType("NUMBER")
                .HasColumnName("INTERNAL_SUP_GROUP_ID");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("LASTNAME");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.OperationType)
                .HasMaxLength(50)
                .HasColumnName("OPERATION_TYPE");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.SendEmail)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SEND_EMAIL");
            entity.Property(e => e.SupCode)
                .HasMaxLength(50)
                .HasColumnName("SUP_CODE");
            entity.Property(e => e.Token).HasColumnName("TOKEN");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("USER_NAME");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
            entity.Property(e => e.UserType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("USER_TYPE");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .HasColumnName("WAREHOUSE_CODE");
        });

        modelBuilder.Entity<UserSupplierGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("USER_SUPPLIER_GROUP_PK");

            entity.ToTable("USER_SUPPLIER_GROUP");

            entity.Property(e => e.Id)
                .HasPrecision(10)
                .HasColumnName("ID");
            entity.Property(e => e.InternalSupGroup)
                .HasPrecision(10)
                .ValueGeneratedOnAdd()
                .HasColumnName("INTERNAL_SUP_GROUP");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("USER_ID");
        });

        modelBuilder.Entity<UserWarehouse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("USER_WAREHOUSE_PK");

            entity.ToTable("USER_WAREHOUSE");

            entity.Property(e => e.Id)
                .HasPrecision(10)
                .HasColumnName("ID");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("USER_ID");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("WAREHOUSE_CODE");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseCode).HasName("WAREHOUSE_PK");

            entity.ToTable("WAREHOUSE");

            entity.HasIndex(e => new { e.WarehouseCode, e.CompanyCode }, "WAREHOUSE_INDEX1");

            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .HasColumnName("WAREHOUSE_CODE");
            entity.Property(e => e.Active)
                .HasPrecision(1)
                .HasColumnName("ACTIVE");
            entity.Property(e => e.Address1)
                .HasMaxLength(250)
                .HasColumnName("ADDRESS1");
            entity.Property(e => e.Address2)
                .HasMaxLength(250)
                .HasColumnName("ADDRESS2");
            entity.Property(e => e.Address3)
                .HasMaxLength(250)
                .HasColumnName("ADDRESS3");
            entity.Property(e => e.AdvanceBookingDay)
                .HasPrecision(6)
                .HasColumnName("ADVANCE_BOOKING_DAY");
            entity.Property(e => e.AdvanceBookingPeriod)
                .HasPrecision(6)
                .HasColumnName("ADVANCE_BOOKING_PERIOD");
            entity.Property(e => e.AdvanceCheckinTime)
                .HasPrecision(6)
                .HasColumnName("ADVANCE_CHECKIN_TIME");
            entity.Property(e => e.BookingIdPrefix)
                .HasMaxLength(50)
                .HasColumnName("BOOKING_ID_PREFIX");
            entity.Property(e => e.BookingIdRunning)
                .HasPrecision(10)
                .HasColumnName("BOOKING_ID_RUNNING");
            entity.Property(e => e.CapUom)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CAP_UOM");
            entity.Property(e => e.CapacityType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CAPACITY_TYPE");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("CITY");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .HasColumnName("COMPANY_CODE");
            entity.Property(e => e.ContactEMail)
                .HasMaxLength(50)
                .HasColumnName("CONTACT_E_MAIL");
            entity.Property(e => e.ContactName)
                .HasMaxLength(50)
                .HasColumnName("CONTACT_NAME");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("COUNTRY");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.DateUpdateRunning)
                .HasColumnType("DATE")
                .HasColumnName("DATE_UPDATE_RUNNING");
            entity.Property(e => e.EndTimeOfDay)
                .HasPrecision(10)
                .HasColumnName("END_TIME_OF_DAY");
            entity.Property(e => e.FirstTimeOfDay)
                .HasPrecision(10)
                .HasColumnName("FIRST_TIME_OF_DAY");
            entity.Property(e => e.FixDoor)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("FIX_DOOR");
            entity.Property(e => e.LateCheckinTime)
                .HasPrecision(6)
                .HasColumnName("LATE_CHECKIN_TIME");
            entity.Property(e => e.MaxAllPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_ALL_PER_HOUR");
            entity.Property(e => e.MaxCAllPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_ALL_PER_HOUR");
            entity.Property(e => e.MaxCConPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_CON_PER_HOUR");
            entity.Property(e => e.MaxCFullPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_FULL_PER_HOUR");
            entity.Property(e => e.MaxCNonPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_NON_PER_HOUR");
            entity.Property(e => e.MaxConPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_CON_PER_HOUR");
            entity.Property(e => e.MaxFullPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_FULL_PER_HOUR");
            entity.Property(e => e.MaxNonPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_NON_PER_HOUR");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(50)
                .HasColumnName("MOBILE_NUMBER");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.Note)
                .HasMaxLength(550)
                .HasColumnName("NOTE");
            entity.Property(e => e.OnlineBookingIdPrefix)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ONLINE_BOOKING_ID_PREFIX");
            entity.Property(e => e.OnlineBookingIdRunning)
                .HasColumnType("NUMBER")
                .HasColumnName("ONLINE_BOOKING_ID_RUNNING");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.PoAfterPeriod)
                .HasPrecision(6)
                .HasColumnName("PO_AFTER_PERIOD");
            entity.Property(e => e.PoBeforePeriod)
                .HasPrecision(6)
                .HasColumnName("PO_BEFORE_PERIOD");
            entity.Property(e => e.SendCallTruckSms)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SEND_CALL_TRUCK_SMS");
            entity.Property(e => e.SendReceiveDocSms)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SEND_RECEIVE_DOC_SMS");
            entity.Property(e => e.TimeIncreaseStep)
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("TIME_INCREASE_STEP");
            entity.Property(e => e.TimeWidth)
                .HasPrecision(10)
                .HasColumnName("TIME_WIDTH");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
            entity.Property(e => e.WarehouseLevel)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("WAREHOUSE_LEVEL");
            entity.Property(e => e.WarehouseMain)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WAREHOUSE_MAIN");
            entity.Property(e => e.WarehouseName)
                .HasMaxLength(150)
                .HasColumnName("WAREHOUSE_NAME");
            entity.Property(e => e.WarehouseWms)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WAREHOUSE_WMS");
            entity.Property(e => e.Zipcode)
                .HasMaxLength(50)
                .HasColumnName("ZIPCODE");
        });

        modelBuilder.Entity<WarehouseCapacity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("WAREHOUSE_CAPACITY_PK");

            entity.ToTable("WAREHOUSE_CAPACITY");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.BookingDate)
                .HasColumnType("DATE")
                .HasColumnName("BOOKING_DATE");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COMPANY_CODE");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.MaxAllPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_ALL_PER_HOUR");
            entity.Property(e => e.MaxCAllPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_ALL_PER_HOUR");
            entity.Property(e => e.MaxCConPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_CON_PER_HOUR");
            entity.Property(e => e.MaxCFullPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_FULL_PER_HOUR");
            entity.Property(e => e.MaxCNonPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_NON_PER_HOUR");
            entity.Property(e => e.MaxConPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_CON_PER_HOUR");
            entity.Property(e => e.MaxFullPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_FULL_PER_HOUR");
            entity.Property(e => e.MaxNonPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_NON_PER_HOUR");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .HasColumnName("WAREHOUSE_CODE");
        });

        modelBuilder.Entity<WarehouseOperationCapacity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("WAREHOUSE_OPERATION_CAPACITY_PK");

            entity.ToTable("WAREHOUSE_OPERATION_CAPACITY");

            entity.Property(e => e.Id)
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.BookingDate)
                .HasColumnType("DATE")
                .HasColumnName("BOOKING_DATE");
            entity.Property(e => e.CreateDate)
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.MaxAllPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_ALL_PER_HOUR");
            entity.Property(e => e.MaxCAllPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_ALL_PER_HOUR");
            entity.Property(e => e.MaxCConPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_CON_PER_HOUR");
            entity.Property(e => e.MaxCFullPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_FULL_PER_HOUR");
            entity.Property(e => e.MaxCNonPerHour)
                .HasColumnType("NUMBER(18,4)")
                .HasColumnName("MAX_C_NON_PER_HOUR");
            entity.Property(e => e.MaxConPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_CON_PER_HOUR");
            entity.Property(e => e.MaxFullPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_FULL_PER_HOUR");
            entity.Property(e => e.MaxNonPerHour)
                .HasPrecision(10)
                .HasColumnName("MAX_NON_PER_HOUR");
            entity.Property(e => e.ModDate)
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.OperationType)
                .HasMaxLength(50)
                .HasColumnName("OPERATION_TYPE");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .HasColumnName("USER_STAMP");
            entity.Property(e => e.WarehouseCode)
                .HasMaxLength(50)
                .HasColumnName("WAREHOUSE_CODE");
        });

        modelBuilder.Entity<Yard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("YARD_PK");

            entity.ToTable("YARD");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.LocationNo)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("LOCATION_NO");
            entity.Property(e => e.ModDate)
                .ValueGeneratedOnAdd()
                .HasColumnType("DATE")
                .HasColumnName("MOD_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("STATUS");
            entity.Property(e => e.TrailerType)
                .HasMaxLength(100)
                .ValueGeneratedOnAdd()
                .HasColumnName("TRAILER_TYPE");
            entity.Property(e => e.UserStamp)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("USER_STAMP");
            entity.Property(e => e.YardNo)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("YARD_NO");
            entity.Property(e => e.YardType)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("YARD_TYPE");
            entity.Property(e => e.YardZone)
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasColumnName("YARD_ZONE");
        });
        modelBuilder.HasSequence("BOOKING_INTERFACE_SEQ");
        modelBuilder.HasSequence("BOOKING_TRUCK_CHECK_IN_DTL_SEQ");
        modelBuilder.HasSequence("BOOKING_TRUCK_CHECK_IN_SEQ");
        modelBuilder.HasSequence("BOOKING_TRUCK_LOG_SEQ");
        modelBuilder.HasSequence("DBOBJECTID_SEQUENCE").IncrementsBy(50);
        modelBuilder.HasSequence("JOB_SEQ");
        modelBuilder.HasSequence("OPERATION_TIME_SEQ");
        modelBuilder.HasSequence("PO_CHECK_IN_OUT_SEQ");
        modelBuilder.HasSequence("PO_LOG_SEQ1");
        modelBuilder.HasSequence("SHUNT_SEQ");
        modelBuilder.HasSequence("TRAILER_SEQ");
        modelBuilder.HasSequence("TT_BOOKING_DETAIL_INTERNAL_");
        modelBuilder.HasSequence("TT_BOOKING_HEADER_INTERNAL_");
        modelBuilder.HasSequence("TT_BOOKING_KEY_INTERNAL_KEY");
        modelBuilder.HasSequence("TT_CUSTOM_ALERT_ALERT_ID");
        modelBuilder.HasSequence("TT_DOOR_INTERNAL_DOOR_ID").IncrementsBy(659);
        modelBuilder.HasSequence("TT_EST_TIME_INTERNAL_EST_ID");
        modelBuilder.HasSequence("TT_FIX_TIME_DATE_FIX_TIME_D");
        modelBuilder.HasSequence("TT_FIX_TIME_MASTER_FIX_TIME");
        modelBuilder.HasSequence("TT_GP_BOOKING_REJECT_REJECT");
        modelBuilder.HasSequence("TT_GP_CONFIRM_E_MAIL_TRANSA");
        modelBuilder.HasSequence("TT_GP_REC_MAIL_CONTACT_REC_");
        modelBuilder.HasSequence("TT_GP_REC_MAIL_DTL_REC_DTL_");
        modelBuilder.HasSequence("TT_GP_REC_MAIL_HDR_REC_ID");
        modelBuilder.HasSequence("TT_GP_REC_MAIL_TRUCK_REC_TR");
        modelBuilder.HasSequence("TT_MSG_LOG_LOG_ID");
        modelBuilder.HasSequence("TT_MT_PO_LIST_DTL_INTERNAL_");
        modelBuilder.HasSequence("TT_MT_PO_LIST_INTERNAL_PO_N");
        modelBuilder.HasSequence("TT_REASON_CONFIRM_INTERNAL_");
        modelBuilder.HasSequence("TT_SUP_TYPE_MASTER_INTERNAL");
        modelBuilder.HasSequence("TT_SUPPLIER_GROUP_INTERNAL_");
        modelBuilder.HasSequence("TT_SUPPLIER_INTERNAL_SUP_ID");
        modelBuilder.HasSequence("TT_TRUCK_MASTER_INTERNAL_TR");
        modelBuilder.HasSequence("TT_USER_ACCRESS_MENU_ID");
        modelBuilder.HasSequence("TT_USER_WAREHOUSE_USER_WHSE");
        modelBuilder.HasSequence("USER_SUPPLIER_GROUP_SEQ");
        modelBuilder.HasSequence("USER_WAREHOUSE_SEQ");
        modelBuilder.HasSequence("USER_WAREHOUSE_SEQ1");
        modelBuilder.HasSequence("WAREHOUSE_CAPACITY_SEQ");
        modelBuilder.HasSequence("WHSE_OPERATION_CAP_SEQ");
        modelBuilder.HasSequence("YARD_SEQ");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
