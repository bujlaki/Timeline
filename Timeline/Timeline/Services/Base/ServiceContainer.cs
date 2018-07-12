
namespace Timeline.Services.Base
{
    public class ServiceContainer
    {
        public INavigationService Navigation;
        public IAuthenticationService Authentication;
        public IDBService Database;

        public void Set(INavigationService _navsvc)
        {
            Navigation = _navsvc;
        }

        public void Set(IAuthenticationService _authsvc)
        {
            Authentication = _authsvc;
        }

        public void Set(IDBService _dbsvc)
        {
            Database = _dbsvc;
        }
    }
}
