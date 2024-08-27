using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    static void Main()
    {
        TcpListener server = null;
        try
        {
            IPAddress localAddr = IPAddress.Any;
            int port = 13000;
            server = new TcpListener(localAddr, port);
            server.Start();
            Console.WriteLine("Servidor iniciado na porta " + port);

            while (true)
            {
                Console.WriteLine("Aguardando conexão...");

                TcpClient client = server.AcceptTcpClient();

                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server?.Stop();
        }

        Console.WriteLine("\nPressione Enter para continuar...");
        Console.Read();
    }

    public static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = null;
        string nome = null;
        
        try
        {
            stream = client.GetStream();
            byte[] buffer = new byte[256];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                
                if (nome == null)
                {
                    nome = new string(data);
                    Console.WriteLine("Conectado com: {0}", data);
                }
                else
                {
                    Console.WriteLine("[{0}] enviou: {1}", nome, data);                    
                }
                
                byte[] msg = Encoding.ASCII.GetBytes("Mensagem recebida");
                stream.Write(msg, 0, msg.Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
        }
        finally
        {
            stream?.Close();
            client.Close();
        }
    }
}