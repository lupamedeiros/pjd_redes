using System;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Configura o endereço IP e a porta do servidor
            string server = "127.0.0.1";
            int port = 13000;

            // Cria um cliente TCP e conecta ao servidor
            TcpClient client = new TcpClient(server, port);

            // Obtém o stream de rede para enviar e receber dados
            NetworkStream stream = client.GetStream();

            // Buffer para armazenar os dados recebidos do servidor
            byte[] buffer = new byte[256];
            int bytesRead;

            // Recebe e exibe o menu inicial
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            // Loop para enviar comandos ao servidor
            while (true)
            {
                // Lê a opção escolhida pelo usuário
                string option = Console.ReadLine();

                // Envia a opção escolhida ao servidor
                byte[] data = Encoding.UTF8.GetBytes(option + "\n");
                stream.Write(data, 0, data.Length);

                // Recebe a resposta do servidor
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine(response);

                // Se o usuário escolher "0", sair do loop
                if (option == "0")
                {
                    break;
                }
            }

            // Fecha o stream e o cliente
            stream.Close();
            client.Close();
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

        Console.WriteLine("\nPressione Enter para sair...");
        Console.Read();
    }
}
