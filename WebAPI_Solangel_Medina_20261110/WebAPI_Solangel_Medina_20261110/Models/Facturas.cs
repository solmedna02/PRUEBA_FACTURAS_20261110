namespace WebAPI_Solangel_Medina_20261110.Models
{
    public class Facturas
    {
        public int ID { get; set; }
        public int IdCliente { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencer { get; set; }
        public bool EstadoPago { get; set; }
        public decimal Monto { get; set; }
    }
}
