using System.ComponentModel.DataAnnotations;

namespace WebPeliculas.DTOs
{
    public class SalaDeCineCercanoFiltroDTO
    {
        [Range(-90,90)]
        public double Latitud { get; set; }
        [Range(-180,180)]
        public double Longitud { get; set; }
        private int distanciaEnKms = 10;
        private int distanciaMaximaEnKms = 50;
        public int DistanciaEnMks
        {
            get { return distanciaEnKms; }
            set
            {
                DistanciaEnMks = (value > distanciaMaximaEnKms ) ? distanciaMaximaEnKms : value;
            }
        }

    }
}
