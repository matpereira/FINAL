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

            var turnosLibresPorFecha = await _restaurantContext.RangoReservas
                .SelectMany(rango => _restaurantContext.Reservas
                    .Where(reserva =>
                        reserva.FechaReserva.Date >= fechaActual.Date &&
                        reserva.FechaReserva.Date <= fechaLimite.Date &&
                        reserva.Estado == "CONFIRMADO" &&
                        reserva.IdRangoReserva == rango.IdRangoReserva)
                    .GroupBy(reserva => reserva.FechaReserva.Date)
                    .Where(grp => rango.Cupo - grp.Sum(res => res.CantidadPersonas) == 0)
                    .Select(grp => new TurnoLibreResult
                    {
                        FechaReserva = grp.Key,
                        IdRangoReserva = rango.IdRangoReserva,
                        DescripcionRango = rango.Descripcion,
                        CupoTotal = rango.Cupo,
                        CupoOcupado = grp.Sum(res => res.CantidadPersonas),
                        LugaresLibres = 0 
                    }))
                .ToListAsync();

            return turnosLibresPorFecha;
        }








    }
}
