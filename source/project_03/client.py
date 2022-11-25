import socket
def imprimir_tabuleiro(tabuleiro):
    print(" " + tabuleiro[0] + " | " + tabuleiro[1] + " | " + tabuleiro[2] + " ")
    print("---+---+---")
    print(" " + tabuleiro[3] + " | " + tabuleiro[4] + " | " + tabuleiro[5] + " ")
    print("---+---+---")
    print(" " + tabuleiro[6] + " | " + tabuleiro[7] + " | " + tabuleiro[8] + " ")

def Main():

    # Define ip do servidor e porta
    server = "127.0.0.1"
    port = 8080

    nome = input("Digite seu nome: ")

    # Cria o socket e conecta ao servidor
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect((server, port))

    print("Conectado ao servidor")
    i_buffer = ""

    s.send(nome.encode('utf8'))

    i_buffer = str(s.recv(1024), encoding='utf8')
    print("Você jogará com " + i_buffer)

    # Definções do Jogo
    peca = i_buffer
    tabuleiro = "---------"

    # Loop para enviar e receber mensagens
    while True:
        i_buffer = str(s.recv(1024), encoding='utf8')
        
        if i_buffer[0] == "V":
            tabuleiro = i_buffer[2:]
            if i_buffer[1] == peca:
                imprimir_tabuleiro(tabuleiro)
                print("Você venceu!\n")
            else:
                imprimir_tabuleiro(tabuleiro)
                print("Você perdeu!\n")
            break

        tabuleiro = i_buffer[1:]
        imprimir_tabuleiro(tabuleiro)

        if i_buffer[0] == "E":
            print("Empate!\n")
            break

        if i_buffer[0] == peca:
            print("É sua vez de jogar")
            o_buffer = input("Digite a posição que deseja jogar: ")
            s.send(o_buffer.encode('utf8'))
        else:
            print("Aguardando o outro jogador")

if __name__=="__main__":
    Main()