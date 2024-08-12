# Avaliação 01

## SharedMemoryServer
O projeto [SharedMemoryServer](./SharedMemoryServer/SharedMemoryServer/Program.cs) apresenta o código do Servidor que
utiliza memória compartilhada para interação entre as Threads.

O Fluxograma abaixo apresenta o comportamento do Código

```mermaid
flowchart TD    
    A[Iniciar Servidor] --> B[Escutar Conexões]
    B --> C{Cliente Conectado?}
    C -- Não --> B
    C -- Sim --> D[Aceitar Conexão]
    D --> E[Iniciar Nova Thread para Cliente]
    D --> B
    subgraph HandleClient
        E --> F[Enviar Menu ao Cliente]
        F --> G[Cliente Envia Escolha]
        G --> H{Escolha do Cliente}
        H -- 1: Incrementar --> I[Incrementar Número]
        H -- 2: Decrementar --> J[Decrementar Número]
        H -- 3: Exibir Número --> K[Exibir Número]
        H -- 0: Sair --> L[Encerrar Conexão]
        H -- Inválido --> M[Enviar Mensagem de Opção Inválida]
        I --> F[Enviar Menu ao Cliente]
        J --> F[Enviar Menu ao Cliente]
        K --> F[Enviar Menu ao Cliente]
        M --> F[Enviar Menu ao Cliente]
        L --> N[Encerrar Thread]
    end
```

## Tarefas:

1. Escreva o fluxograma para de um cliente que seja capaz de se comunicar com esse servidor.
2. Implemente o código do cliente descrito no seu fluxograma.
3. Escreva um relatório explicando, em detalhes, o funcionamento dos blocos de código que você utilizou.

## Avaliação:

- Fluxograma;
- Código do Cliente;
- Interação entre o Cliente e o Servidor;
- Relatório;
- Explicação/Apresentação.