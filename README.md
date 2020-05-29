# Quests

##### Clonar o projeto

git clone https://github.com/schivei/quests.git

##### Para executar a aplica��o:

Instalar o SDK do [dotnet core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

Abrir dois terminais de comando (linux, windows ou macos)

Em um dos terminais, acesse a pasta do projeto `quests/src/Quests.Api` e execute:
```
dotnet run
```

Executar a Api pela primeira vez poder� levar algum tempo, j� que ele ir� baixar os execut�veis do banco de dados ([dgraph](https://dgraph.io)).

Se estiver executando em windows, dois outros terminais ser�o abertos e executados, � a inicializa��o do dgraph.

Quando a aplica��o estiver pronta para executar, a seguinte mensagem aparecer� no terminal e o navegador principal se abrir� exibindo a p�gina de documenta��o da API em swagger:
```
Successfully registered URL "http://localhost:54304/" for site "Quests.Api" application "/"
Successfully registered URL "https://localhost:44394/" for site "Quests.Api" application "/"
Registration completed for site "Quests.Api"
```

Em outro terminal, acesse a pasta do projeto `quests/src/Quests.App` e execute:
```
dotnet run
```

Esse comando dar� in�cio � aplica��o de front-end e abrir� o navegador na p�gina principal.

Na p�gina de login, basta entrar com um usu�rio v�lido `^([a-z][a-z0-9]+)(\.[a-z0-9]+)?$`, exemplo: `meu.usuario`.

##### Para executar os testes

Acesse a pasta onde foi feito o clone e execute:
```
dotnet test
```

#### Aten��o: **Certifique-se de n�o estar executando os projetos Quest.Api e Quest.App, para evitar conflitos com o dgraph, uma vez que os testes exegem um banco limpo e inicializam o banco em um pasta diferente.**
