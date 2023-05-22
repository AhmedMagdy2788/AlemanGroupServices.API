using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlemanGroupServices.Core.Repositories
{
    public interface IDataAccess
    {
        Task<List<T>> LoadData<T, U>(string sql, U parameters/*, string connectionString*/);
        Task SaveData<T>(string sql, T parameters/*, string connectionString*/);
    }
}
