// See https://aka.ms/new-console-template for more information

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketClient
{
    class Client
    {
        static void Main(string[] args)
        {
            // Define o IP do Servidor e a Porta
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 11000;

            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, port);
            
            // Cria um Socket TCP/IP
            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Conecta ao Servidor
                sender.Connect(remoteEndPoint);
                
                Console.WriteLine("Socket conectado a {0}", sender.RemoteEndPoint.ToString());
                
                // Envia dados ao servidor
                byte[] msg = Encoding.ASCII.GetBytes("Olá, servidor!");

                int bytesSent = sender.Send(msg);
                
                // Recebe a resposta do servidor
                byte[] bytes = new byte[1024];
                int bytesRec = sender.Receive(bytes);
                
                Console.WriteLine("Resposta recebida : {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
                
                // Fecha o Socket
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            Console.WriteLine("\nPressione ENTER para continuar...");
            Console.Read();
        }
    }
}

