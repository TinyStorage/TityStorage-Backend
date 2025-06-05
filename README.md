# 📦 TinyStorage Backend

[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**TinyStorage** — это backend-сервис для управления учётом физических предметов через сканирование штрихкодов. Проект поддерживает роли, авторизацию, административную панель и REST API.

---

## 🚀 Возможности

- 📇 Учёт предметов по штрихкодам
- 🔐 Авторизация через OAuth 2.0
- 🧾 Отметка предмета как выданного/возвращённого
- 🧑‍💻 Административные команды: регистрация и списание
- 📱 Поддержка мобильного клиента

---

## 🛠️ Технологии

- **ASP.NET Core 8**
- **Entity Framework Core** + PostgreSQL
- **OAuth2** авторизация
- **Docker + Docker Compose**
- **Swagger (Swashbuckle)** для документации

---

## 🧪 Локальный запуск

```bash
# Клонировать репозиторий
$ git clone https://github.com/TinyStorage/TityStorage-Backend.git
$ cd TityStorage-Backend

# Собрать и запустить через Docker Compose
$ docker-compose up --build
```

По умолчанию backend будет доступен по адресу: `http://localhost:5000`

Swagger UI: `http://localhost:5000/swagger`

---

## 🔐 Авторизация

Backend использует OAuth2 и предполагает наличие внешнего identity-провайдера (например, Keycloak).

Для доступа к защищённым маршрутам необходимо передавать `Bearer`-токен в заголовке запроса:

```http
Authorization: Bearer <access_token>
```

---

## 📁 Структура проекта

```text
📦 TityStorage-Backend
├── src/                # Основной код приложения
│   ├── Api/            # Точка входа (Controllers, Program.cs)
│   ├── Application/    # Бизнес-логика (CQRS, Use Cases)
│   ├── Domain/         # Доменные модели и сущности
│   ├── Infrastructure/ # Репозитории, миграции, авторизация и т.п.
│   └── Common/         # Общие утилиты и базовые классы
├── docker-compose.yml # Контейнеризация
└── README.md
```

---

## ✅ Примеры API

### 📌 Отметить предмет как выданный

`POST /v1/items/{id}/is-taken`

```json
{
  "isTaken": true
}
```

---

## 🤝 Контрибьютинг

1. Сделайте форк репозитория
2. Создайте свою ветку (`git checkout -b feature/foo`)
3. Сделайте коммиты (`git commit -am 'Add foo'`)
4. Отправьте изменения (`git push origin feature/foo`)
5. Создайте Pull Request

---

## 📄 Лицензия

Проект распространяется под лицензией [MIT](LICENSE).

---

_Проект создан в рамках инициативы по упрощению учёта предметов с помощью мобильного сканирования._
