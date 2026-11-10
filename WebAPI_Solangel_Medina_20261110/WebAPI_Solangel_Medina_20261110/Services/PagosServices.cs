using Dapper;
using System.Data;
using WebAPI_Solangel_Medina_20261110.Data;
using WebAPI_Solangel_Medina_20261110.Models;

namespace WebAPI_Solangel_Medina_20261110.Services
{
    public class PagosServices
    {
        private readonly DataContext _context;

        public PagosServices(DataContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Pagos>> Obtener()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Pagos>(
                "SELECT * FROM Pagos ORDER BY ID DESC");
        }


        public async Task<IEnumerable<Pagos>> ObtenerPorFactura(int idFactura)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Pagos>(
                "SELECT * FROM Pagos WHERE IdFactura = @IdFactura",
                new { IdFactura = idFactura });
        }


        public async Task<int> Crear(Pagos pago)
        {
            if (pago.MontoPagado <= 0)
                throw new ArgumentException(
                    "El monto pagado debe ser mayor a cero");

            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction(
                IsolationLevel.ReadCommitted);

            try
            {

                decimal? montoFactura =
                    await connection.QueryFirstOrDefaultAsync<decimal?>(@"
                        SELECT Monto
                        FROM Facturas WITH (UPDLOCK, HOLDLOCK)
                        WHERE ID = @IdFactura",
                        new { pago.IdFactura },
                        transaction);

                if (montoFactura == null)
                    throw new ArgumentException(
                        "La factura no existe");


                decimal totalPagado =
                    await connection.ExecuteScalarAsync<decimal>(@"
                        SELECT ISNULL(SUM(MontoPagado), 0)
                        FROM Pagos
                        WHERE IdFactura = @IdFactura",
                        new { pago.IdFactura },
                        transaction);

                decimal saldoPendiente =
                    montoFactura.Value - totalPagado;

                if (pago.MontoPagado > saldoPendiente)
                    throw new ArgumentException(
                        "El pago supera el saldo pendiente");


                string sql = @"
                    INSERT INTO Pagos
                    (IdFactura, MontoPagado, FechaPago)
                    OUTPUT INSERTED.ID
                    VALUES
                    (@IdFactura, @MontoPagado, @FechaPago)";

                int idPago =
                    await connection.ExecuteScalarAsync<int>(
                        sql, pago, transaction);


                await connection.ExecuteAsync(@"
                    UPDATE Facturas
                    SET EstadoPago =
                        CASE
                            WHEN @TotalPagado >= Monto
                                THEN 1
                            ELSE 0
                        END
                    WHERE ID = @IdFactura",
                    new
                    {
                        pago.IdFactura,
                        TotalPagado =
                            totalPagado + pago.MontoPagado
                    },
                    transaction);

                transaction.Commit();

                return idPago;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

        }
    }
}
