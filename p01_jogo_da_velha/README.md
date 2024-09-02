# Projeto Jogo da Velha

Nesse projeto deverá ser desenvolvido o programa cliente, compatível com o servidor apresentado, 
para o jogo da velha.

Como material para avaliação deverá ser apresentado:

- Relatório contendo:
  - Fluxograma para a aplicação cliente;
  - Descrição do código-fonte correspondente a cada bloco do diagrama;
- Repositório do GitHub com o código da aplicação desenvolvida.

## Servidor

Nesta seção serão apresentadas as características de funcionamento do servidor disponibilizado. É importante compreender bem o funcionamento do servidor para desenvolver um cliente capaz de interagir corretamente com ele.

### Características

- O Servidor irá aguardar solicitações de conexão dos clientes e os organizará em Salas de Jogo conforme a ordem de conexão.
- O Servidor irá aguardar conexões na porta 13000.
- Cada Sala de Jogo abrigará 2 jogadores (duas conexões simultâneas) que irão interagir naquela partida.
- O primeiro jogador e se conectar será o primeiro a jogar.
- O Servidor irá coordenar o fluxo de comunicação, enviando as informações necessárias para cada cliente, e aguardando respostas.
- O Servidor será responsável pela manutenção do tabuleiro de jogo e pela integridades das informações contidas nele.
- O Servidor irá identificar condições de finalização do jogo (vitória de um dos jogadores ou não haver mais espaços disponíveis para jogadas).

### Fluxograma

O Fluxograma a seguir apresenta o comportamento do servidor.

```mermaid
graph TD
A(Iniciar Servidor) --> B[Aguardar Conexões]
B --> C[Jogador Conectado]
C --> D{Sala de Jogo <br> Disponível?}
D -- Não --> E[Criar Nova Sala de Jogo]
D -- Sim --> F[Adicionar Jogador <br> à Sala Existente]
E --> F
F --> G{Sala Completa?}
G -- Não --> Z[Notificar que está aguardando outro jogador.]
Z --> B
G -- Sim --> W[Informar Jogadores que <br> a Partida irá começar]
W --> B
W --> H[Iniciar Jogo na Sala]
H --> I[Envia o Tabuleiro para <br> os dois jogadores]
I --> J[Envia solicitação de Jogada <br> para o Jogador Ativo]
J --> K[Recebe Jogada do <br> Jogador Ativo]
K --> L{Jogada entre 1 e 9?}
L -- Não --> M[Envia informação de jogada <br> inválida para o Jogador Ativo]
L -- Sim --> N{Posição escolhida <br> está Livre?}
N -- Não --> M
M --> K
N -- Sim --> O[Envia confirmação de jogada <br> válida para o Jogador Ativo]
O --> P[Atualiza o Tabuleiro]
P --> R{Vitória?}
R -- Sim --> S[Envia Tabuleiro e indicação <br> de vitória para os Jogadores]
R -- Não --> T{Empate?}
T -- Sim --> U[Envia Tabuleiro e indicação <br> de empate para os Jogadores]
S --> X(Encerra a Sala de Jogo)
U --> X
T -- Não --> V[Alterna o Jogador Ativo]
V --> Y[Envia o tabuleiro atualizado <br> para os jogadores]
Y --> J
```

### Mensagens

- Notificação que está aguardando outro jogador:
  - `"1"`
- Informar jogadores que a partida vai começar:
  - `"0"`
- Enviar Tabuleiro
  - `"---------"`
  - Será enviada uma string com tamanho 9, onde cada posição da string representa uma posição do tabuleiro
- Solicitação de Jogada para o Jogador Ativo
  - `"X"` ou `"O"`
  - Será enviado o símbolo do próprio jogador
- Envio da Jogada pelo Cliente
  - `"1"`, `"2"`, `"3"`, `"4"`, `"5"`, `"6"`, `"7"`, `"8"` ou `"9"`
  - O cliente deverá enviar um número de 1 a 9
- Confirmação de Jogada pelo Servidor:
  - Jogada inválida:
    - `"-1"`
  - Jogada Válida
    - `"1"`
- Mensagem do Servidor em caso de Vitória:
  - `"---------1"` ou `"---------2"`
  - O servidor enviará o tabuleiro seguido do número que identifique o Jogador
- Mensagem do Servidor em caso de Empate:
  - `"---------3"`
  - O servidor enviará o tabuleiro seguido do número 3

### Diagrama de Sequência

O Diagrama de Sequência abaixo apresenta um exemplo de comunicação entre o servidor e 2 clientes.

```mermaid
sequenceDiagram
    Cliente01 ->> Servidor: connect()
    Servidor ->> Cliente01: "1"
    Cliente02 ->> Servidor: connect()
    Servidor ->> Cliente02: "0"
    Servidor ->> Cliente01: "0"
    Servidor ->> Cliente01: "---------"
    Servidor ->> Cliente02: "---------"
    Servidor ->> Cliente01: "O"
    Cliente01 ->> Servidor: "1"
    Servidor ->> Cliente01: "1"
    Servidor ->> Cliente01: "O--------"
    Servidor ->> Cliente02: "O--------"
    Servidor ->> Cliente02: "X"
    Cliente02 ->> Servidor: "4"
    Servidor ->> Cliente02: "1"
    Servidor ->> Cliente01: "O--X-----"
    Servidor ->> Cliente02: "O--X-----"
    Servidor ->> Cliente01: "O"
    Cliente01 ->> Servidor: "2"
    Servidor ->> Cliente01: "1"
    Servidor ->> Cliente01: "OO-X-----"
    Servidor ->> Cliente02: "OO-X-----"
    Servidor ->> Cliente02: "X"
    Cliente02 ->> Servidor: "5"
    Servidor ->> Cliente02: "1"
    Servidor ->> Cliente01: "OO-XX----"
    Servidor ->> Cliente02: "OO-XX----"
    Servidor ->> Cliente01: "O"
    Cliente01 ->> Servidor: "3"
    Servidor ->> Cliente01: "1"
    Servidor ->> Cliente01: "OOOXX----1"
    Servidor ->> Cliente02: "OOOXX----1"
    
```