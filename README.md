# backend-development

## Название проекта

**РАЗРАБОТКА REST API ДЛЯ ПРЕДМЕТНОЙ ОБЛАСТИ «КЛИЕНТЫ» С ПОДДЕРЖКОЙ ОПЕРАЦИЙ GET, POST, PUT, PATCH, DELETE**

## Описание предметной области

Выбранная предметная область – Вариант №7 «Клиенты».
В ней основной сущностью является сущность «Client» со следующими полями:

- id,
- fullName,
- phone,
- email,
- discount,
- verified.

Дополнительно в серверном приложении имеется сущность «User», представляющая собой пользователей, имеющими доступ к сущностям «Client».
Поля сущности «User» представлены в списке ниже:

- id,
- login,
- hashPassword.

Так как в серверном приложении используется авторизация с помощью JWT-токенов, дополнительно реализован механизм «Refresh Token», что позволяет пользователю обновить JWT-токен для дальнейшего использования приложения.
Поля сущности «RefreshToken» представлены в списке ниже:

- id,
- user_id (foreign key сущности «User»),
- token,
- expires_at,
- is_revoked,
- created_at,
- replaced_by_token.

## Стек технологий

- Язык программирования: C#
- .NET 9.0 - Модульная платформа для разработки ПО
- ASP .NET Core - Фреймворк для разработки веб-приложений
- PostgreSQL - Система управления базами данных

## Как запустить проект

Для того чтобы запустить проект, на Вашем ПК должен быть установлен пакет .NET SDK, поддерживающий версию .NET 9.0.
Для запуска проекта необходимо перейти в директорию проекта и выполнить команду в любом удобном для Вас терминале: `dotnet run`.

Также перед запуском приложения можно осуществить сборку проекта с помощью команды `dotnet build`.

В случае, если Вам необходимо поменять порт, на котором будет запускаться веб-приложение, Вам необходимо открыть файл `launchSettings.json` в папке `Properties` и изменить порт в поле `applicationUrl`.

## Описание маршрутов API

### Документация Swagger

Маршруты API можно просмотреть с помощью Swagger UI, предварительно запустив проект и перейти по пути `http://localhost:5279/swagger`.

### Текстовое описание

Доступ к ручкам осуществляется благодаря API-контроллерам, построенным вокруг конкретной задачи/сущности. В приложении описаны 3 контроллера:

- AuthController (путь: “/api/auth”),
- UserController (путь: “/api/user”),
- ClientController (путь: “/api/client”).

#### AuthController

У AuthController имеется 3 ручки:

- Login (POST “/api/auth/login”),
- Logout (POST “/api/auth/logout”),
- Refresh (POST “/api/auth/refresh”).

#### UserController

У UserController имеется 7 ручек:

- GetAll (GET “/api/user”),
- GetById (GET “/api/user/{id}”),
- GetByLogin (GET “/api/user/by-login?login={login}”),
- Add (POST “/api/user”),
- Update (PUT “/api/user/{id}),
- PartialUpdate (PATCH “/api/user/{id}),
- Delete (DELETE “/api/user/{id}).

#### ClientController

У ClientController имеется 7 ручек:

- GetAll (GET “/api/client”),
- GetById (GET “/api/client/{id}”),
- GetByEmail (GET “/api/client/by-email?email={email}”),
- Add (POST “/api/client”),
- Update (PUT “/api/client/{id}),
- PartialUpdate (PATCH “/api/client/{id}),
- Delete (DELETE “/api/client/{id}).

## Примеры запросов и ответов

### Документация Swagger

Примеры запросов и ответов можно просмотреть с помощью Swagger UI во вкладке конкретного метода, предварительно запустив проект и перейдя по пути `http://localhost:5279/swagger`.

## Примеры ошибок

Примеры ошибок при выполнении запросов можно просмотреть с помощью Swagger UI во вкладке конкретного метода, в разделе **"Responses"**, предварительно запустив проект и перейдя по пути `http://localhost:5279/swagger`.

## Описание структуры проекта

Описание структуры проекта можно просмотреть в [репозитории GitHub](https://github.com/Mordahaoff/backend-development/tree/dev).
