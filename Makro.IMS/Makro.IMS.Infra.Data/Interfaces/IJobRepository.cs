using Makro.IMS.Infra.Data.Models;

namespace Makro.IMS.Infra.Data.Interfaces
{

    public interface IJobRepository : IDisposable
    {
        IQueryable<Job> GetJobPaged();
        IEnumerable<Job> GetAllJobs();
        Job GetJobById(int jobId);
        void AddJob(Job job);
        void UpdateJob(Job job);
        void DeleteJob(Job Job);
    }

}
