# Avaliação de Programação de Jogos em Rede
Nesta avaliação, você deverá implementar um cliente em C/C++ que interaja com o ``server.py``.

O cliente deverá implementar o seguinte fluxo de comunicação e ações:

```mermaid
graph TD;
    I(Início) --> C(Conecta ao Servidor)
    C --> R(Recebe Peça que jogará)
    R --> V(Recebe Situação + Tabuleiro)
    V --> F{É Fim de Jogo?}
    F -- Sim --> T(Apresenta Resultado e Termina)
    F -- Não --> J{Vez do Jogador?}
    J -- Sim --> L(Lê jogada e envia para o servidor)
    L --> V
    J -- Não --> V

```

## Configurações do servidor:

* O Servidor estará executando na porta 8080;

## Formato da mensagem

As mensagens do servidor seguirão o seguinte formato:
* ``"O---------"``: É vez do jogador ``O`` jogar e o tabuleiro possui a configuração ``---------``;
* ``"XO-X------"``: É vez do jogador ``X`` jogar e o tabuleiro possui a configuração ``O-X------``;
* ``"VOOOOXX----"``: Vitória do Jogador ``O`` e o tabuleiro e o tabuleiro possui a configuração ``OOOXX----``;
* ``"VXOXOOXX-XO"``: Vitória do Jogador ``X`` e o tabuleiro e o tabuleiro possui a configuração ``OXOOXX-XO``;
* ``"EXOOXXOOXX"``: O Jogo terminou empatado e o tabuleiro possui a seguinte configuração ``XOOXXOOXX``;