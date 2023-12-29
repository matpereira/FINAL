
using Microsoft.EntityFrameworkCore;
using TrabajoFinalRestaurante.Controllers;
using TrabajoFinalRestaurante.Domain.DTO;
using TrabajoFinalRestaurante.Repository;

namespace TrabajoFinalRestaurante.Services
{
    public class ValidacionService
    {
        private readonly ReservaRestaurantContext _restaurantContext;

        public ValidacionService(ReservaRestaurantContext restaurantContext)
        {
            _restaurantContext = restaurantContext;
        }

        public AddReservaResponse ValidaCamposReserva(ReservaDTO reserva)
        {
            var response = new AddReservaResponse();
            var errores = new List<string>();

            if (reserva.NombrePersona.Length > 50 || string.IsNullOrEmpty(reserva.NombrePersona))
            {
                errores.Add("El nombre debe tener entre 1 y 50 caracteres.");
            }

            if (reserva.ApellidoPersona.Length > 50 || string.IsNullOrEmpty(reserva.ApellidoPersona))
            {
                errores.Add("El apellido debe tener entre 1 y 50 caracteres.");
            }

            if (string.IsNullOrEmpty(reserva.Dni) || reserva.Dni.Length > 8 || !EsNumero(reserva.Dni))
            {
                errores.Add("El DNI debe tener entre 1 y 8 caracteres y contener solo números.");
            }

            if (reserva.Mail.Length > 50 || string.IsNullOrEmpty(reserva.Mail) || !reserva.Mail.Contains('@'))
            {
                errores.Add("El mail debe tener entre 1 y 50 caracteres y contener el símbolo '@'.");
            }

            if (reserva.Celular.Length > 20 || string.IsNullOrEmpty(reserva.Celular) || !EsNumero(reserva.Celular))
            {
                errores.Add("El celular debe tener entre 1 y 20 caracteres.");
            }

            if (!CantidadComensales(reserva.CantidadPersonas) )
            {
                errores.Add("La cantidad de comensales debe ser entre 1 y 100.");
            }

            if (!RangoValido(reserva.IdRangoReserva))
            {
                errores.Add("El rango seleccionado no es válido.");
            }

            if (errores.Any())
            {
                response.Success = false;
                response.Message = string.Join(" ", errores);
                return response;
            }

            response.Success = true;
            response.Message = "Todos los campos son válidos.";
            return response;
        }


        public AddReservaResponse ValidaCamposReserva(ModificacionReservaDTO reserva)
        {
            var response = new AddReservaResponse();
            var errores = new List<string>();


            if (!CantidadComensales(reserva.CantidadPersonas))
            {
                errores.Add("La cantidad de comensales debe ser entre 1 y 100.");
            }

            if (!RangoValido(reserva.IdRangoReserva))
            {
                errores.Add("El rango seleccionado no es válido.");
            }

            if (!ValidarFecha(reserva.FechaReserva))
            {
                errores.Add("La reserva debe ser con, máximo, 7 días de anticipación.");
            }

            if (errores.Any())
            {
                response.Success = false;
                response.Message = string.Join(" ", errores);
                return response;
            }

            response.Success = true;
            response.Message = "Todos los campos son válidos.";
            return response;
        }


        public bool RangoValido(int IdRango)
        {
            var result = _restaurantContext.RangoReservas.Where(w => w.IdRangoReserva == IdRango).Count();
            if (result > 0)
            { return true; }
            return false;

        }

        public bool CantidadComensales(int cantidadPersonas)
        {
            return cantidadPersonas >= 1 && cantidadPersonas <= 100;
        }

        public bool ValidarFecha(DateTime fechaReserva)
        {
            return (fechaReserva - DateTime.Now).TotalDays <= 7;
        }

        public bool ReservaUnica(DateTime fechaReserva, string dniReserva)
        {
            var result = _restaurantContext.Reservas
                .Any(w => w.FechaReserva == fechaReserva && w.Estado == "CONFIRMADO" && w.Dni == dniReserva);

            return !result;
        }

        public bool CupoValido(int idRango, int cantidadPersonas, DateTime fechaReserva)
        {
            int disponibles = CuposDisponibles(idRango, fechaReserva);
            return disponibles >= cantidadPersonas;
        }

        public int CuposDisponibles(int idRango, DateTime fecha)
        {
            var ocupados = _restaurantContext.Reservas
                .Where(x => x.FechaReserva == fecha && x.IdRangoReserva == idRango && x.Estado == "CONFIRMADO")
                .Sum(x => x.CantidadPersonas);

            var cupos = _restaurantContext.RangoReservas
                .Where(w => w.IdRangoReserva == idRango)
                .Select(s => s.Cupo)
                .FirstOrDefault();

            return cupos - ocupados;
        }

        public async Task<bool> ReservaExisteAsync(string dni)
        {
            var result = await _restaurantContext.Reservas.Where(w => w.Dni == dni && w.Estado == "confirmado").FirstOrDefaultAsync();
            if (result == null)
            {
                return false;
            }

            return true;
        }

        public bool ValidaLongDNIOK(string dni)
        {
            return (dni.Length <= 8 && dni.Length > 0) && EsNumero(dni); 
        }


        private static bool EsNumero(string valor)
        {
            return valor.All(char.IsDigit);
        }
    }


}