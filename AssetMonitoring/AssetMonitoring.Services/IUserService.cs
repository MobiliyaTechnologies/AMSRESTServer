namespace AssetMonitoring.Services
{
    using System.Threading.Tasks;
    using AssetMonitoring.Contracts;

    public interface IUserService
    {
        /// <summary>
        /// Creates the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The user ID.</returns>
        Task<int> Create(User user);

        /// <summary>
        /// Gets the by B2C identifier.
        /// </summary>
        /// <param name="b2cIdentifier">The B2C identifier.</param>
        /// <returns>The user detail</returns>
        User GetByB2cIdentifier(string b2cIdentifier);
    }
}
