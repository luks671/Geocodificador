using System;
using System.Collections.Generic;
using System.Text;

namespace Geocodificador.Interface
{
    interface IGeocodificar
    {
        void Codificar(Dictionary<string,string> direccion);
    }
}
