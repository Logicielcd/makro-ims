using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Sieve.Services;

namespace Makro.IMS.Services.Api.Services
{
    public class JobOnDockService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public JobOnDockService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
        }

        public JobOnDockService(ISieveProcessor sieveProcessor)
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<List<JobOnDock>> GetJobByYardOnDock()
        {
            var jobs = await Task.Run(() => unitOfWork.JobRepository.GetAllJobs());
            var yards = await Task.Run(() => unitOfWork.YardRepository.GetYards());
            var trailers = await Task.Run(() => unitOfWork.TrailerRepository.GetTrailers());
            var dockYards = yards.Where(y => y.YardType == "Dock").ToList();
            var activeJobs = jobs.Where(j => !new[] { "Completed", "Cancelled" }.Contains(j.Status)).ToList();

            var result = (from yard in dockYards
                          let latestActiveJob = activeJobs
                              .Where(j => j.LocationId == yard.Id)
                              .OrderByDescending(j => j.CreateDate)
                              .FirstOrDefault()
                          let trailer = latestActiveJob != null
                              ? trailers.FirstOrDefault(t => t.Id == latestActiveJob.TrailerId)
                              : null
                          // เช็คเงื่อนไขการเปลี่ยน trailer แบบใหม่ 
                          let isChangeTrailer = latestActiveJob == null && // ไม่มี active job
                                              yard.Status == "Not Available" && // yard ไม่ว่าง
                                              trailers.Any(t => t.Status == "On-Dock") // มี trailer ที่มีสถานะ On-Dock
                          select new JobOnDock
                          {
                              LocationId = yard.Id,
                              LocationNo = yard.YardNo,
                              LocationZone = yard.YardZone,
                              LocationType = yard.YardType,
                              LocationStatus = yard.Status,

                              Id = latestActiveJob?.Id ?? 0,
                              JobId = latestActiveJob?.JobId,
                              Status = isChangeTrailer
                                  ? "Change Trailer"
                                  : latestActiveJob?.Status,

                              TrailerId = trailer?.Id,
                              TrailerLicensePlate = trailer?.LicensePlate,
                              TrailerStatus = trailer?.Status,

                              CreateDate = latestActiveJob?.CreateDate,
                              ModDate = latestActiveJob?.ModDate,

                              TotalProcessTime = CalculateProcessTime(latestActiveJob)
                          }).ToList();

            return result;
        }

        private string CalculateProcessTime(Job? latestActiveJob)
        {
            if (latestActiveJob == null)
            {
                return null;
            }

            DateTime startDate = (DateTime)latestActiveJob.CreateDate;
            DateTime endDate;

            // ถ้า Job สถานะ Completed ใช้ ModDate ในการคำนวณ
            // ถ้าไม่ใช่ ใช้เวลาปัจจุบันในการคำนวณ
            if (latestActiveJob.Status == "Completed")
            {
                endDate = latestActiveJob.ModDate ?? startDate;
            }
            else
            {
                endDate = DateTime.Now;
            }

            TimeSpan duration = endDate - startDate;

            int totalDays = duration.Days;
            int totalHours = duration.Hours;
            int totalMinutes = duration.Minutes;
            int totalSeconds = duration.Seconds;

            return $"{totalDays}วัน {totalHours}ชั่วโมง {totalMinutes}นาที {totalSeconds}วินาที";
        }

        public async Task<bool> ChangeDoor(ChangeDoorRequestDto changeDoor)
        {
            try
            {
                // 1. Validate Job
                Job check_job = unitOfWork.JobRepository.GetJobById(changeDoor.JobId)
                    ?? throw new Exception("Job not found.");
                if (check_job.Status != "On-Dock")
                    throw new Exception("Job cannot change door.");

                // 2. Validate new Yard location
                Yard check_location_new = unitOfWork.YardRepository.GetById(changeDoor.LocationId)
                    ?? throw new Exception("Yard New not found.");

                // 2.1 Check if new location is allocated to other active jobs
                Job check_job_in_new_location = unitOfWork.JobRepository.GetAllJobs()
                    .FirstOrDefault(x => x.LocationId == check_location_new.Id &&
                                   (x.Status == "Completed" || x.Status == "Cancelled"));

                // 3. Check if new location is available
                if (check_location_new.YardType != "Dock")
                {
                    //if (check_location_new.Status != "Empty")
                    if (check_location_new.Status != "Available")
                        throw new Exception("Location new is not empty.");
                }

                // 4. Validate Trailer
                Trailer check_trailer_job = unitOfWork.TrailerRepository.GetById(check_job.TrailerId ?? 0)
                    ?? throw new Exception("Trailer in job not found.");

                // 5. Get and validate old Yard
                Yard check_yard_in_trailer_job = unitOfWork.YardRepository.GetById(check_trailer_job.LocationId ?? 0)
                    ?? throw new Exception("Yard old not found.");

                // Start transaction
                unitOfWork.BeginTransaction();

                try
                {
                    // 6. Update old Yard status
                    //check_yard_in_trailer_job.Status = "Empty";
                    check_yard_in_trailer_job.Status = "Available";
                    await Task.Run(() => unitOfWork.YardRepository.Update(check_yard_in_trailer_job));

                    // 7. Update new Yard status if not Dock
                    //check_location_new.Status = "Full";
                    check_location_new.Status = "Not Available";
                    await Task.Run(() => unitOfWork.YardRepository.Update(check_location_new));

                    // 8. Update Trailer location
                    check_trailer_job.LocationId = check_location_new.Id;
                    await Task.Run(() => unitOfWork.TrailerRepository.Update(check_trailer_job));

                    // 9. Update Job
                    check_job.LocationId = check_location_new.Id;
                    check_job.LocationType = check_location_new.LocationNo;
                    check_job.ModDate = DateTime.UtcNow;
                    await Task.Run(() => unitOfWork.JobRepository.UpdateJob(check_job));

                    // 10. Save all changes
                    unitOfWork.Save();
                    unitOfWork.Commit();
                    return true;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ConfirmLoading(Job job, string user)
        {
            Job check_job = unitOfWork.JobRepository.GetJobById(job.Id) ?? throw new Exception("Job not found.");
            if (check_job.Status != "On-Dock")
                throw new Exception("Job cannot be loading.");

            check_job.Status = "Loading";
            check_job.UserStamp = user;
            check_job.ModDate = DateTime.UtcNow;

            await Task.Run(() => unitOfWork.JobRepository.UpdateJob(check_job));
            unitOfWork.Save();
            return true;
        }

        public async Task<bool> ConfirmLoaded(Job job, string user)
        {
            Job check_job = unitOfWork.JobRepository.GetJobById(job.Id) ?? throw new Exception("Job not found.");
            if (check_job.Status != "Loading")
                throw new Exception("Job cannot be loaded.");

            check_job.Status = "Loaded";
            check_job.UserStamp = user;
            check_job.ModDate = DateTime.UtcNow;

            await Task.Run(() => unitOfWork.JobRepository.UpdateJob(check_job));
            unitOfWork.Save();
            return true;
        }

        public async Task<bool> ConfirmComplete(Job job, string user)
        {
            try
            {
                Job check_job = unitOfWork.JobRepository.GetJobById(job.Id);
                if (job == null)
                {
                    throw new Exception("Job not found.");
                }
                if (check_job.Status != "Loaded")
                {
                    throw new Exception("Job cannot complete.");
                }

                Trailer check_trailer = unitOfWork.TrailerRepository.GetById((int)(check_job.TrailerId ?? 0));
                if (check_trailer != null)
                {
                    check_trailer.Status = "Full";
                    check_trailer.UserStamp = user;
                    check_trailer.ModDate = DateTime.UtcNow;
                    await Task.Run(() => unitOfWork.TrailerRepository.Update(check_trailer));
                }

                check_job.Status = "Completed";
                check_job.UserStamp = user;
                check_job.ModDate = DateTime.UtcNow;
                await Task.Run(() => unitOfWork.JobRepository.UpdateJob(check_job));

                unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ChangeTrailerOnDock(ChangeTrailerOnDockDto request, string user)
        {
            Job check_job = unitOfWork.JobRepository.GetJobId(request.JobId) ?? throw new Exception("Job not found.");
            if (check_job.Status != "On-Dock")
            {
                throw new Exception("Job cannot change trailer.");
            }

            Yard check_location_old = unitOfWork.YardRepository.GetById((int)(check_job.LocationId ?? 0)) ?? throw new Exception("Location not found");

            Trailer check_trailer = unitOfWork.TrailerRepository.GetById(request.TrailerId) ?? throw new Exception("Trailer not found");

            Shunt check_shunt = unitOfWork.ShuntRepository.GetById(request.ShuntId) ?? throw new Exception("Shunt not found");

            if (check_shunt.Status != "Available")
            {
                throw new Exception("Shunt not available");
            }

            Yard check_location_new = unitOfWork.YardRepository.GetById(request.LocationId) ?? throw new Exception("Location not found");

            try
            {
                // Generate new JobId
                string newJobId;
                do
                {
                    newJobId = unitOfWork.JobRepository.GetNextJobNumber();
                } while (unitOfWork.JobRepository.IsJobIdExists(newJobId));

                Yard trailer_location_type = unitOfWork.YardRepository.GetById((int)(check_trailer.LocationId ?? 0));
                string jobType = "ย้ายตู้เปล่า";

                // Update job old status
                check_job.Status = "Cancelled";
                check_job.UserStamp = user;
                check_job.ModDate = DateTime.UtcNow;
                await Task.Run(() => unitOfWork.JobRepository.UpdateJob(check_job));

                // Update location old status
                //check_location_old.Status = "Available";
                check_location_old.UserStamp = user;
                check_location_old.ModDate = DateTime.UtcNow;
                await Task.Run(() => unitOfWork.YardRepository.Update(check_location_old));

                // Update Shunt status
                check_shunt.Status = "In-Use";
                check_shunt.UserStamp = user;
                check_shunt.ModDate = DateTime.UtcNow;
                await Task.Run(() => unitOfWork.ShuntRepository.Update(check_shunt));

                // Add job
                // Set job properties
                Job add_job = new()
                {
                    JobId = newJobId,
                    JobType = jobType,
                    JobDate = DateTime.UtcNow,
                    TrailerId = request.TrailerId,
                    ShuntId = request.ShuntId,
                    LocationId = request.LocationId,
                    LocationType = check_location_new.LocationNo,
                    Status = "New",
                    CreateDate = DateTime.UtcNow,
                    UserStamp = user,
                };

                await Task.Run(() => unitOfWork.JobRepository.AddJob(add_job));
                unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
