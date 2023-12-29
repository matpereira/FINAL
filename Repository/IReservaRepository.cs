using TrabajoFinalRestaurante.Controllers;
using TrabajoFinalRestaurante.Domain.DTO;
using TrabajoFinalRestaurante.Domain.Entities;

namespace TrabajoFinalRestaurante.Repository
{
    public interface IReservaRepository
    {
        public Task<bool> AddReservaRepositoryAsync(ReservaDTO Reserva);

        public Task<bool> UpdateReservaRepositoryAsync(ModificacionReservaDTO Reserva);

        public Task<bool> CancelarReservaRepositoryAsync(string Dni);

        public Task<bool> UpdateReservaRepositoryAsync(string dni, ModificacionReservaDTO Reserva);

        public Task<List<Reserva>> TurnosCanceladosRepository();

        public Task<List<Reserva>> TurnosConfirmadosRepository();

        public Task<List<TurnoLibreResult>> TurnosPorFechaRepository(DateTime fechaReserva);

        public Task<List<TurnoLibreResult>> TurnosSinCuposRepository();
    }
}



