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
                NetworkStream stream = player.GetStream();

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
                        byte[] buffer = new byte[8];
                        string msg = "1";
                        buffer = Encoding.UTF8.GetBytes(msg);
                        stream.Write(buffer, 0, buffer.Length);
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
    private char[] board = new char[9];
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
        string msg = "0";

        buffer = Encoding.UTF8.GetBytes(msg);
        stream.Write(buffer, 0, buffer.Length);
        
        SendBoard(stream, "");

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
                        string result = "";
                        if (CheckWin(symbol == 'X' ? 'O' : 'X'))
                        {
                            result = currentPlayer == 1 ? "2" : "1";
                            SendBoard(stream, result);
                            break;
                        }

                        if (CheckDraw())
                        {
                            SendBoard(stream, "3");
                            break;
                        }
                    }

                    // Envia uma mensagem solicitando o movimento
                    msg = symbol == 'X' ? "X" : "O";
                    buffer = Encoding.UTF8.GetBytes(msg);
                    stream.Write(buffer, 0, buffer.Length);

                    // Lê o movimento do jogador
                    int position = 0;
                    while (true)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string input = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        bool parse = int.TryParse(input, out position);

                        if (!parse || position < 1 || position > 9)
                        {
                            msg = "-1";
                            buffer = Encoding.UTF8.GetBytes(msg);
                            stream.Write(buffer, 0, buffer.Length);
                            Console.WriteLine($"Jogo {id}: jogador {playerNumber} enviou a jogada inválida {input}");
                            continue;
                        }

                        if (board[position - 1] != '-')
                        {
                            msg = "-1";
                            buffer = Encoding.UTF8.GetBytes(msg);
                            stream.Write(buffer, 0, buffer.Length);
                            Console.WriteLine($"Jogo {id}: jogador {playerNumber} enviou a jogada inválida {input}");
                            continue;
                        }

                        break;
                    }

                    msg = "1";
                    buffer = Encoding.UTF8.GetBytes(msg);
                    stream.Write(buffer, 0, buffer.Length);
                    
                    Console.WriteLine($"Jogo {id}: jogador {playerNumber} marcou a posição {position}");
                    board[position - 1] = symbol;
                    
                    if (CheckWin(symbol))
                    {
                        gameEnded = true;
                        SendBoard(stream, symbol == 'X' ? "1" : "2");
                        Console.WriteLine($"Jogo {id} terminou: jogador {playerNumber} venceu");
                        break;
                    }

                    if (CheckDraw())
                    {
                        gameEnded = true;
                        SendBoard(stream, "3");
                        Console.WriteLine($"Jogo {id} terminou em empate");
                        break;
                    }

                    currentPlayer = currentPlayer == 1 ? 2 : 1;
                    
                    Monitor.PulseAll(lockObject); // Permite que o outro jogador jogue
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
        for (int i = 0; i < 9; i++)
        {
            board[i] = '-';
        }
    }

    private void SendBoard(NetworkStream stream, String result)
    {
        string boardString = board + result;
        byte[] msg = Encoding.UTF8.GetBytes(boardString);
        stream.Write(msg, 0, msg.Length);
    }

    private bool CheckWin(char symbol)
    {
        // Verifica linhas, colunas e diagonais
        if ((board[0] == symbol && board[1] == symbol && board[2] == symbol) ||
            (board[3] == symbol && board[4] == symbol && board[5] == symbol) ||
            (board[6] == symbol && board[7] == symbol && board[8] == symbol) ||
            (board[0] == symbol && board[3] == symbol && board[6] == symbol) ||
            (board[1] == symbol && board[4] == symbol && board[7] == symbol) ||
            (board[2] == symbol && board[5] == symbol && board[6] == symbol) ||
            (board[0] == symbol && board[4] == symbol && board[8] == symbol) ||
            (board[2] == symbol && board[4] == symbol && board[6] == symbol))
        {
            return true;
        }
        
        return false;
    }

    private bool CheckDraw()
    {
        foreach (char spot in board)
        {
            if (spot == '-')
            {
                return false;
            }
        }
        return true;
    }
}
