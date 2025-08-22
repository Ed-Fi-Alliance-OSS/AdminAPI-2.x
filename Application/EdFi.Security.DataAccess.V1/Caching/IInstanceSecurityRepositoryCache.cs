namespace EdFi.Security.DataAccess.V1.Caching
{
    public interface IInstanceSecurityRepositoryCache
    {
        InstanceSecurityRepositoryCacheObject GetSecurityRepository(string instanceId);
    }
}