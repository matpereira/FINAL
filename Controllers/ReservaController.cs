using Microsoft.AspNetCore.Mvc;
using TrabajoFinalRestaurante.Domain.DTO;
using TrabajoFinalRestaurante.Services.Interfaces;

namespace TrabajoFinalRestaurante.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class ReservaController : ControllerBase
    {

        private readonly IReservaService _reservaService;

        public ReservaController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        [HttpPost("AddReserva")]
        public async Task<IActionResult> AddReserva([FromBody] ReservaDTO NuevaReserva )
        {
            var result = await _reservaService.AddReservaAsync(NuevaReserva);
            if(!result.Success) return BadRequest(result.Message);

            return Created("",result.Message);
        }

        [HttpPut("UpdateReserva/{dni}")]
        public async Task<IActionResult> UpdateReserva(string dni, [FromBody] ModificacionReservaDTO ReservaModif)
        {
            var result = await _reservaService.UpdateReservaAsync(dni, ReservaModif);
            if (!result.Success) return BadRequest(result.Message);

            return Ok(result.Message);
        }

       
        [HttpPut("CancelarReserva/{dni}")]
        public async Task<IActionResult> CancelarReserva(string dni)
        {
            var result = await _reservaService.CancelarReservaAsync(dni);
            if (!result.Success) return BadRequest(result.Message);

            return Ok(result.Message);
        }

 
       [HttpGet("TurnosCancelados")]
       public async Task<IActionResult> TurnosCancelados()
       {
           var result = await _reservaService.TurnosCanceladosAsync();
           if (result == null) return BadRequest("No hay turnos cancelados");

           return Ok(result);
       }
       
        [HttpGet("TurnosConfirmados")]
        public async Task<IActionResult> TurnosConfirmados()
        {
            var result = await _reservaService.TurnosConfirmadosAsync();
            if (result == null) return BadRequest("No hay turnos confirmados");

            return Ok(result);

        }
        
        [HttpGet("TurnosPorFecha/{FechaReserva}")]
        public async Task<IActionResult> TurnosPorFecha(DateTime FechaReserva)
        {
           var result = await _reservaService.TurnosPorFechaAsync(FechaReserva);
           if (result == null) return BadRequest("No hay turnos para la fecha ingresada");

           return Ok(result);
        }

        [HttpGet("TurnosSinCupos")]
        public async Task<IActionResult> TurnosSinCupos()
        {
            var result = await _reservaService.TurnosSinCuposAsync();
            if (result == null) return BadRequest("No hay turnos sin cupos");

            return Ok(result);
        }

    }


}
