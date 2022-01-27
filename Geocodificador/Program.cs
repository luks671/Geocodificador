using Geocodificador.Consumidor;
using Geocodificador.Interface;
using System;

namespace Geocodificador
{
    class Program
    {
        static void Main(string[] args)
        {
            IReceptor geocodificar = new GeocodificarYReenviar();

            geocodificar.Receiver();
        }
    }
}
