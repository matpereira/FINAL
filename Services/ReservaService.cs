using System.Net;
using System.Threading.Tasks;
using TrabajoFinalRestaurante.Controllers;
using TrabajoFinalRestaurante.Domain.DTO;
using TrabajoFinalRestaurante.Domain.Entities;
using TrabajoFinalRestaurante.Repository;
using TrabajoFinalRestaurante.Services.Interfaces;

namespace TrabajoFinalRestaurante.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly ValidacionService _validacionService;

        public ReservaService(IReservaRepository reservaRepository, ValidacionService validacionService)
        {
            _reservaRepository = reservaRepository;
            _validacionService = validacionService;
        }

        public async Task<AddReservaResponse> AddReservaAsync(ReservaDTO reserva)
        {
            var response = _validacionService.ValidaCamposReserva(reserva); 

            if (!response.Success) 
            {
                return response;
            }

            if (!_validacionService.CupoValido(reserva.IdRangoReserva, reserva.CantidadPersonas, reserva.FechaReserva))
            {
                response.Success = false;
                response.Message = "El turno seleccionado no posee cupos suficientes.";
                return response;
            }

            if (!_validacionService.ValidarFecha(reserva.FechaReserva))
            {
                response.Success = false;
                response.Message = "La reserva debe ser con, máximo, 7 días de anticipación.";
                return response;
            }

            if (!_validacionService.ReservaUnica(reserva.FechaReserva, reserva.Dni))
            {
                response.Success = false;
                response.Message = "El DNI ingresado ya posee una reserva ese día, para continuar, ingrese una fecha nueva o cancele la reserva actual.";
                return response;
            }

            await _reservaRepository.AddReservaRepositoryAsync(reserva);
            response.Success = true;
            response.Message = "Reserva realizada correctamente.";
            return response;
        }


        public async Task<AddReservaResponse> UpdateReservaAsync(string dni, ModificacionReservaDTO ReservaModif)
        {
            var response = _validacionService.ValidaCamposReserva(ReservaModif);

            if (!response.Success)
            {
                return response;
            }

            if (_validacionService.ValidaLongDNIOK(dni) == false)
            {
                response.Success = false;
                response.Message = "El DNI ingresado es incorrecto.";
                return response;
            }

            bool reservaExiste = await _validacionService.ReservaExisteAsync(dni);

            if (!reservaExiste)
            {
                response.Success = false;
                response.Message = "La reserva no existe.";
                return response;
            }

            if (!_validacionService.CupoValido(ReservaModif.IdRangoReserva, ReservaModif.CantidadPersonas, ReservaModif.FechaReserva))
            {
                response.Success = false;
                response.Message = "El turno seleccionado no posee cupos suficientes.";
                return response;
            }
            if (!_validacionService.ValidarFecha(ReservaModif.FechaReserva))

            {
                response.Success = false;
                response.Message = "La reserva debe ser con, maximo, 7 dias de anticipación.";
                return response;
            }
            if (!_validacionService.ReservaUnica(ReservaModif.FechaReserva, dni))
            {
                response.Success = false;
                response.Message = "El DNI ingresado ya posee una reserva ese dia, para continuar, ingrese una fecha nueva o cancele la reserva actual.";
                return response;
            }

            await _reservaRepository.UpdateReservaRepositoryAsync(dni, ReservaModif);
            response.Success = true;
            response.Message = "Reserva modificada correctamente.";
            return response;
        }



        public async Task<AddReservaResponse> CancelarReservaAsync(string dni)
        {
            var response = new AddReservaResponse();

            if (_validacionService.ValidaLongDNIOK(dni) == false )
            {
                response.Success = false;
                response.Message = "El DNI ingresado es incorrecto.";
                return response;
            }

            bool reservaExiste = await _validacionService.ReservaExisteAsync(dni);

            if (!reservaExiste)
            {
                response.Success = false;
                response.Message = "La reserva no existe.";
                return response;
            }

            await _reservaRepository.CancelarReservaRepositoryAsync(dni);
            response.Success = true;
            response.Message = "Reserva cancelada correctamente.";
            return response;
        }



        public async Task<List<Reserva>> TurnosCanceladosAsync()
         {
             return await _reservaRepository.TurnosCanceladosRepository();
         }
       
           public async Task<List<Reserva>> TurnosConfirmadosAsync()
           {
               return await _reservaRepository.TurnosConfirmadosRepository();
           }
        
         public async Task<List<TurnoLibreResult>> TurnosPorFechaAsync(DateTime FechaReserva)
          {
              return await _reservaRepository.TurnosPorFechaRepository(FechaReserva);
          }

        public async Task<List<TurnoLibreResult>> TurnosSinCuposAsync()
        {
            return await _reservaRepository.TurnosSinCuposRepository();
        }

    }
}
