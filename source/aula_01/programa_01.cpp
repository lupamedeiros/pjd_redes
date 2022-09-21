/*******************************************************************************
Autor: Luiz Paulo de Souza Medeiros
Disciplina: Programação de Jogos em Rede
Aula 01 - Programa 01 - Trabalhando com Tempo.
*******************************************************************************/

#include <ctime>
#include <iostream>

int main(){
    std::time_t result = std::time(nullptr);
    std::cout << std::asctime(std::localtime(&result))
              << result << " seconds since the Epoch\n";

    return 0;
}