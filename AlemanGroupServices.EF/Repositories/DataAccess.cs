using AlemanGroupServices.Core.Repositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace AlemanGroupServices.EF.Repositories
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration _configuration;
        public DataAccess(IConfiguration configuration) 
        {
            _configuration = configuration;
        }
        public async Task<List<T>> LoadData<T, U>(string sql, U parameters /*string connectionString*/)
        {
            using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var rows = await connection.QueryAsync<T>(sql, parameters);
                return rows.ToList();
            }
        }

        public Task SaveData<T>(string sql, T parameters/*, string connectionString*/)
        {
            using (IDbConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}
