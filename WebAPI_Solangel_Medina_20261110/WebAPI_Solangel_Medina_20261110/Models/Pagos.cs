namespace WebAPI_Solangel_Medina_20261110.Models
{
    public class Pagos
    {
        public int ID { get; set; }
        public int IdFactura { get; set; }
        public decimal MontoPagado { get; set; }
        public DateTime FechaPago { get; set; }
    }
}
