using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetMonitoring.Components.Context;
using AssetMonitoring.Components.Repository;
using AssetMonitoring.Contracts;
using AssetMonitoring.Contracts.Enums;
using AssetMonitoring.Services.Impl.Mappings;

namespace AssetMonitoring.Services.Impl
{
    public class ApplicationConfigurationService : IApplicationConfigurationService
    {
        private readonly IRepository repository;
        private readonly IContextInfoProvider context;

        public ApplicationConfigurationService(IRepository repository, IContextInfoProvider context)
        {
            this.repository = repository;
            this.context = context;
        }

        OperationStatus IApplicationConfigurationService.AddApplicationConfiguration(Configuration applicationConfigurationModel)
        {
            var applicationConfiguration = this.repository.Query<Entities.ApplicationConfiguration>().FirstOrDefault(a => a.ConfigurationType.Equals(applicationConfigurationModel.ApplicationConfigurationType.Trim(), StringComparison.InvariantCultureIgnoreCase));

            if (applicationConfiguration == null)
            {
                return new OperationStatus(StatusCode.Error, "Invalid application configuration type.");
            }

            foreach (var applicationConfigurationEntry in applicationConfigurationModel.ApplicationConfigurationEntries)
            {
                var updateConfig = applicationConfiguration.ApplicationConfigurationEntries.FirstOrDefault(e => e.ConfigurationKey.Equals(applicationConfigurationEntry.ConfigurationKey, StringComparison.InvariantCultureIgnoreCase));

                if (updateConfig != null)
                {
                    updateConfig.ConfigurationValue = applicationConfigurationEntry.ConfigurationValue;
                }
                else
                {
                    updateConfig = new Entities.ApplicationConfigurationEntry()
                    {
                        ConfigurationKey = applicationConfigurationEntry.ConfigurationKey,
                        ConfigurationValue = applicationConfigurationEntry.ConfigurationValue,
                        ApplicationConfigurationId = applicationConfiguration.Id,
                    };
                }

                this.repository.Persist(updateConfig);
            }

            return new OperationStatus();
        }

        OperationStatus IApplicationConfigurationService.DeleteApplicationConfiguration(string applicationConfigurationType)
        {
            var applicationConfiguration = this.repository.Query<Entities.ApplicationConfiguration>().FirstOrDefault(a => a.ConfigurationType.Equals(applicationConfigurationType.Trim(), StringComparison.InvariantCultureIgnoreCase));

            if (applicationConfiguration == null)
            {
                return new OperationStatus(StatusCode.Error, "Invalid application configuration type.");
            }

            foreach (var applicationConfigurationEntry in applicationConfiguration.ApplicationConfigurationEntries)
            {
                this.repository.Delete(applicationConfigurationEntry);
            }

            return new OperationStatus();
        }

        OperationStatus IApplicationConfigurationService.DeleteApplicationConfigurationEntry(int applicationConfigurationEntryId)
        {
            var applicationConfigurationEntry = this.repository.Read<Entities.ApplicationConfigurationEntry>(applicationConfigurationEntryId);

            if (applicationConfigurationEntry == null)
            {
                return new OperationStatus(StatusCode.Error, "Application configuration entry does not exists.");
            }

            this.repository.Delete(applicationConfigurationEntry);

            return new OperationStatus();
        }

        List<Configuration> IApplicationConfigurationService.GetAllApplicationConfiguration()
        {
            var applicationConfigurations = this.repository.Query<Entities.ApplicationConfiguration>();
            return new ApplicationConfigurationMapping().Map(applicationConfigurations).ToList();
        }

        Configuration IApplicationConfigurationService.GetApplicationConfiguration(string applicationConfigurationType)
        {
            var applicationConfigurations = this.repository.Query<Entities.ApplicationConfiguration>().FirstOrDefault(a => a.ConfigurationType.Equals(applicationConfigurationType.Trim(), StringComparison.InvariantCultureIgnoreCase)); 

            return new ApplicationConfigurationMapping().Map(applicationConfigurations);
        }

        OperationStatus IApplicationConfigurationService.UpdateApplicationConfigurationEntry(ApplicationConfigurationEntry applicationConfigurationEntryModel)
        {
            var applicationConfigurationEntry = this.repository.Read<Entities.ApplicationConfigurationEntry>(applicationConfigurationEntryModel.Id);

            if (applicationConfigurationEntry == null)
            {
                return new OperationStatus(StatusCode.Error, "Application configuration entry does not exists.");
            }

            applicationConfigurationEntry.ConfigurationKey = applicationConfigurationEntryModel.ConfigurationKey;
            applicationConfigurationEntry.ConfigurationValue = applicationConfigurationEntryModel.ConfigurationValue;

            this.repository.Persist(applicationConfigurationEntry);

            return new OperationStatus();
        }
    }
}
