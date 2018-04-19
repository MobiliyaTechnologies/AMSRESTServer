namespace AssetMonitoring.StreamAnalytics.Services
{
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts.AnalyticsContract;

    /// <summary>
    /// Provides stream analytics related operations.
    /// </summary>
    public interface IStreamAnalyticsService
    {
        /// <summary>
        /// Adds the transformation.
        /// Adds query to stream analytics job.
        /// </summary>
        /// <param name="jobTransformation">The job transformation.</param>
        Task AddTransformation(JobTransformation jobTransformation);

        /// <summary>
        /// Creates the stream analytics job for group alert.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <returns>The alert job query if it's exist and running else null.</returns>
        Task<string> CreateAlertJob(string jobName);

        /// <summary>
        /// Deletes the stream analytics job for given name.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        Task DeleteJob(string jobName);

        /// <summary>
        /// Initializes the stream analytics.
        /// Creates stream analytics job to populate sensor data in documentDB.
        /// </summary>
        Task InitializeStreamAnalytics();
    }
}
