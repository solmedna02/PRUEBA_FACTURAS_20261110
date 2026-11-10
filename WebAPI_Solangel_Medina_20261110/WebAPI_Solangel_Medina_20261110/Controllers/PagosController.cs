using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI_Solangel_Medina_20261110.Models;
using WebAPI_Solangel_Medina_20261110.Services;

namespace WebAPI_Solangel_Medina_20261110.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController:ControllerBase
    {
        private readonly PagosServices _service;

        public PagosController(PagosServices service)
        {
            _service = service;
        }

        
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var pagos = await _service.Obtener();
            return Ok(pagos);
        }

        
        [HttpGet("factura/{idFactura}")]
        public async Task<IActionResult> ObtenerPorFactura(
            int idFactura)
        {
            var pagos = await _service.ObtenerPorFactura(
                idFactura);

            return Ok(pagos);
        }

        
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear(
            [FromBody] Pagos pago)
        {
            if (pago.MontoPagado <= 0)
                return BadRequest(
                    "El monto debe ser mayor a cero");

            try
            {
                int idPago = await _service.Crear(pago);

                return Ok(new
                {
                    mensaje = "Pago registrado correctamente",
                    idPago = idPago
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
