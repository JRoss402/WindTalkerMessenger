
namespace WindTalkerMessenger.Models.DataLayer.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
