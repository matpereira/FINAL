namespace TrabajoFinalRestaurante.Controllers
{
    public class TurnoLibreResult
    {

        public int IdRangoReserva { get; set; }
        public string DescripcionRango { get; set; }
        public int CupoTotal { get; set; }
        public int CupoOcupado { get; set; }
        public int LugaresLibres { get; set; }
        public DateTime FechaReserva { get; set; }


    }
}
