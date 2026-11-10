using Dapper;
using WebAPI_Solangel_Medina_20261110.Data;
using WebAPI_Solangel_Medina_20261110.Models;

namespace WebAPI_Solangel_Medina_20261110.Services
{
    public class FacturaServices
    {
        private readonly DataContext _context;

        public FacturaServices(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Facturas>> Obtener()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Facturas>(
                "SELECT * FROM Facturas"
            );
        }

        public async Task<int> Crear(Facturas factura)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                INSERT INTO Facturas
                (IdCliente, FechaEmision, FechaVencer,
                 EstadoPago, Monto)
                VALUES
                (@IdCliente, @FechaEmision, @FechaVencer,
                 @EstadoPago, @Monto)";

            return await connection.ExecuteAsync(sql, factura);
        }

        public async Task<bool> UpdateFacturas(int id, Facturas facturas)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                UPDATE Facturas
                SET FechaEmision= @FechaEmision, 
                FechaVencer= @FechaVencer, 
                EstadoPago= @EstadoPago, 
                Monto= @Monto
                 WHERE ID = @Id";

            int rows = await connection.ExecuteAsync(
                sql,
                new
                {
                    Id = id,
                    facturas.FechaEmision,
                    facturas.FechaVencer,
                    facturas.EstadoPago,
                    facturas.Monto
                });

            return rows > 0;
        }


        public async Task<int> DeleteFacturas(int id)
        {
            using var connection = _context.CreateConnection();

            return await connection.ExecuteAsync(
                "DELETE FROM Facturas WHERE ID = @Id",
                new { Id = id });
        }
    }
}
