using System.Security.Cryptography;
using TrabajoFinalRestaurante.Controllers;
using TrabajoFinalRestaurante.Domain.DTO;
using TrabajoFinalRestaurante.Domain.Entities;

namespace TrabajoFinalRestaurante.Services.Interfaces
{
    public interface IReservaService
    {

        public Task<AddReservaResponse> AddReservaAsync(ReservaDTO reserva);

        public Task<AddReservaResponse> UpdateReservaAsync(string dni, ModificacionReservaDTO ReservaModif);

        public Task<AddReservaResponse> CancelarReservaAsync(string Dni);

        public Task<List<Reserva>> TurnosCanceladosAsync();

        public Task<List<Reserva>> TurnosConfirmadosAsync();

        public Task<List<TurnoLibreResult>> TurnosPorFechaAsync(DateTime FechaReserva);

       public Task<List<TurnoLibreResult>> TurnosSinCuposAsync(); 

    }
}

