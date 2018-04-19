namespace AssetMonitoring.Services.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Components.Repository;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;
    using AssetMonitoring.Services.Impl.Mappings;
    using AssetMonitoring.Utilities;

    public class IndoorLayoutService : IIndoorLayoutService
    {
        private readonly IRepository repository;
        private readonly IAlertService alertyService;

        public IndoorLayoutService(IRepository repository, IAlertService alertyService)
        {
            this.repository = repository;
            this.alertyService = alertyService;
        }

        void IIndoorLayoutService.Add(IndoorLayout indoorLayout)
        {
            var layout = this.repository.Query<Entities.IndoorLayout>().FirstOrDefault();

            if (layout != null)
            {
                layout.Gateways.Clear();
            }
            else
            {
                layout = new Entities.IndoorLayout
                {
                    Name = indoorLayout.Name,
                    Description = indoorLayout.Description,
                    FileName = indoorLayout.FileName
                };
            }

            this.repository.Persist(layout);
        }

        OperationStatus IIndoorLayoutService.Delete(int indoorLayoutId)
        {
            var layout = this.repository.Read<Entities.IndoorLayout>(indoorLayoutId);

            if (layout == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Indoor layout does not exists for id - {0}", indoorLayoutId));
            }

            this.repository.Delete(layout);

            return new OperationStatus();
        }

        OperationStatus IIndoorLayoutService.DetachGateway(IndoorLayout indoorLayout)
        {
            var layout = this.repository.Read<Entities.IndoorLayout>(indoorLayout.Id);

            if (layout == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Indoor layout does not exists for id - {0}", indoorLayout.Id));
            }

            foreach (var removedGateway in indoorLayout.Gateways)
            {
                var gateway = this.repository.Read<Entities.Gateway>(removedGateway.Id);

                if (gateway == null)
                {
                    return new OperationStatus(StatusCode.Error, string.Format("Gateway does not exists for id - {0}", removedGateway.Id));
                }

                gateway.IndoorLayout = null;
                gateway.LayoutX = null;
                gateway.LayoutX = null;

                this.repository.Persist(gateway);
            }

            return new OperationStatus();
        }

         IndoorLayout IIndoorLayoutService.Get(int indoorLayoutId)
        {
            var layout = this.repository.Read<Entities.IndoorLayout>(indoorLayoutId);
            var indoorLayout = new IndoorLayoutMapping().Map(layout);

            if (indoorLayout != null)
            {
               this.SetAssets(new List<IndoorLayout> { indoorLayout });
            }

            return indoorLayout;
        }

        List<IndoorLayout> IIndoorLayoutService.GetAll()
        {
            var layouts = this.repository.Query<Entities.IndoorLayout>();

            var indoorLayouts = new IndoorLayoutMapping().Map(layouts).ToList();

           this.SetAssets(indoorLayouts);

            return indoorLayouts;
        }

        OperationStatus IIndoorLayoutService.MapGateway(IndoorLayout indoorLayout)
        {
            var layout = this.repository.Read<Entities.IndoorLayout>(indoorLayout.Id);

            if (layout == null)
            {
                return new OperationStatus(StatusCode.Error, string.Format("Indoor layout does not exists for id - {0}", indoorLayout.Id));
            }

            foreach (var newGateway in indoorLayout.Gateways)
            {
                var gateway = this.repository.Read<Entities.Gateway>(newGateway.Id);

                if (gateway == null)
                {
                    return new OperationStatus(StatusCode.Error, string.Format("Gateway does not exists for id - {0}", newGateway.Id));
                }

                if (!newGateway.LayoutX.HasValue || !newGateway.LayoutY.HasValue)
                {
                    return new OperationStatus(StatusCode.Error, "Invalid gateway position on layout");
                }

                gateway.IndoorLayout = layout;
                gateway.LayoutX = newGateway.LayoutX;
                gateway.LayoutY = newGateway.LayoutY;

                this.repository.Persist(gateway);
            }

            return new OperationStatus();
        }

        private void SetAssets(List<IndoorLayout> indoorLayouts)
        {
            foreach (var indoorLayout in indoorLayouts)
            {
                foreach (var gateway in indoorLayout.Gateways)
                {
                    var gatewayGroupIds = this.repository.Query<Entities.SensorRule>().Where(s => s.CapabilityFilter.Capability.Name.Equals(ApplicationConstant.GatewayCapability) && s.MinThreshold.Equals(gateway.GatewayKey)).Select(s => s.SensorGroupId);

                    var gatewayAssets = this.repository.Query<Entities.Asset>().Where(a => gatewayGroupIds.Any(id => id == a.SensorGroupId));

                    //var damageAssetBarcodes = new List<string>();

                    //foreach (var gatewayGroupId in gatewayGroupIds)
                    //{
                    //    damageAssetBarcodes.AddRange(this.alertyService.GetDamageAssetBarcodes(gatewayGroupId));
                    //}

                    foreach (var gatewayAsset in gatewayAssets)
                    {
                        var asset = new GatewaytAsset { AssetBarcode = gatewayAsset.AssetBarcode };

                        //if (damageAssetBarcodes.Any(d => d.Equals(gatewayAsset.AssetBarcode)))
                        //{
                        //    asset.IsDamagedAsset = true;
                        //}

                        asset.SensorCapabilities = gatewayAsset.Sensors.SelectMany(s => s.SensorType.Capabilities).Distinct().Select(c => new SensorCapability { Id = c.Id, Name = c.Name }).ToList();

                        gateway.GatewaytAsset.Add(asset);
                    }
                }
            }
        }
    }
}
