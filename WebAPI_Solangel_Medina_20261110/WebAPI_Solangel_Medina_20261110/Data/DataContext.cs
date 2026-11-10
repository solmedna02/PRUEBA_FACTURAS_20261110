using Microsoft.Data.SqlClient;
using System.Data;

namespace WebAPI_Solangel_Medina_20261110.Data
{
    public class DataContext
    {
        private readonly IConfiguration _configuration;

        public DataContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            );
        }
    }
}
