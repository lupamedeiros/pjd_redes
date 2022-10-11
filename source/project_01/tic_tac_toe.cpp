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
    int contador_jogadas = 0;
    
    while (jogo_continua){
        // Imprimir Tabuleiro
        imprimir_tabuleiro(tabuleiro);

        // Receber a posição de jogada;
        bool jogada_valida = false;

        while (!jogada_valida){
            cout << ((jogador_ativo == 'O') ? jogador_1 : jogador_2)
                 << ", indique a posição em que quer jogar (1-9): ";

            cin >> posicao_jogada;

            if (posicao_jogada < 1 || posicao_jogada > 9) // Verifica se valor digitado está entre 1 e 9
                cout << "Posição inválida!" << endl;
            else if (tabuleiro[posicao_jogada -1] != '-') // Verifica se a posição está disponível
                cout << "Posição não disponível!" << endl;
            else
                jogada_valida = true;
        }
        
        // Enviar posição para o tabuleiro
        tabuleiro[posicao_jogada -1] = jogador_ativo;
        contador_jogadas++;
        
        // Verificar Condições de Parada
        // Vitória
        if ((tabuleiro[0] == jogador_ativo && tabuleiro[1] == jogador_ativo && tabuleiro[2] == jogador_ativo) || // Lin 1
            (tabuleiro[3] == jogador_ativo && tabuleiro[4] == jogador_ativo && tabuleiro[5] == jogador_ativo) || // Lin 2
            (tabuleiro[6] == jogador_ativo && tabuleiro[7] == jogador_ativo && tabuleiro[8] == jogador_ativo) || // Lin 3
            (tabuleiro[0] == jogador_ativo && tabuleiro[3] == jogador_ativo && tabuleiro[6] == jogador_ativo) || // Col 1
            (tabuleiro[1] == jogador_ativo && tabuleiro[4] == jogador_ativo && tabuleiro[7] == jogador_ativo) || // Col 2
            (tabuleiro[2] == jogador_ativo && tabuleiro[5] == jogador_ativo && tabuleiro[8] == jogador_ativo) || // Col 3
            (tabuleiro[0] == jogador_ativo && tabuleiro[4] == jogador_ativo && tabuleiro[8] == jogador_ativo) || // Diag Principal
            (tabuleiro[2] == jogador_ativo && tabuleiro[4] == jogador_ativo && tabuleiro[6] == jogador_ativo)) { // Diag Inversa

            cout << " ******************************** " << endl
                 << " " << ((jogador_ativo == 'O') ? jogador_1 : jogador_2)
                 << " VENCEU!! =D" << endl
                 << " ******************************** " << endl;
            
            imprimir_tabuleiro(tabuleiro);

            jogo_continua = false;
            break;
        }
        // Verificar quantidade de jogadas realizadas
        else if (contador_jogadas == 9) {
            cout << " ******************************** " << endl
                 << " Empate. =/" << endl
                 << " ******************************** " << endl;

            imprimir_tabuleiro(tabuleiro);

            jogo_continua = false;
            break;
        }

        if (jogador_ativo == 'O')
            jogador_ativo = 'X';
        else
            jogador_ativo = 'O';
    
    }

    return 0;
}