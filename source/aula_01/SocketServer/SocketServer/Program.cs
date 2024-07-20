// See https://aka.ms/new-console-template for more information

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    class Server
    {
        static void Main(string[] args)
        {
            // Definir o IP e a PORTA em que o servidor irá escutar
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 11000;
            
            // Cria um endpoint de rede
            IPEndPoint localEndpoint = new IPEndPoint(ipAddress, port);
            
            // Cria um socket TCP/IP
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Associa o Socket ao EndPoint e escuta as conexões
                listener.Bind(localEndpoint);
                listener.Listen(10);
                
                Console.WriteLine("Aguardando por uma Conexão...");
                
                // Inicia a escuta de conexões de entrada
                while (true)
                {
                    // Programa está Bloqueado enquanto espera por uma conexão
                    Socket handler = listener.Accept();
                    string data = null;
                    
                    // Buffer para armazenar os dados recbidos
                    byte[] bytes = new byte[1024];
                    int bytesReceived = handler.Receive(bytes);

                    data += Encoding.ASCII.GetString(bytes, 0, bytesReceived);
                    
                    // Exibe a Mensagem Recebia
                    Console.WriteLine("Texto recebido : {0}", data);
                    
                    // Envia uma mensagem  de confirmação de volta oa cliente
                    byte[] msg = Encoding.ASCII.GetBytes("Menssagem recebida");
                    handler.Send(msg);
                    
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
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