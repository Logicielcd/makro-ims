using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Interfaces;
using Makro.IMS.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Makro.IMS.Infra.Data.Repository
{
    public class JobRepository : IJobRepository
    {
        private IMSContext context;
        private DbSet<Job> dbSet;

        public JobRepository(IMSContext context)
        {
            this.context = context;
            this.dbSet = this.context.Jobs;
        }

        public IQueryable<Job> GetJobPaged()
        {
            return this.dbSet.AsNoTracking();
        }

        public IEnumerable<Job> GetAllJobs()
        {
            return this.dbSet.AsNoTracking().ToList();
        }

        public Job GetJobById(int id)
        {
            return this.dbSet.FirstOrDefault(x => x.Id == id);
        }

        public Job GetJobId(string jobId)
        {
            return this.dbSet.FirstOrDefault(x => x.JobId == jobId);
        }

        public void AddJob(Job job)
        {
            this.dbSet.Add(job);
        }

        public void UpdateJob(Job job)
        {
            this.dbSet.Update(job);
        }

        public void DeleteJob(Job job)
        {
            this.dbSet.Remove(job);
        }

        public string GetNextJobNumber()
        {
            var today = DateTime.UtcNow;
            var datePrefix = today.ToString("yyyyMMdd");
            var jobPrefix = $"JOB-{datePrefix}";

            // Get the highest number for today
            var lastJob = this.dbSet.AsNoTracking()
                .Where(j => j.JobId != null &&
                           j.JobId.StartsWith(jobPrefix) &&
                           j.CreateDate.HasValue &&
                           j.CreateDate.Value.Date == today.Date)
                .Select(j => j.JobId)
                .OrderByDescending(j => j)
                .FirstOrDefault();

            if (lastJob == null)
            {
                // First job of the day
                return $"{jobPrefix}001";
            }

            // Extract the running number (last 3 digits) and increment
            if (int.TryParse(lastJob.Substring(lastJob.Length - 3), out int lastNumber))
            {
                // Handle overflow - if somehow we reach 999 jobs in a day
                if (lastNumber >= 999)
                {
                    throw new Exception("Maximum number of jobs for today has been reached");
                }
                return $"{jobPrefix}{(lastNumber + 1).ToString("000")}";
            }

            // Fallback in case of parsing error
            return $"{jobPrefix}001";
        }

        public bool IsJobIdExists(string jobId)
        {
            return this.dbSet.Any(x => x.JobId == jobId);
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
