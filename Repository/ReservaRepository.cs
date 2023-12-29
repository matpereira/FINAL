using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using TrabajoFinalRestaurante.Controllers;
using TrabajoFinalRestaurante.Domain.DTO;
using TrabajoFinalRestaurante.Domain.Entities;

namespace TrabajoFinalRestaurante.Repository
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly ReservaRestaurantContext _restaurantContext;

        public ReservaRepository(ReservaRestaurantContext context)
        {
            _restaurantContext = context;
        }
        public async Task<bool> AddReservaRepositoryAsync(ReservaDTO Reserva)
        {
                var NuevaReserva = new Reserva();
                NuevaReserva.CodReserva = Guid.NewGuid().ToString();
                NuevaReserva.NombrePersona = Reserva.NombrePersona.ToUpper();
                NuevaReserva.ApellidoPersona = Reserva.ApellidoPersona.ToUpper(); ;
                NuevaReserva.Dni = Reserva.Dni;
                NuevaReserva.Mail = Reserva.Mail;
                NuevaReserva.Celular = Reserva.Celular; 
                NuevaReserva.FechaReserva = Reserva.FechaReserva;
                NuevaReserva.IdRangoReserva = Reserva.IdRangoReserva;
                NuevaReserva.CantidadPersonas = Reserva.CantidadPersonas;
                NuevaReserva.Estado = "CONFIRMADO";
                NuevaReserva.FechaAlta = DateTime.Now;
                NuevaReserva.FechaModificacion = null;
               await _restaurantContext.Reservas.AddAsync(NuevaReserva);
               await _restaurantContext.SaveChangesAsync();
                return true;
        }


        public Task<bool> UpdateReservaRepositoryAsync(ModificacionReservaDTO Reserva)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> UpdateReservaRepositoryAsync(string dni, ModificacionReservaDTO ReservaModif)  
        {
            var reserva = await _restaurantContext.Reservas.Where(w => w.Dni == dni && w.Estado == "CONFIRMADO").FirstOrDefaultAsync()!;
            reserva.FechaReserva = ReservaModif.FechaReserva;
            reserva.IdRangoReserva = ReservaModif.IdRangoReserva;
            reserva.CantidadPersonas = ReservaModif.CantidadPersonas;
            reserva.FechaModificacion = DateTime.Now;
            await _restaurantContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelarReservaRepositoryAsync(string Dni) 
        {
            var Reserva = await _restaurantContext.Reservas.Where(w => w.Dni == Dni && w.Estado == "CONFIRMADO").FirstOrDefaultAsync()!;
            if (Reserva != null)
            {
                Reserva.Estado = "CANCELADO";
                Reserva.FechaModificacion = DateTime.Now;
                await _restaurantContext.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }


        public async Task<List<Reserva>> TurnosCanceladosRepository() 
        {
            return await _restaurantContext.Reservas.Where(w => w.Estado == "CANCELADO").ToListAsync();

        }

        public async Task<List<Reserva>> TurnosConfirmadosRepository() 
        {
            return await _restaurantContext.Reservas.Where(w => w.Estado == "CONFIRMADO").ToListAsync();
        }

        public async Task<List<TurnoLibreResult>> TurnosPorFechaRepository(DateTime FechaReserva)
        {
            var result = await _restaurantContext.RangoReservas
                .Select(r => new
                {
                    RangoReserva = r,
                    CupoOcupado = _restaurantContext.Reservas
                        .Where(res => res.FechaReserva.Date == FechaReserva.Date && res.IdRangoReserva == r.IdRangoReserva && res.Estado == "CONFIRMADO")
                        .Sum(res => res.CantidadPersonas)
                })
                .ToListAsync();

            var turnosLibres = result.Select(r => new TurnoLibreResult
            {
                IdRangoReserva = r.RangoReserva.IdRangoReserva,
                DescripcionRango = r.RangoReserva.Descripcion,
                CupoTotal = r.RangoReserva.Cupo,
                CupoOcupado = r.CupoOcupado,
                LugaresLibres = r.RangoReserva.Cupo - r.CupoOcupado
            }).ToList();

            return turnosLibres;

        }

        public async Task<List<TurnoLibreResult>> TurnosSinCuposRepository()
        {
            DateTime fechaActual = DateTime.Now;
            DateTime fechaLimite = fechaActual.AddDays(7);

            var turnosSinLugaresLibres = await _restaurantContext.RangoReservas
                .Where(r => _restaurantContext.Reservas
                    .Where(res => res.FechaReserva.Date >= fechaActual.Date && res.FechaReserva.Date <= fechaLimite.Date && res.Estado == "CONFIRMADO" && r.IdRangoReserva == res.IdRangoReserva)
                    .GroupBy(res => res.FechaReserva)
                    .Any(g => r.Cupo - g.Sum(res => res.CantidadPersonas) == 0))
                .Select(r => new TurnoLibreResult
                {
                    IdRangoReserva = r.IdRangoReserva,
                    DescripcionRango = r.Descripcion,
                    FechaReserva = _restaurantContext.Reservas
                        .Where(res => res.FechaReserva.Date >= fechaActual.Date && res.FechaReserva.Date <= fechaLimite.Date && res.Estado == "CONFIRMADO" && res.IdRangoReserva == r.IdRangoReserva)
                        .Select(res => res.FechaReserva)
                        .FirstOrDefault(),
                    CupoTotal = r.Cupo,
                    CupoOcupado = _restaurantContext.Reservas
                        .Where(res => res.FechaReserva.Date >= fechaActual.Date && res.FechaReserva.Date <= fechaLimite.Date && res.Estado == "CONFIRMADO" && res.IdRangoReserva == r.IdRangoReserva)
                        .Sum(res => res.CantidadPersonas),
                    LugaresLibres = 0 // Aquí se establece como 0 ya que estos turnos están completamente ocupados
                })
                .ToListAsync();

            return turnosSinLugaresLibres;
        }
        rn turnosSinLugaresLibres;
        }






    }
}
