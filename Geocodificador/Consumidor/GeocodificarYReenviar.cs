using Geocodificador.Interface;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Geocodificador.Geocodificar;

namespace Geocodificador.Consumidor
{
    public class GeocodificarYReenviar : IReceptor
    {
        /// <summary>
        /// Recibe el mensaje, obtiene las coordenadas y lo reenvia
        /// </summary>
        public void Receiver()
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var conecction = factory.CreateConnection())
            {
                using (var channel = conecction.CreateModel())
                {
                    //en el exchange procesar estan los datos pendienties
                    //en procesado los que estan listos
                    channel.ExchangeDeclare(exchange : "procesar" , type : ExchangeType.Fanout);

                    string queName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue : queName, exchange : "procesar", routingKey: "");

                    Console.WriteLine("Geocodificar Waiting for logs...");

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        string message = Encoding.UTF8.GetString(body);
                        Dictionary<string, string> geolocalizarJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                        IGeocodificar coordenadas = new GeocoficarEnCoordenadas();
                        coordenadas.Codificar(geolocalizarJson);
                        
                        //Console.WriteLine($"[x] Received: {message}");

                        #region reenviar a exchange procesado los geocodificados
                        
                        string bodyResponse = JsonConvert.SerializeObject(geolocalizarJson);
                        var prop = channel.CreateBasicProperties();
                        prop.Persistent = true;

                        channel.BasicPublish(exchange: "procesado", routingKey: "", basicProperties: prop, body: Encoding.UTF8.GetBytes(bodyResponse));

                        #endregion

                        //Confirma el ack para liberar el mensaje de la cola
                        //solo luego de geocodificar
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    };

                    //Se configura el autoack en manual para asegurarse de que el mensaje se procese
                    channel.BasicConsume(queue: queName, autoAck: false, consumer: consumer);


                    Console.WriteLine("Presione una tecla para salir...");
                    Console.ReadLine();


                }
            }
        }
    }
}
