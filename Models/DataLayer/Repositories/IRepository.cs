using System.Collections.Generic;

namespace WindTalkerMessenger.Models.DataLayer.Repositories
{
    public interface IRepository<T> where T: class
    {
        IEnumerable<T> GetAll();

       
    }
}
