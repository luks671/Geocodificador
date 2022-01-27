using Geocodificador.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geocodificador.Geocodificar
{
    public class GeocoficarEnCoordenadas : IGeocodificar
    {
        public void Codificar(Dictionary<string,string> direccion)
        {
            #region Parameters
            Parameter pPais = Parameter.CreateParameter("country",direccion["Pais"],ParameterType.QueryString);
            Parameter pCiudad = Parameter.CreateParameter("city",direccion["Ciudad"],ParameterType.QueryString);
            Parameter pProvincia = Parameter.CreateParameter("state",direccion["Provincia"],ParameterType.QueryString);
            Parameter pCalle = Parameter.CreateParameter("street", direccion["Numero"] +" "+ direccion["Calle"],ParameterType.QueryString);
            Parameter pCodPostal = Parameter.CreateParameter("postalcode",direccion["Codigo_Postal"],ParameterType.QueryString);
            Parameter pFormat = Parameter.CreateParameter("format","geojson",ParameterType.QueryString);
            Parameter pLimit = Parameter.CreateParameter("limit","1",ParameterType.QueryString);

            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddParameter(pPais);
            request.AddParameter(pCiudad);
            request.AddParameter(pProvincia);
            request.AddParameter(pCalle);
            request.AddParameter(pCodPostal);
            request.AddParameter(pFormat);
            #endregion

            var client = new RestClient("https://nominatim.openstreetmap.org/search");

            var response = client.ExecuteAsync(request);

            string content = response.Result.Content;
            JObject jsonObject = JObject.Parse(content);

            
            IList<JToken> results = jsonObject["features"].First["geometry"].Last.ToList();
            string[] coordenadas = results.FirstOrDefault().ToString().Replace("\r\n", "").TrimStart('[').TrimEnd(']').Split(",");
            string longitud = coordenadas[0].Trim();
            string latitud = coordenadas[1].Trim();
            

            direccion["Latitud"] = latitud;
            direccion["Longitud"] = longitud;
        }
    }
}
