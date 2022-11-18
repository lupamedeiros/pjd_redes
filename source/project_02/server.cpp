// Server side C/C++ program to demonstrate Socket programming
#include <unistd.h>
#include <stdio.h>
#include <sys/socket.h>
#include <stdlib.h>
#include <netinet/in.h>
#include <string.h>
#include <pthread.h>

#define PORT 8080

void *connection_handler(void *socket_desc)
{
    // Recuperando o descritor do Socket
    int sock = *(int*)socket_desc;
    int read_size;
    char *message, client_message[2000];
    char nome[2000], matricula[2000];
    
    do {
        // Envia mensagem Inicial
        message = "\nConexão realizada.\n\nEscolha uma das opcoes do menu abaixo:\n1. Para informar seu nome\n0. Para encerar\n";
        write(sock, message, strlen(message));

        read(sock, client_message, 2000);

        if (client_message == "1")
        {
            message = "\nInforme seu nome: \n";
            write(sock, message, strlen(message));

            read(sock, client_message, 2000);
            *nome = *client_message;

            printf("%s\n", nome);
        }
        else if (client_message == "0")
        {
            message = "FIM";
            write(sock, message, strlen(message));
        }
    } while (client_message != "0");
}

int main(int argc, char const *argv[])
{
    int server_fd, new_socket, valread;
    struct sockaddr_in address;
    int opt = 1;
    int addrlen = sizeof(address);
    char buffer[1024] = {0};
    char *hello = "Ok! Eu sou o servidor!";
       
    // Criando o Descritor do Socket
    if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == 0)
    {
        perror("socket failed");
        exit(EXIT_FAILURE);
    }
       
    // Associando o Socked a um dercritor de arquivo
    if (setsockopt(server_fd, SOL_SOCKET, SO_REUSEADDR | SO_REUSEPORT,
                                                  &opt, sizeof(opt)))
    {
        perror("setsockopt");
        exit(EXIT_FAILURE);
    }
    address.sin_family = AF_INET;
    address.sin_addr.s_addr = INADDR_ANY;
    address.sin_port = htons( PORT );
       
    // Associando o Socket a Porta 8080
    if (bind(server_fd, (struct sockaddr *)&address, 
                                 sizeof(address))<0)
    {
        perror("bind failed");
        exit(EXIT_FAILURE);
    }
    if (listen(server_fd, 3) < 0)
    {
        perror("listen");
        exit(EXIT_FAILURE);
    }
    if ((new_socket = accept(server_fd, (struct sockaddr *)&address, 
                       (socklen_t*)&addrlen))<0)
    {
        perror("accept");
        exit(EXIT_FAILURE);
    }
    valread = read( new_socket , buffer, 1024);
    printf("%s\n",buffer );
    send(new_socket , hello , strlen(hello) , 0 );
    printf("Mensagem Enviada\n");
    return 0;
}
