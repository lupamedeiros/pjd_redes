/*******************************************************************************
Autor: Luiz Paulo de Souza Medeiros
Disciplina: Programação de Jogos em Rede
Aula 01 - Programa 01 - Implementando o Jogo da Velha
*******************************************************************************/

#include <iostream> // biblioteca padrão de entrada e saída

using namespace std;

void imprimir_tabuleiro(char* tabuleiro){
    cout << " " << tabuleiro[0] << " " << tabuleiro[1] << " " << tabuleiro[2] << " " << endl
         << " " << tabuleiro[3] << " " << tabuleiro[4] << " " << tabuleiro[5] << " " << endl
         << " " << tabuleiro[6] << " " << tabuleiro[7] << " " << tabuleiro[8] << " " << endl
         << endl;
}

// Fluxo principal do Jogo
int main(){

    /* ESTADO INICIAL ==========================================================
     * - Criação dos elementos do jogo:
     *   - Tabuleiro
     *   - Jogadores
    */

    // Criação do Tabuleiro
    char tabuleiro[9]; // Tabuleiro em char[] com 9 posições

    for (int i=0; i<9; i++){ // Inicialização do Tabuleiro
        tabuleiro[i] = '-';
    }

    cout << "Jogo da Velha (versão off-line)!" << endl;

    // Identificação dos Jogadores
    string jogador_1, jogador_2;

    cout << "Insira o nome para o Jogador 1 (O): ";
    cin >> jogador_1;

    if (jogador_1 == "")
        jogador_1 = "Jogador 1";

    cout << "Insira o nome para o Jogador 2 (X): ";
    cin >> jogador_2;

    if (jogador_2 == "")
        jogador_2 = "Jogador 2";

    /* FLUXO DE TURNO ==========================================================
     * - Identificar de quem é o turno
     * - Receber a posição de jogada do jogador
     * - Identificar a condição de parada
    */

    // Iniciliação das Variáveis para controle do Fluxo de jogo
    bool jogo_continua = true;
    int posicao_jogada = 0;
    char jogador_ativo = 'O';
    
    while (jogo_continua){
        // Imprimir Tabuleiro
        imprimir_tabuleiro(tabuleiro);

        // Receber a posição de jogada;
        cout << "Indique a posição que quer jogar (1-9): ";
        cin >> posicao_jogada;
        
        tabuleiro[posicao_jogada -1] = jogador_ativo;
        
        if (jogador_ativo == 'O')
            jogador_ativo = 'X';
        else
            jogador_ativo = 'O';

        // Identificar condição de parada;
        jogo_continua = true; // Deverá receber false se o jogo encerrar;
    }
    
    /* FIM DO JOGO
     - Exibir o resultado.
    */

    return 0;
}