using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

class TicTacToeServer
{
    private static readonly object lockObject = new object();
    private static List<GameRoom> gameRooms = new List<GameRoom>();

    static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            IPAddress localAddr = IPAddress.Any;
            int port = 13000;
            server = new TcpListener(localAddr, port);
            server.Start();
            Console.WriteLine("Servidor do Jogo da Velha iniciado na porta " + port);
            int gameCounter = 0;

            while (true)
            {
                // Aceitar novos jogadores
                TcpClient player = server.AcceptTcpClient();
                Console.WriteLine("Jogador conectado!");

                // Tentar encontrar uma sala incompleta
                GameRoom room = null;
                lock (lockObject)
                {
                    room = gameRooms.Find(r => r.IsWaitingForPlayer);
                    if (room == null)
                    {
                        // Criar uma nova sala se não houver disponível
                        room = new GameRoom(gameCounter++);
                        gameRooms.Add(room);
                    }
                }

                // Adicionar jogador à sala
                room.AddPlayer(player);
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

        Console.WriteLine("\nPressione Enter para sair...");
        Console.Read();
    }
}

class GameRoom
{
    private TcpClient player1;
    private TcpClient player2;
    private char[,] board = new char[3, 3];
    private int currentPlayer = 1; // 1 = Player 1, 2 = Player 2
    private readonly object lockObject = new object();
    private bool gameEnded = false;
    private int id;

    public bool IsWaitingForPlayer { get; private set; } = true;

    public GameRoom(int gameId)
    {
        id = gameId;
        InitializeBoard();
    }

    public void AddPlayer(TcpClient player)
    {
        lock (lockObject)
        {
            if (player1 == null)
            {
                player1 = player;
                Thread playerThread = new Thread(() => HandlePlayer(player1, 'X'));
                playerThread.Start();
            }
            else if (player2 == null)
            {
                player2 = player;
                IsWaitingForPlayer = false;
                Thread playerThread = new Thread(() => HandlePlayer(player2, 'O'));
                playerThread.Start();
            }
        }
    }

    private void HandlePlayer(TcpClient client, char symbol)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[256];
        int playerNumber = symbol == 'X' ? 1 : 2;

        try
        {
            while (!gameEnded)
            {
                lock (lockObject)
                {
                    // Verifica se é a vez do jogador
                    if (currentPlayer != playerNumber)
                    {
                        Monitor.Wait(lockObject);
                    }

                    // Envia o tabuleiro atual para o jogador
                    SendBoard(stream);

                    // Envia uma mensagem solicitando o movimento
                    string prompt = $"Jogador {playerNumber} ({symbol}), faça seu movimento (linha e coluna): ";
                    byte[] msg = Encoding.UTF8.GetBytes(prompt);
                    stream.Write(msg, 0, msg.Length);

                    // Lê o movimento do jogador
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string input = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    if (int.TryParse(input, out int position) && position >= 1 && position <= 9)
                    {
                        int row = (position - 1) / 3;
                        int col = (position - 1) % 3;

                        if (board[row, col] == ' ')
                        {
                            Console.WriteLine($"Jogo {id}: jogador {playerNumber} marcou a posição {position}");
                            board[row, col] = symbol;
                            currentPlayer = currentPlayer == 1 ? 2 : 1;

                            if (CheckWin(symbol))
                            {
                                gameEnded = true;
                                SendBoard(stream);
                                string winMessage = $"Jogador {playerNumber} ({symbol}) venceu!\n";
                                msg = Encoding.UTF8.GetBytes(winMessage);
                                stream.Write(msg, 0, msg.Length);
                                Console.WriteLine($"Jogo {id} terminou: jogador {playerNumber} venceu");
                                break;
                            }
                            else if (CheckDraw())
                            {
                                gameEnded = true;
                                SendBoard(stream);
                                string drawMessage = "O jogo terminou em empate!\n";
                                msg = Encoding.UTF8.GetBytes(drawMessage);
                                stream.Write(msg, 0, msg.Length);
                                Console.WriteLine($"Jogo {id} terminou em empate");
                                break;
                            }
                            
                            Monitor.PulseAll(lockObject); // Permite que o outro jogador jogue
                        }
                        else
                        {
                            string invalidMoveMessage = "Movimento inválido, tente novamente.\n";
                            msg = Encoding.UTF8.GetBytes(invalidMoveMessage);
                            stream.Write(msg, 0, msg.Length);
                        }
                    }
                    else
                    {
                        string invalidInputMessage = "Entrada inválida, digite um número de 1 a 9.\n";
                        msg = Encoding.UTF8.GetBytes(invalidInputMessage);
                        stream.Write(msg, 0, msg.Length);
                    }
                }
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

    private void InitializeBoard()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                board[i, j] = ' ';
            }
        }
    }

    private void SendBoard(NetworkStream stream)
    {
        string boardString = "\n" +
            $" {board[0, 0]} | {board[0, 1]} | {board[0, 2]} \n" +
            "---+---+---\n" +
            $" {board[1, 0]} | {board[1, 1]} | {board[1, 2]} \n" +
            "---+---+---\n" +
            $" {board[2, 0]} | {board[2, 1]} | {board[2, 2]} \n\n";
        byte[] msg = Encoding.UTF8.GetBytes(boardString);
        stream.Write(msg, 0, msg.Length);
    }

    private bool CheckWin(char symbol)
    {
        // Verifica linhas, colunas e diagonais
        for (int i = 0; i < 3; i++)
        {
            if ((board[i, 0] == symbol && board[i, 1] == symbol && board[i, 2] == symbol) ||
                (board[0, i] == symbol && board[1, i] == symbol && board[2, i] == symbol))
            {
                return true;
            }
        }
        if ((board[0, 0] == symbol && board[1, 1] == symbol && board[2, 2] == symbol) ||
            (board[0, 2] == symbol && board[1, 1] == symbol && board[2, 0] == symbol))
        {
            return true;
        }
        return false;
    }

    private bool CheckDraw()
    {
        foreach (char spot in board)
        {
            if (spot == ' ')
            {
                return false;
            }
        }
        return true;
    }
}
