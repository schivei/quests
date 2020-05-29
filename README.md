# Quests

##### Clonar o projeto

git clone https://github.com/schivei/quests.git

##### Para executar a aplicação:

Instalar o SDK do [dotnet core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

Abrir dois terminais de comando (linux, windows ou macos)

Em um dos terminais, acesse a pasta do projeto `quests/src/Quests.Api` e execute:
```
dotnet run
```

Executar a Api pela primeira vez poderá levar algum tempo, já que ele irá baixar os executáveis do banco de dados ([dgraph](https://dgraph.io)).

Se estiver executando em windows, dois outros terminais serão abertos e executados, é a inicialização do dgraph.

Quando a aplicação estiver pronta para executar, a seguinte mensagem aparecerá no terminal e o navegador principal se abrirá exibindo a página de documentação da API em swagger:
```
Successfully registered URL "http://localhost:54304/" for site "Quests.Api" application "/"
Successfully registered URL "https://localhost:44394/" for site "Quests.Api" application "/"
Registration completed for site "Quests.Api"
```

Em outro terminal, acesse a pasta do projeto `quests/src/Quests.App` e execute:
```
dotnet run
```

Esse comando dará início à aplicação de front-end e abrirá o navegador na página principal.

Na página de login, basta entrar com um usuário válido `^([a-z][a-z0-9]+)(\.[a-z0-9]+)?$`, exemplo: `meu.usuario`.

##### Para executar os testes

Acesse a pasta onde foi feito o clone e execute:
```
dotnet test
```

#### Atenção: **Certifique-se de não estar executando os projetos Quest.Api e Quest.App, para evitar conflitos com o dgraph, uma vez que os testes exegem um banco limpo e inicializam o banco em um pasta diferente.**
