using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Sieve.Services;

namespace Makro.IMS.Services.Api.Services
{
    public class JobService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public JobService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
        }

        public JobService(ISieveProcessor sieveProcessor)
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PagedResult<Job>> GetJobPaged(SieveModel sieveModel)
        {
            var queryable = unitOfWork.JobRepository.GetJobPaged();
            return await _sieveProcessor.GetPagedAsync<Job>(queryable, sieveModel);
        }

        public async Task<List<JobDto>> GetJobs()
        {
            // Get jobs with related data
            var jobs = await Task.Run(() => unitOfWork.JobRepository.GetAllJobs());

            // Get related data repositories
            var trailers = unitOfWork.TrailerRepository.GetTrailers();
            var shunts = unitOfWork.ShuntRepository.GetShunts();
            var yards = unitOfWork.YardRepository.GetYards();

            // Map and join data
            var result = (from job in jobs
                          join trailer in trailers
                              on job.TrailerId equals trailer.Id into trailerJoin
                          from trailer in trailerJoin.DefaultIfEmpty()
                          join shunt in shunts
                              on job.ShuntId equals shunt.Id into shuntJoin
                          from shunt in shuntJoin.DefaultIfEmpty()
                          join yard in yards
                              on job.LocationId equals yard.Id into yardJoin
                          from yard in yardJoin.DefaultIfEmpty()
                          join fromYard in yards
                              on trailer.LocationId equals fromYard.Id into fromYardJoin
                          from fromYard in fromYardJoin.DefaultIfEmpty()
                          select new JobDto
                          {
                              Id = job.Id,
                              JobId = job.JobId,
                              JobType = job.JobType,
                              JobDate = job.JobDate,
                              TrailerId = job.TrailerId,
                              TrailerLicensePlate = trailer != null ? trailer.LicensePlate : null,
                              TrailerType = trailer != null ? trailer.TrailerType : null,
                              FromYardId = fromYard != null ? fromYard.Id : null,
                              FromYardNo = fromYard != null ? fromYard.YardNo : null,
                              FromYardType = fromYard != null ? fromYard.YardType : null,
                              ShuntId = job.ShuntId,
                              ShuntLicensePlate = shunt != null ? shunt.LicensePlate : null,
                              ShuntDriver = shunt != null ? shunt.DriverName : null,
                              ShuntTel = shunt != null ? shunt.TelNo : null,
                              YardId = job.LocationId,
                              YardNo = yard != null ? yard.YardNo : null,
                              YardLocationNo = yard != null ? yard.LocationNo : null,
                              YardType = yard != null ? yard.YardType : null,
                              YardZone = yard != null ? yard.YardZone : null,
                              LocationType = job.LocationType,
                              Status = job.Status,
                              CreateDate = job.CreateDate,
                              UserStamp = job.UserStamp,
                              ModDate = job.ModDate
                          }).OrderByDescending(x => x.CreateDate).ToList();

            return result;
        }

        public async Task<Job?> GetById(int Id)
        {
            return await Task.Run<Job?>(() => unitOfWork.JobRepository.GetJobById(Id));
        }

        public async Task<bool> Add(Job job, string user)
        {
            Trailer check_trailer = unitOfWork.TrailerRepository.GetById((int)(job.TrailerId ?? 0));
            if (check_trailer == null)
            {
                throw new Exception("Trailer not found");
            }

            Shunt check_shunt = unitOfWork.ShuntRepository.GetById((int)(job.ShuntId ?? 0));
            if (check_shunt == null)
            {
                throw new Exception("Shunt not found");
            }
            if (check_shunt.Status != "Available")
            {
                throw new Exception("Shunt not available");
            }

            Yard check_location = unitOfWork.YardRepository.GetById((int)(job.LocationId ?? 0));
            if (check_location == null)
            {
                throw new Exception("Location not found");
            }

            try
            {
                // Generate new JobId
                string newJobId;
                do
                {
                    newJobId = unitOfWork.JobRepository.GetNextJobNumber();
                } while (unitOfWork.JobRepository.IsJobIdExists(newJobId));

                Yard trailer_location_type = unitOfWork.YardRepository.GetById((int)(check_trailer.LocationId ?? 0));
                string jobType = DetermineJobType(trailer_location_type.YardType, check_location.YardType);

                // Set job properties
                Job add_job = new()
                {
                    JobId = newJobId,
                    JobType = jobType,
                    JobDate = DateTime.UtcNow,
                    TrailerId = job.TrailerId,
                    ShuntId = job.ShuntId,
                    LocationId = job.LocationId,
                    LocationType = job.LocationType,
                    Status = "New",
                    CreateDate = DateTime.UtcNow,
                    UserStamp = user,
                };

                // Update Shunt status
                check_shunt.Status = "In-Use";
                check_shunt.UserStamp = user;
                check_shunt.ModDate = DateTime.UtcNow;
                await Task.Run(() => unitOfWork.ShuntRepository.Update(check_shunt));
                unitOfWork.Save();

                // Add job
                await Task.Run(() => unitOfWork.JobRepository.AddJob(add_job));
                unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding job: {ex.Message}");
            }
        }

        public async Task<bool> ConfirmAssignJob(Job job, string user)
        {
            Job check_job = unitOfWork.JobRepository.GetJobById(job.Id);
            if (check_job == null)
                throw new Exception("Job not found.");

            if (check_job.Status != "New")
                throw new Exception("Job cannot be assigned.");

            var get_shunt = unitOfWork.ShuntRepository.GetById(check_job.ShuntId ?? 0)
                ?? throw new Exception("Shunt not found.");

            var get_trailer = unitOfWork.TrailerRepository.GetById(check_job.TrailerId ?? 0)
                ?? throw new Exception("Trailer not found.");

            var get_location_origin = unitOfWork.YardRepository.GetById(get_trailer.LocationId ?? 0)
                ?? throw new Exception("Origin location not found.");

            var get_location_destination = unitOfWork.YardRepository.GetById(check_job.LocationId ?? 0)
                ?? throw new Exception("Destination location not found.");

            // send sms
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var smsSetting = config.GetSection("SmsSetting");
            string smsMsg = smsSetting.GetChildren().FirstOrDefault(x => x.Key == "AssignJobMsg").Value;
            SmsService smsService = new SmsService();
            SmsDto sms = new SmsDto();

            sms.TelNo = "66" + get_shunt.TelNo.Substring(1, 9);
            sms.SmsMessage = string.Format(smsMsg,
                check_job.JobId,
                get_shunt.LicensePlate,
                get_shunt.DriverName,
                get_trailer.LicensePlate,
                get_location_origin.YardNo,
                get_location_destination.YardNo);

            var result = await smsService.SendSms(sms);

            check_job.Status = "Assigned";
            check_job.UserStamp = user;
            check_job.ModDate = DateTime.UtcNow;

            await Task.Run(() => unitOfWork.JobRepository.UpdateJob(check_job));
            unitOfWork.Save();
            return true;
        }

        public async Task<bool> ConfirmStartJob(Job job, string user)
        {
            try
            {
                Job check_job = unitOfWork.JobRepository.GetJobById(job.Id);
                if (check_job == null)
                {
                    throw new Exception("Job not found.");
                }
                if (check_job.Status != "Assigned")
                {
                    throw new Exception("Job cannot assigned.");
                }
                Yard check_location = unitOfWork.YardRepository.GetById((int)(check_job.LocationId ?? 0));
                if (check_location == null)
                {
                    throw new Exception("Location not found");
                }
                if (check_location.YardType == "Dock")
                {
                    // จัดการ Shunt
                    Shunt get_shunt = unitOfWork.ShuntRepository.GetById((int)(check_job.ShuntId ?? 0));
                    if (get_shunt != null)
                    {
                        get_shunt.Status = "Available";
                        get_shunt.UserStamp = user;
                        get_shunt.ModDate = DateTime.UtcNow;
                        await Task.Run(() => unitOfWork.ShuntRepository.Update(get_shunt));
                    }
                    // จัดการ Trailer
                    Trailer trailer = unitOfWork.TrailerRepository.GetById((int)(check_job.TrailerId ?? 0));
                    if (trailer != null)
                    {
                        // อัพเดทสถานะ yard เก่า
                        Yard oldYard = unitOfWork.YardRepository.GetById((int)trailer.LocationId);
                        if (oldYard != null)
                        {
                            //oldYard.Status = "Empty";
                            oldYard.Status = "Available";
                            oldYard.UserStamp = user;
                            oldYard.ModDate = DateTime.UtcNow;
                            await Task.Run(() => unitOfWork.YardRepository.Update(oldYard));
                        }
                        // อัพเดทสถานะ yard ใหม่
                        Yard newYard = unitOfWork.YardRepository.GetById((int)check_job.LocationId);
                        if (newYard != null)
                        {
                            //newYard.Status = "Full";
                            newYard.Status = "Not Available";
                            newYard.UserStamp = user;
                            newYard.ModDate = DateTime.UtcNow;
                            await Task.Run(() => unitOfWork.YardRepository.Update(newYard));
                        }
                        // อัพเดทสถานะ trailer
                        trailer.LocationId = check_job.LocationId;
                        trailer.Status = "On-Dock";
                        trailer.UserStamp = user;
                        trailer.ModDate = DateTime.UtcNow;
                        await Task.Run(() => unitOfWork.TrailerRepository.Update(trailer));
                        unitOfWork.Save();
                    }
                    check_job.Status = "On-Dock";
                    check_job.UserStamp = user;
                    check_job.ModDate = DateTime.UtcNow;
                    await Task.Run(() => unitOfWork.JobRepository.UpdateJob(check_job));
                    unitOfWork.Save();
                    return true;
                }
                check_job.Status = "In-Process";
                check_job.UserStamp = user;
                check_job.ModDate = DateTime.UtcNow;
                await Task.Run(() => unitOfWork.JobRepository.UpdateJob(check_job));
                unitOfWork.Save();
                return true;
            }
            catch (Exception ex)
            {
                // Log error หรือจัดการ error ตามที่ต้องการ
                throw new Exception($"เกิดข้อผิดพลาดในการเริ่มงาน: {ex.Message}", ex);
            }
        }

        public async Task<bool> ConfirmCompleteJob(Job job, string user)
        {
            try
            {
                Job check_job = unitOfWork.JobRepository.GetJobById(job.Id);
                if (job == null)
                {
                    throw new Exception("Job not found.");
                }
                if (check_job.Status != "In-Process")
                {
                    throw new Exception("Job cannot assigned.");
                }

                // จัดการ Shunt
                Shunt get_shunt = unitOfWork.ShuntRepository.GetById((int)(check_job.ShuntId ?? 0));
                if (get_shunt != null)
                {
                    get_shunt.Status = "Available";
                    get_shunt.UserStamp = user;
                    get_shunt.ModDate = DateTime.UtcNow;
                    await Task.Run(() => unitOfWork.ShuntRepository.Update(get_shunt));
                }

                // จัดการ Trailer และ Yard
                Trailer check_trailer = unitOfWork.TrailerRepository.GetById((int)(check_job.TrailerId ?? 0));
                if (check_trailer != null)
                {
                    // อัพเดทสถานะ yard เก่า
                    Yard oldYard = unitOfWork.YardRepository.GetById((int)check_trailer.LocationId);
                    if (oldYard != null)
                    {
                        //oldYard.Status = "Empty";
                        oldYard.Status = "Available";
                        oldYard.UserStamp = user;
                        oldYard.ModDate = DateTime.UtcNow;
                        await Task.Run(() => unitOfWork.YardRepository.Update(oldYard));
                    }

                    // อัพเดท trailer
                    check_trailer.LocationId = check_job.LocationId;
                    if (check_trailer.Status == "On-Dock")
                    {
                        check_trailer.Status = "Empty";
                    }

                    // อัพเดท trailer
                    check_trailer.LocationId = check_job.LocationId;
                    check_trailer.UserStamp = user;
                    check_trailer.ModDate = DateTime.UtcNow;
                    await Task.Run(() => unitOfWork.TrailerRepository.Update(check_trailer));

                    // อัพเดทสถานะ yard ใหม่
                    Yard newYard = unitOfWork.YardRepository.GetById((int)check_job.LocationId);
                    if (newYard != null)
                    {
                        //newYard.Status = "Full";
                        newYard.Status = "Not Available";
                        newYard.UserStamp = user;
                        newYard.ModDate = DateTime.UtcNow;
                        await Task.Run(() => unitOfWork.YardRepository.Update(newYard));
                    }
                }

                // อัพเดทสถานะ Job
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

        public async Task<bool> CancelJob(Job job, string user)
        {
            var check_job = unitOfWork.JobRepository.GetJobById(job.Id);
            if (job == null)
            {
                throw new Exception("Job not found.");
            }
            if (check_job.Status != "New")
            {
                throw new Exception("Job cannot be deleted.");
            }
            else
            {
                Shunt check_shunt = unitOfWork.ShuntRepository.GetById((int)(check_job.ShuntId ?? 0));
                if (check_shunt != null)
                {
                    check_shunt.Status = "Available";
                    check_shunt.UserStamp = user;
                    check_shunt.ModDate = DateTime.UtcNow;
                    await Task.Run(() => unitOfWork.ShuntRepository.Update(check_shunt));
                    unitOfWork.Save();
                }

                check_job.Status = "Cancelled";
                check_job.UserStamp = user;
                check_job.ModDate = DateTime.UtcNow;
                unitOfWork.JobRepository.UpdateJob(check_job);
                unitOfWork.Save();
            }
            return true;
        }

        public async Task<IActionResult> UpdateInDc([FromBody] ChangeLocationRequest request, string user)
        {
            Trailer check_trailer = unitOfWork.TrailerRepository.GetById(request.TrailerId);
            if (check_trailer == null)
            {
                throw new Exception("Trailer not found.");
            }

            Yard check_yard = unitOfWork.YardRepository.GetById(request.LocationId);
            if (check_trailer == null)
            {
                throw new Exception("Trailer not found.");
            }
            if (check_yard.Status != "Available")
            {
                throw new Exception("Location not available.");
            }

            check_yard.Status = "Not Available";
            check_yard.UserStamp = user;
            check_yard.ModDate = DateTime.UtcNow;
            await Task.Run(() => unitOfWork.YardRepository.Update(check_yard));

            check_trailer.LocationId = request.LocationId;
            check_trailer.Status = "Empty";
            check_trailer.UserStamp = user;
            check_trailer.ModDate = DateTime.UtcNow;
            await Task.Run(() => unitOfWork.TrailerRepository.Update(check_trailer));
            unitOfWork.Save();

            return new OkResult();
        }

        public async Task<bool> UpdateOutDc(OutDCRequestDto trailer, string user)
        {
            Trailer check_trailer = unitOfWork.TrailerRepository.GetById(trailer.TrailerId);
            if (check_trailer == null)
            {
                throw new Exception("Trailer not found.");
            }
            if (check_trailer.Status == "Out-DC")
            {
                throw new Exception("Trailer already Out-DC.");
            }

            Yard check_yard = unitOfWork.YardRepository.GetById(check_trailer.LocationId ?? 0);
            if (check_yard == null)
            {
                throw new Exception("Location not found.");
            }

            check_yard.Status = "Available";
            check_yard.UserStamp = user;
            check_yard.ModDate = DateTime.UtcNow;
            await Task.Run(() => unitOfWork.YardRepository.Update(check_yard));

            check_trailer.LocationId = null;
            check_trailer.Status = "Out-DC";
            check_trailer.OutDcLicensePlate = trailer.OutDcLicensePlate;
            check_trailer.OutDcDriver = trailer.OutDcDriver;
            check_trailer.UserStamp = user;
            check_trailer.ModDate = DateTime.UtcNow;
            await Task.Run(() => unitOfWork.TrailerRepository.Update(check_trailer));
            unitOfWork.Save();

            return true;
        }

        private string DetermineJobType(string fromTrailerYardType, string toYardType)
        {
            // Define the mapping of yard type transitions to job types
            if (fromTrailerYardType == "Yard" && toYardType == "Yard")
                return "เปลี่ยนลานจอด";

            if (fromTrailerYardType == "Yard" && toYardType == "Dock")
                return "ย้ายตู้เปล่า";

            if (fromTrailerYardType == "Dock" && toYardType == "Yard")
                return "ย้ายตู้เต็ม";

            if (fromTrailerYardType == "Yard" && toYardType == "Maintenance")
                return "ติดต่อช่าง";

            if (fromTrailerYardType == "Yard" && toYardType == "Washing")
                return "นำไปล้าง";

            if (fromTrailerYardType == "Dock" && toYardType == "Dock")
                return "เปลี่ยนประตู";

            if (fromTrailerYardType == "Maintenance" && toYardType == "Yard")
                return "เปลี่ยนลานจอด";

            if (fromTrailerYardType == "Washing" && toYardType == "Yard")
                return "เปลี่ยนลานจอด";

            if (fromTrailerYardType == "Washing" && toYardType == "Dock")
                return "ย้ายตู้เปล่า";

            // If no valid transition is found, throw an exception
            throw new Exception($"Invalid yard type transition from {fromTrailerYardType} to {toYardType}");
        }
    }
}
