import socket
def imprimir_tabuleiro(tabuleiro):
    print(" " + tabuleiro[0] + " | " + tabuleiro[1] + " | " + tabuleiro[2] + " ")
    print("---+---+---")
    print(" " + tabuleiro[3] + " | " + tabuleiro[4] + " | " + tabuleiro[5] + " ")
    print("---+---+---")
    print(" " + tabuleiro[6] + " | " + tabuleiro[7] + " | " + tabuleiro[8] + " ")

def Main():

    # Define IP do Servidor e Porta
    server = "127.0.0.1"
    port = 8080

    # Leio nome do Jogador do Teclado
    nome = input("Digite seu nome: ")

    # Cria o socket e conecta ao servidor
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect((server, port))

    print("Conectado ao servidor")
    i_buffer = ""

    # Envio o Nome do Jogador
    s.send(nome.encode('utf8'))

    # Recebo a peça que irei jogar
    i_buffer = str(s.recv(1024), encoding='utf8')
    print("Você jogará com " + i_buffer)

    # Definções de Variaveis Jogo
    peca = i_buffer
    tabuleiro = "---------"

    # Loop para enviar e receber mensagens
    while True:
        # Recebo estado atual + tabuleiro
        i_buffer = str(s.recv(1024), encoding='utf8')
        
        # Verifico se houve vitoria
        if i_buffer[0] == "V":
            tabuleiro = i_buffer[2:]
            imprimir_tabuleiro(tabuleiro)
            if i_buffer[1] == peca:
                print("Você venceu!\n")
            else:
                print("Você perdeu... =( \n")
            break

        # Copio tabuleiro para outra variavel e imprimo na tela
        tabuleiro = i_buffer[1:]
        imprimir_tabuleiro(tabuleiro)

        # Verifico se houve empate
        if i_buffer[0] == "E":
            print("Empate!\n")
            break

        # Verifico se eh minha vez
        if i_buffer[0] == peca:
            # Leio a jogada e envio para o servidor
            print("É sua vez de jogar")
            o_buffer = input("Digite a posição que deseja jogar: ")
            s.send(o_buffer.encode('utf8'))
        else:
            # Informo que estou aguardando o outro jogador
            print("Aguardando o outro jogador")

if __name__=="__main__":
    Main()