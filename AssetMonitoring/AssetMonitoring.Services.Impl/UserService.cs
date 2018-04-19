namespace AssetMonitoring.Services.Impl
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services.Impl.Mappings;
    using AssetMonitoring.StreamAnalytics.Services;

    public sealed class UserService : IUserService
    {
        private readonly IRepository repository;
        private readonly IDocumentDbRepository documentDbRepository;
        private readonly IStreamAnalyticsService streamAnalyticsService;

        public UserService(IRepository repository, IDocumentDbRepository documentDbRepository, IStreamAnalyticsService streamAnalyticsService)
        {
            this.repository = repository;
            this.documentDbRepository = documentDbRepository;
            this.streamAnalyticsService = streamAnalyticsService;
        }

        async Task<int> IUserService.Create(User user)
        {
            if (string.IsNullOrWhiteSpace(user.B2cIdentifier))
            {
                throw new ArgumentException("Can not create user, invalid object identifier.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Can not create user, invalid user email.");
            }

            var userEntity = new Entities.User
            {
                Name = string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name,
                Email = user.Email,
                B2cIdentifier = user.B2cIdentifier
            };

            var isAnySuperAdminUser = this.repository.Query<Entities.User>().Any(u => u.Role.Name.Equals(UserRole.SuperAdmin.ToString()));

            if (isAnySuperAdminUser)
            {
                userEntity.Role = this.repository.Query<Entities.Role>().First(r => r.Name.Equals(UserRole.Admin.ToString()));
            }
            else
            {
                userEntity.Role = this.repository.Query<Entities.Role>().First(r => r.Name.Equals(UserRole.SuperAdmin.ToString()));

                // initialize document db and stream analytics.
               await this.documentDbRepository.InitializeDocumentDB();
               await this.streamAnalyticsService.InitializeStreamAnalytics();
            }

            this.repository.Persist(userEntity);

            // required to get user id.
            this.repository.Flush();

            return userEntity.Id;
        }

        User IUserService.GetByB2cIdentifier(string b2cIdentifier)
        {
            if (string.IsNullOrWhiteSpace(b2cIdentifier))
            {
                throw new ArgumentException("Invalid user identifier.");
            }

            var user = this.repository.Query<Entities.User>().FirstOrDefault(u => u.B2cIdentifier.Equals(b2cIdentifier));
            return new UserMapping().Map(user);
        }
    }
}
