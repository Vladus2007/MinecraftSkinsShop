# Minecraft Skins Shop

Кратко: это примерное приложение (backend на .NET, frontend на React + Vite) для магазина скинов Minecraft. README описывает архитектуру, логику расчёта цены по курсу BTC, переменные окружения и шаги запуска через Docker Compose.

**Содержание**
- Обзор
- Архитектура
- Логика расчёта цены (формула)
- Конфигурация / переменные окружения
- Запуск (Docker Compose)
- Локальный запуск (без Docker)
- Важные файлы
  

---

## Обзор
Сервис предоставляет API для получения списка скинов и покупки скинов. Цена финальная рассчитывается на основе базовой цены в USD и текущего курса BTC (используется внешний API). Для разработки включены mock-авторизация и seed-данные.

## Архитектура
- Backend: `MinecraftSkinsWebApi` (ASP.NET Core)
  - Presentation — точка входа (controllers, Program.cs)
  - Application — бизнес-логика (сервисы, PriceCalculator)
  - Infrastructure — подключения к БД, внешним API, репозитории
- Frontend: `frontend` (React + Vite)
- Сборка/развёртывание: `docker-compose.yaml` (Postgres, backend, frontend)

## Логика расчёта цены (формула)
Рассчёт цены реализован в `Application/Services/PriceCalculator.cs`.

Формула:

FinalPrice = BasePrice * (1 + (BTC - reference) / reference)

- `BasePrice` — базовая цена скина в USD
- `BTC` — текущая цена биткоина в USD, получаемая через `IGetRateService`
- `reference` — эталонная цена BTC, задана как `50000` (константа в коде)

Пример: при базовой цене 10 USD и текущем BTC = 60000 USD
multiplier = 1 + (60000 - 50000) / 50000 = 1 + 10000/50000 = 1 + 0.2 = 1.2
FinalPrice = 10 * 1.2 = 12.00 USD

Если получение курса не удалось (возвращается значение ≤ 0), используется защита и возвращается `BasePrice` округлённый до 2 знаков.

> Где изменить: в коде `PriceCalculator` можно поменять `referencePrice` и саму формулу.



## Запуск через Docker Compose
1. Постройте и запустите контейнеры:

```bash
docker compose build --no-cache
docker compose up -d
```

3. Проверить логи бэкенда:

```bash
docker compose logs -f backend
```

4. Фронтенд будет доступен на `http://localhost:8080` (в `docker-compose.yaml`), API на `http://localhost:5042`.

Если при старте видите ошибку `/usr/bin/env: 'bash\r'` — убедитесь, что в репозитории LF окончания строк для `.sh` файлов (есть `.gitattributes`) или пересоберите образ после нормализации.

## Локальный запуск (без Docker)
1. Backend (требуется .NET 7 SDK):
```bash
cd MinecraftSkinsWebApi/Presentation
dotnet restore
dotnet run --project ./Presentation.csproj
```

2. Frontend (Node.js 20+):
```bash
cd frontend
npm ci
npm run build
npm run preview    # или serve через nginx в Docker
```

## Важные файлы
- `Application/Services/PriceCalculator.cs` — формула расчёта цены
- `Infrastructure/BTCConnection/BTCConnection.cs` — клиент для получения курса BTC
- `Presentation/wait-for-postgres.sh` — скрипт ожидания Postgres (используется в Dockerfile)
- `docker-compose.yaml` — конфигурация сервисов
- `frontend/nginx/default.conf` — nginx конфигурация для frontend

## Полезные заметки
- Не храните реальные пароли и API-ключи в `appsettings.json` — используйте `.env` и `appsettings.json.example`.
- Для разработки включена `MockAuthoriseHandler` — проверяйте `Presentation/Program.cs` для схемы аутентификации.
- Seed-данные находятся в `Infrastructure/SeedData/SeedData.cs` и используются для инициализации БД при запуске с InMemory/SQLite.

---

Если хотите, я могу:
- добавить в README раздел с примерами запросов (curl) к API,
- добавить описание DTO и примеры ответов,
- или создать `appsettings.json.example` и убрать реальные секреты из `appsettings.json`.

