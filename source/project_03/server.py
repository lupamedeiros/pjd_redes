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

def testa_jogada(jogada, tabuleiro):
    if jogada < 0 or jogada > 8:
        return False
    if tabuleiro[jogada] != '-':
        return False
    return True

def checa_vitoria(tabuleiro):
    if tabuleiro[0] == tabuleiro[1] and tabuleiro[1] == tabuleiro[2] and tabuleiro[0] != '-':
        return True
    if tabuleiro[3] == tabuleiro[4] and tabuleiro[4] == tabuleiro[5] and tabuleiro[3] != '-':
        return True
    if tabuleiro[6] == tabuleiro[7] and tabuleiro[7] == tabuleiro[8] and tabuleiro[6] != '-':
        return True
    if tabuleiro[0] == tabuleiro[3] and tabuleiro[3] == tabuleiro[6] and tabuleiro[0] != '-':
        return True
    if tabuleiro[1] == tabuleiro[4] and tabuleiro[4] == tabuleiro[7] and tabuleiro[1] != '-':
        return True
    if tabuleiro[2] == tabuleiro[5] and tabuleiro[5] == tabuleiro[8] and tabuleiro[2] != '-':
        return True
    if tabuleiro[0] == tabuleiro[4] and tabuleiro[4] == tabuleiro[8] and tabuleiro[0] != '-':
        return True
    if tabuleiro[2] == tabuleiro[4] and tabuleiro[4] == tabuleiro[6] and tabuleiro[2] != '-':
        return True
    return False


def jogo(p):
    print("\nNovo jogo - [" + p['O'].ip + " - " + p['X'].ip + "]")

    tabuleiro = "---------"

    i_buffer = ""
    o_buffer = ""

    jogador_ativo = 'O'
    contador_jogadas = 0

    while True:
        o_buffer =  jogador_ativo + tabuleiro
        p['O'].sock.send(o_buffer.encode('utf8'))
        p['X'].sock.send(o_buffer.encode('utf8'))

        i_buffer = ""
        i_buffer = p[jogador_ativo].sock.recv(1024)
        jogada = int(i_buffer) -1
        print("\nJogada recebida: " + str(jogada))

        if testa_jogada(jogada, tabuleiro):
            tabuleiro = tabuleiro[:jogada] + jogador_ativo + tabuleiro[jogada + 1:]
            contador_jogadas += 1
        else:
            continue

        if checa_vitoria(tabuleiro):
            o_buffer = "V" + jogador_ativo + tabuleiro
            p['O'].sock.send(o_buffer.encode('utf8'))
            p['X'].sock.send(o_buffer.encode('utf8'))
            break
        
        if contador_jogadas == 9:
            o_buffer = "E" + tabuleiro
            p['O'].sock.send(o_buffer.encode('utf8'))
            p['X'].sock.send(o_buffer.encode('utf8'))
            break

        if jogador_ativo == 'O':
            jogador_ativo = 'X'
        else:
            jogador_ativo = 'O'

    p['O'].sock.close()
    p['X'].sock.close()
    

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
        partidas.append({})

        print("\nAguardando Jogador 1...")
        c1, addr1 = s.accept()
        i_buffer = str(c1.recv(1024), encoding='utf8')
        partidas[num_partida]['O'] = jogador(addr1[0], 'O', c1, i_buffer)

        o_buffer = 'O'
        c1.send(o_buffer.encode('utf8'))
        
        print("\nAguardando Jogador 2...")
        c2, addr2 = s.accept()
        i_buffer = str(c2.recv(1024), encoding='utf8')
        partidas[num_partida]['X'] = jogador(addr2[0], 'X', c2, i_buffer)

        o_buffer = 'X'
        c2.send(o_buffer.encode('utf8'))

        print("\nIniciando partida " + str(num_partida))

        start_new_thread(jogo, ((partidas[num_partida],)))

        num_partida += 1

if __name__=="__main__":
    Main()