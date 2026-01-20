using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;

namespace Makro.IMS.Infra.Data.UnitOfWorks
{
    public class UnitOfWork : IDisposable
    {
        private readonly IMSContext _context;
        private IDbContextTransaction _transaction;

        // Declare private fields for each repository
        private WarehouseRepository warehouseRepository;
        private CompanyRepository companyRepository;
        private DoorRepository doorRepository;
        private TruckMasterRepository truckMasterRepository;
        private WarehouseCapacityRepository warehouseCapacityRepository;
        private SupplierRepository supplierRepository;
        private SupplierGroupRepository supplierGroupRepository;
        private SupplierTypeRepository supplierTypeRepository;
        private EstTimeRepository estTimeRepository;
        private UserMasterRepository userMasterRepository;
        private BookingKeyRepository bookingKeyRepository;
        private BookingHeaderRepository bookingHeaderRepository;
        private BookingDetailRepository bookingDetailRepository;
        private BookingTruckRepository bookingTruckRepository;
        private BookingTruckCheckInRepository bookingTruckCheckInRepository;
        private BookingTruckCheckInDetailRepository bookingTruckCheckInDetailRepository;
        private PoListRepository poListRepository;
        private WarehouseOperationCapacityRepository warehouseOperationCapacityRepository;
        private OperationRepository operationRepository;
        private OperationTimeRepository operationTimeRepository;
        private OperationCapacityRepository operationCapacityRepository;
        private OperationFixSlotRepository operationFixSlotRepository;
        private QueueSequenceRepository queueSequenceRepository;
        private PoCheckInOutRepository poCheckInOutRepository;
        private ReportRepository reportRepository;
        private BookingTruckLogRepository bookingTruckLogRepository;
        private SlotCapacityRepository slotCapacityRepository;
        private TruckCapRepository truckCapRepository;
        private PoCommentRepository poCommentRepository;
        private PoLogRepository poLogRepository;
        private BookingInterfaceRepository bookingInterfaceRepository;
        private TruckRuleRepository truckRuleRepository;
        private TrailerRepository trailerRepository;
        private YardRepository yardRepository;
        private ShuntRepository shuntRepository;
        private JobRepository jobRepository;
        private UserWarehouseRepository userWarehouseRepository;
        private UserSupplierGroupRepository userSupplierGroupRepository;
        

        private T GetOrCreateRepository<T>(ref T repository) where T : class
        {
            if (repository == null)
            {
                repository = (T)Activator.CreateInstance(typeof(T), _context);
            }
            return repository;
        }

        public UnitOfWork(IMSContext context)
        {
            _context = context;
        }

        public WarehouseRepository WarehouseRepository => GetOrCreateRepository(ref warehouseRepository);
        public CompanyRepository CompanyRepository => GetOrCreateRepository(ref companyRepository);
        public DoorRepository DoorRepository => GetOrCreateRepository(ref doorRepository);
        public TruckMasterRepository TruckMasterRepository => GetOrCreateRepository(ref truckMasterRepository);
        public WarehouseCapacityRepository WarehouseCapacityRepository => GetOrCreateRepository(ref warehouseCapacityRepository);
        public SupplierRepository SupplierRepository => GetOrCreateRepository(ref supplierRepository);
        public SupplierGroupRepository SupplierGroupRepository => GetOrCreateRepository(ref supplierGroupRepository);
        public SupplierTypeRepository SupplierTypeRepository => GetOrCreateRepository(ref supplierTypeRepository);
        public EstTimeRepository EstTimeRepository => GetOrCreateRepository(ref estTimeRepository);
        public UserMasterRepository UserMasterRepository => GetOrCreateRepository(ref userMasterRepository);
        public BookingKeyRepository BookingKeyRepository => GetOrCreateRepository(ref bookingKeyRepository);
        public BookingHeaderRepository BookingHeaderRepository => GetOrCreateRepository(ref bookingHeaderRepository);
        public BookingDetailRepository BookingDetailRepository => GetOrCreateRepository(ref bookingDetailRepository);
        public BookingTruckRepository BookingTruckRepository => GetOrCreateRepository(ref bookingTruckRepository);
        public BookingTruckCheckInRepository BookingTruckCheckInRepository => GetOrCreateRepository(ref bookingTruckCheckInRepository);
        public BookingTruckCheckInDetailRepository BookingTruckCheckInDetailRepository => GetOrCreateRepository(ref bookingTruckCheckInDetailRepository);
        public PoListRepository PoListRepository => GetOrCreateRepository(ref poListRepository);
        public WarehouseOperationCapacityRepository WarehouseOperationCapacityRepository => GetOrCreateRepository(ref warehouseOperationCapacityRepository);
        public OperationRepository OperationRepository => GetOrCreateRepository(ref operationRepository);
        public OperationTimeRepository OperationTimeRepository => GetOrCreateRepository(ref operationTimeRepository);
        public OperationCapacityRepository OperationCapacityRepository => GetOrCreateRepository(ref operationCapacityRepository);
        public OperationFixSlotRepository OperationFixSlotRepository => GetOrCreateRepository(ref operationFixSlotRepository);
        public QueueSequenceRepository QueueSequenceRepository => GetOrCreateRepository(ref queueSequenceRepository);
        public PoCheckInOutRepository PoCheckInOutRepository => GetOrCreateRepository(ref poCheckInOutRepository);
        public ReportRepository ReportRepository => GetOrCreateRepository(ref reportRepository);
        public BookingTruckLogRepository BookingTruckLogRepository => GetOrCreateRepository(ref bookingTruckLogRepository);
        public SlotCapacityRepository SlotCapacityRepository => GetOrCreateRepository(ref slotCapacityRepository);
        public TruckCapRepository TruckCapRepository => GetOrCreateRepository(ref truckCapRepository);
        public PoCommentRepository PoCommentRepository => GetOrCreateRepository(ref poCommentRepository);
        public PoLogRepository PoLogRepository => GetOrCreateRepository(ref poLogRepository);
        public BookingInterfaceRepository BookingInterfaceRepository => GetOrCreateRepository(ref bookingInterfaceRepository);
        public TruckRuleRepository TruckRuleRepository => GetOrCreateRepository(ref truckRuleRepository);
        public TrailerRepository TrailerRepository => GetOrCreateRepository(ref trailerRepository);
        public YardRepository YardRepository => GetOrCreateRepository(ref yardRepository);
        public ShuntRepository ShuntRepository => GetOrCreateRepository(ref shuntRepository);
        public JobRepository JobRepository => GetOrCreateRepository(ref jobRepository);
        public UserWarehouseRepository UserWarehouseRepository => GetOrCreateRepository(ref userWarehouseRepository);
        public UserSupplierGroupRepository UserSupplierGroupRepository => GetOrCreateRepository(ref userSupplierGroupRepository);

        public void Save()
        {
            _context.SaveChanges();
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = _context.Database.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress to commit.");
            }

            try
            {
                //  _context.SaveChanges();
                _transaction.Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Rollback()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress to rollback.");
            }

            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
