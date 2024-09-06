using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

class TicTacToeServer
{
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
                room = gameRooms.Find(r => r.IsWaitingForPlayer);
                if (room == null)
                {
                    // Criar uma nova sala se não houver disponível
                    room = new GameRoom(gameCounter++);
                    gameRooms.Add(room);
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
        Console.WriteLine($"Jogo {id} criado.");
        InitializeBoard();
    }

    public void AddPlayer(TcpClient player)
    {
        lock (lockObject)
        {
            if (player1 == null)
            {
                Console.WriteLine("Entrou o Jogador 1");
                player1 = player;
                NetworkStream stream = player.GetStream();
                string msg = "1";
                byte[] buffer = new byte[8];
                buffer = Encoding.UTF8.GetBytes(msg);
                stream.Write(buffer, 0, buffer.Length);
            }
            else if (player2 == null)
            {
                Console.WriteLine("Entrou o Jogador 2");
                player2 = player;
                IsWaitingForPlayer = false;
                Thread gameThread = new Thread(() => HandleGame(player1, player2));
                gameThread.Start();
            }
        }
    }

    private void HandleGame(TcpClient player1, TcpClient player2)
    {
        NetworkStream[] stream = new NetworkStream[2];
        stream[0] = player1.GetStream();
        stream[1] = player2.GetStream();
        byte[] inBuffer = new byte[256];
        byte[] outBuffer = new byte[256];
        int currentPlayer = 1;
        char currentSymbol = 'X';
        string msg = "0";
        int bytesRead = 0;
        bool parse = false;
        int position = 0;

        outBuffer = Encoding.UTF8.GetBytes(msg);
        stream[0].Write(outBuffer, 0, outBuffer.Length);
        stream[1].Write(outBuffer, 0, outBuffer.Length);
        
        SendBoard(stream[0], "");
        SendBoard(stream[1], "");

        try
        {
            while (!gameEnded)
            {
                outBuffer = Encoding.UTF8.GetBytes(currentSymbol == 'X' ? "X" : "O");
                stream[currentPlayer-1].Write(outBuffer, 0, outBuffer.Length);

                while (true)
                {
                    bytesRead = stream[currentPlayer - 1].Read(inBuffer, 0, inBuffer.Length);
                    msg = Encoding.UTF8.GetString(inBuffer, 0, bytesRead);

                    parse = int.TryParse(msg, out position);

                    if (!parse || position < 1 || position > 9)
                    {
                        Console.WriteLine(position);
                        msg = "-1";
                        outBuffer = Encoding.UTF8.GetBytes(msg);
                        stream[currentPlayer - 1].Write(outBuffer, 0, outBuffer.Length);
                        Console.WriteLine($"Jogo {id}: jogador {currentPlayer} enviou uma jogada inválida.");
                        continue;
                    }

                    if (board[position - 1] != '-')
                    {
                        msg = "-1";
                        outBuffer = Encoding.UTF8.GetBytes(msg);
                        stream[currentPlayer - 1].Write(outBuffer, 0, outBuffer.Length);
                        Console.WriteLine($"Jogo {id}: jogador {currentPlayer} escolheu um espaço ocupado.");
                        continue;
                    }

                    break;
                }

                msg = "1";
                outBuffer = Encoding.UTF8.GetBytes(msg);
                stream[currentPlayer-1].Write(outBuffer, 0, outBuffer.Length);
                
                Console.WriteLine($"Jogo {id}: jogador {currentPlayer} marcou a posição {position}");
                board[position - 1] = currentSymbol;
                
                if (CheckWin(currentSymbol))
                {
                    gameEnded = true;
                    SendBoard(stream[0], currentSymbol == 'X' ? "1" : "2");
                    SendBoard(stream[1], currentSymbol == 'X' ? "1" : "2");
                    Console.WriteLine($"Jogo {id} terminou: jogador {currentPlayer} venceu");
                    break;
                }

                if (CheckDraw())
                {
                    gameEnded = true;
                    SendBoard(stream[0], "3");
                    SendBoard(stream[1], "3");
                    Console.WriteLine($"Jogo {id} terminou em empate");
                    break;
                }

                currentPlayer = currentPlayer == 1 ? 2 : 1;
                currentSymbol = currentSymbol == 'X' ? 'O' : 'X';
                
                SendBoard(stream[0], "");
                SendBoard(stream[1], "");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e);
        }
        finally
        {
            stream[0]?.Close();
            stream[1]?.Close();
            player1.Close();
            player2.Close();
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
        string boardString = new string(board) + result;
        Console.WriteLine(boardString);
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
            (board[2] == symbol && board[5] == symbol && board[8] == symbol) ||
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
