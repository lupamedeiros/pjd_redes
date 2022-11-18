"""_summary_
    Servidor para o Jogo da Velha On-Line.
    Desenvolvido por Luiz Paulo de Souza Medeiros para a disciplina de
    Programação de Jogos em Rede.
"""

import socket

from _thread import *
import threading

class jogador():
    def __init__(self, ip, peca, sock, nome="") -> None:
        self.ip = ip
        self.nome = nome
        self.peca = peca
        self.sock = sock

class partida():
    def __init__(self) -> None:
        j1 = None
        j2 = None
        tabuleiro = ""

def jogo(j):
    print("\nNovo jogo - [" + j['O'].ip + " - " + j['X'].ip + "]")

    tabuleiro = "---------"

    i_buffer = ""
    o_buffer = ""

    jogo_continua = True
    jogador_ativo = 'O'
    contador_jogadas = 0

    while jogo_continua:
        jogada_valida = False

        while not jogada_valida:
            pass

def Main():
    num_partida = 0
    partidas = []

    host = ""
    port = 8080

    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    s.bind((host, port))

    s.listen()

    while True:
        print("\nCriando partida " + str(num_partida))
        partidas.append[partida()]

        print("\nAguardando Jogador 1...")
        c1, addr1 = s.accept()
        i_buffer = str(c1.recv(1024), encoding='utf8')
        partidas[num_partida].j1 = jogador(addr1[0], 'O', c1, i_buffer)

        o_buffer = 'O'
        c1.send(o_buffer.encode('utf8'))
        
        print("\nAguardando Jogador 2...")
        c2, addr2 = s.accept()
        i_buffer = str(c2.recv(1024), encoding='utf8')
        partidas[num_partida].j2 = jogador(addr2[0], 'X', c2, i_buffer)

        o_buffer = 'X'
        c2.send(o_buffer.encode('utf8'))

        print("\nIniciando partida " + str(num_partida))

        start_new_thread(jogo, (partidas[num_partida]))

        num_partida += 1

if __name__=="__main__":
    Main()