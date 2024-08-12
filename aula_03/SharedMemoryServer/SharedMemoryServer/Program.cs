using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    // Número inteiro compartilhado entre os clientes
    private static int sharedNumber = 0;

    // Objeto para sincronizar o acesso ao número compartilhado
    private static readonly object lockObject = new object();

    static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            // Configura o endereço IP e a porta do servidor
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            int port = 13000;
            server = new TcpListener(localAddr, port);

            // Inicia o servidor
            server.Start();
            Console.WriteLine("Servidor iniciado na porta " + port);

            // Loop principal do servidor
            while (true)
            {
                Console.WriteLine("Aguardando conexão...");

                // Aceita uma nova conexão de cliente
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Conexão aceita!");

                // Cria uma nova thread para tratar a conexão
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // Para o servidor
            server?.Stop();
        }
    }

    // Método para tratar a conexão com o cliente
    private static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = null;
        try
        {
            stream = client.GetStream();
            byte[] buffer = new byte[256];
            int bytesRead;

            // Envia o menu ao cliente
            SendMenu(stream);

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string input = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                int option;
                if (int.TryParse(input, out option))
                {
                    switch (option)
                    {
                        case 1:
                            IncrementNumber(stream);
                            break;
                        case 2:
                            DecrementNumber(stream);
                            break;
                        case 3:
                            DisplayNumber(stream);
                            break;
                        case 0:
                            CloseConnection(stream);
                            return;
                        default:
                            SendInvalidOptionMessage(stream);
                            break;
                    }
                }
                else
                {
                    SendInvalidOptionMessage(stream);
                }

                // Reenvia o menu após a operação
                SendMenu(stream, "");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
        }
        finally
        {
            // Fecha o stream e o cliente
            stream?.Close();
            client.Close();
        }
    }

    private static void SendMenu(NetworkStream stream, string info)
    {
        string menu = info + 
                      "Menu:\n" +
                      "1. Incrementar o número em 1\n" +
                      "2. Decrementar o número em 1\n" +
                      "3. Exibir o número\n" +
                      "0. Sair da aplicação\n" +
                      "Escolha uma opção: ";
        byte[] msg = Encoding.UTF8.GetBytes(menu);
        stream.Write(msg, 0, msg.Length);
    }

    private static void IncrementNumber(NetworkStream stream)
    {
        lock (lockObject)
        {
            sharedNumber++;
        }
        string response = "Número incrementado.\n";
        SendMenu(stream, response);
    }

    private static void DecrementNumber(NetworkStream stream)
    {
        lock (lockObject)
        {
            sharedNumber--;
        }
        string response = "Número decrementado.\n";
        SendMenu(stream, response);
    }

    private static void DisplayNumber(NetworkStream stream)
    {
        string response;
        lock (lockObject)
        {
            response = $"O número atual é: {sharedNumber}\n";
        }
        SendMenu(stream, response);
    }

    private static void SendInvalidOptionMessage(NetworkStream stream)
    {
        string response = "Opção inválida. Tente novamente.\n";
        SendMenu(stream, response);
    }

    private static void CloseConnection(NetworkStream stream)
    {
        string response = "Conexão encerrada.\n";
        byte[] msg = Encoding.UTF8.GetBytes(response);
        stream.Write(msg, 0, msg.Length);
    }
}
