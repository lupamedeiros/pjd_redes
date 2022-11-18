import socket

from _thread import *
import threading

def threaded(c,a):
    print("\nNova Conexão: " + a[0])
    while True:
        i_buffer = ""
        o_buffer = "\nEscolha uma das opcoes do menu abaixo:\n"\
                   "1. Para informar seu nome\n"\
                   "0. Para encerar\n\0"
        c.send(o_buffer.encode('utf8'))

        i_buffer = str(c.recv(1024), encoding='utf8')

        print("\n[" + a[0] + "] Opção " + i_buffer)

        if i_buffer == "1":
            o_buffer = "1"
            c.send(o_buffer.encode('utf8'))
            i_buffer = str(c.recv(1024), encoding='utf8')
            print("\n[" + a[0] + "] Nome: " + i_buffer)
            continue

        elif i_buffer == "0":
            o_buffer = "0"
            c.send(o_buffer.encode('utf8'))
            break

        else:
            o_buffer = "?"
            c.send(o_buffer.encode('utf8'))
            continue

    c.close()
    
    print("\n[" + a[0] + "]Conexão Encerrada")

def Main():
    host = ""
    port = 8080
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    s.bind((host, port))
    
    s.listen(10)
    print("Aguardando conxões...")

    while True:
        c, addr = s.accept()
        start_new_thread(threaded, (c,addr))
    
    s.close()

if __name__=="__main__":
    Main()