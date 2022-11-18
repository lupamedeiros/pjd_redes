// Exemplo de aplicação cliente utilizando Socket em C/C++
#include <stdio.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <string.h>
#define PORT 8080
   
int main(int argc, char const *argv[])
{
    // Cria variáveis para manipular o socket
    int sock = 0, valread;
    struct sockaddr_in serv_addr;
    
    // Criação Buffers
    char *hello = "Olá, eu sou o cliente!";
    char buffer[1024] = {0};

    // Criar o Socket
    if ((sock = socket(AF_INET, SOCK_STREAM, 0)) < 0)
    {
        printf("\n Socket creation error \n");
        return -1;
    }
   
    // Conexão com o Socket do Servidor
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_port = htons(PORT);
       
    // Converte o Endereço IP de Texto para o formato binário
    if(inet_pton(AF_INET, "127.0.0.1", &serv_addr.sin_addr)<=0) 
    {
        printf("\nInvalid address/ Address not supported \n");
        return -1;
    }
   
    if (connect(sock, (struct sockaddr *)&serv_addr, sizeof(serv_addr)) < 0)
    {
        printf("\nConnection Failed \n");
        return -1;
    }
    
    // Comportamento do meu programa
    
    // Enviar Mensagem
    send(sock , hello , strlen(hello) , 0 );
    printf("Mensagem Enviada\n");
 
    // Receber Mensagem
    valread = read( sock , buffer, 1024);
    printf("%s\n",buffer );


    return 0;
}
