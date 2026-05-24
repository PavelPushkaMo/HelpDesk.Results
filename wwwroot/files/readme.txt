===========================================
HELPDESK.RESULTS - ИНСТРУКЦИЯ
===========================================

Данный сервис демонстрирует работу Results API в ASP.NET Core.

Доступные маршруты:
- GET / - Главная страница
- GET /about/text - Текстовый ответ
- GET /about/content - Ответ с явным Content-Type
- GET /api/tickets - Список заявок
- GET /api/tickets/{id} - Заявка по ID
- POST /api/tickets/create?title=...&priority=... - Создание заявки
- GET /status/unauthorized - 401 Unauthorized
- GET /status/forbidden - 403 Forbidden
- GET /status/custom/418 - 418 I'm a teapot
- GET /redirect/old-tickets - Редирект на /api/tickets
- GET /redirect/ticket/{id} - Редирект на заявку
- GET /files/readme - Скачать этот файл
- GET /throw - Генерация исключения

