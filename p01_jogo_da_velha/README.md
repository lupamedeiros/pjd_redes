# Projeto Jogo da Velha

Nesse projeto deverá ser desenvolvido o programa cliente, compatível com o servidor apresentado, 
para o jogo da velha.

Como material para avaliação deverá ser apresentado:
 
 - Relatório contendo:
   - Fluxograma para a aplicação cliente;
   - Descrição do código fonte correspondente a cada bloco do diagrama;
 - Repositório do GitHub com o código da aplicação desenvolvida.

## Servidor

### Fluxograma
```mermaid
graph TD
A[Iniciar Servidor] --> B[Escutar Conexões]
B --> C[Jogador Conectado]
C --> D{Sala de Jogo Disponível?}
D -- Não --> E[Criar Nova Sala de Jogo]
D -- Sim --> F[Adicionar Jogador à Sala Existente]
E --> F
F --> G{Sala Completa?}
G -- Não --> B
G -- Sim --> H[Iniciar Jogo na Sala]
H --> I{Jogador Atual?}
I -- Jogador 1 --> J[Enviar Tabuleiro e Pedido de Jogada]
I -- Jogador 2 --> J[Enviar Tabuleiro e Pedido de Jogada]
J --> K[Receber Jogada]
K --> L{Jogada Válida?}
L -- Não --> M[Enviar Mensagem de Erro]
L -- Sim --> N[Atualizar Tabuleiro]
N --> O{Vitória?}
O -- Sim --> P[Enviar Mensagem de Vitória]
O -- Não --> Q{Empate?}
Q -- Sim --> R[Enviar Mensagem de Empate]
Q -- Não --> S[Alternar Jogador]
S --> H
P --> T[Encerrar Jogo na Sala]
R --> T
T --> B
```