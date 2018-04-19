namespace AssetMonitoring.Components.Context
{
    public interface IContextInfoProvider
    {
        UserContext Current { get; }
    }
}
