using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IDataService
    {
        IEnumerable<T> GetData<T>(string code, Dictionary<string, object> parameters);
    }
}
