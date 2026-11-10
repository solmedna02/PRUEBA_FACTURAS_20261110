using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI_Solangel_Medina_20261110.Models;
using WebAPI_Solangel_Medina_20261110.Services;

namespace WebAPI_Solangel_Medina_20261110.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController: ControllerBase
    {
        private readonly FacturaServices _service;

        public FacturasController(FacturaServices service)
        {
            _service = service;
        }

        
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var facturas = await _service.Obtener();
            return Ok(facturas);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] Facturas factura)
        {
            if (factura.Monto <= 0)
            {
                return BadRequest(
                    "El monto debe ser mayor a cero");
            }

            if (factura.FechaVencer < factura.FechaEmision)
            {
                return BadRequest(
                    "La fecha de vencimiento no puede ser anterior a la emisión");
            }

            var resultado = await _service.Crear(factura);

            if (resultado > 0)
            {
                return Ok(new
                {
                    mensaje = "Factura registrada correctamente"
                });
            }

            return BadRequest(
                "No se pudo registrar la factura");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Actualizar(int id, Facturas factura)
        {
            if (factura.Monto <= 0 ||
                factura.FechaVencer < factura.FechaEmision)
                return BadRequest("Monto o fechas inválidas");

            try
            {
                var resultado = await _service.UpdateFacturas(id, factura);

                return resultado
                    ? Ok("Factura actualizada")
                    : NotFound("Factura no encontrada");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
