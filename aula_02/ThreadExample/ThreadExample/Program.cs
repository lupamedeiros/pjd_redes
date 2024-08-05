using System;
using System.Threading;

class Program
{
    static void Main()
    {
        // Cria uma nova thread que executa o método PrintNumbers
        Thread thread = new Thread(PrintNumbers);
        
        // Inicia a thread
        thread.Start();
        
        // Método executado na thread principal
        PrintNumbers();
        
        // Aguarda a conclusão da thread
        thread.Join();

        Console.WriteLine("Fim do programa");
    }

    // Método que será executado pela thread
    static void PrintNumbers()
    {
        // Obtém o identificador da thread atual
        string threadId = Thread.CurrentThread.ManagedThreadId.ToString();
        
        for (int i = 1; i <= 5; i++)
        {
            Console.WriteLine($"Thread {threadId} - Número: {i}");
            // Pausa de 1 segundo entre cada número
            Thread.Sleep(1000);
        }
    }
}