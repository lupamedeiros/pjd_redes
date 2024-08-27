using System;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            //configura o servidor e a porta
            string server = "127.0.0.1";
            int port = 13000;

            //conecta ao servidor
            TcpClient client = new TcpClient(server, port);

            // Obtém o stream para ler e escrever dados
            NetworkStream stream = client.GetStream();

            // Buffer para armazenar os dados recebidos do servidor
            byte[] buffer = new byte[512];
            int bytesRead;

            // Recebe e exibe o menu inicial do servidor
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            string serverMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            Console.WriteLine(serverMessage);

            while (true)
            {
                // Lê a opção do usuário
                string option = Console.ReadLine() + "\n";

                // Envia a opção escolhida ao servidor
                byte[] data = Encoding.UTF8.GetBytes(option);
                stream.Write(data, 0, data.Length);

                // Recebe e exibe a resposta do servidor
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                serverMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                
                Console.WriteLine(serverMessage);

                // Se a opção for 0 (Sair), encerra a conexão
                if (option == "0")
                {
                    break;
                }
            }

            // Fecha o stream e o cliente
            stream.Close();
            client.Close();
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
        }
        
        Console.WriteLine("\nPressione Enter para sair");
        Console.Read();
    }
}

