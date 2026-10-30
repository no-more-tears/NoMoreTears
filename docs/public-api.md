## Public API

Централізований опис усіх ендпоінтів нашої системи, доступних для інтеграції зовнішніх систем. На усі ендпоінти накладено глобальний rate limit, крім того є деякі, для яких вказано особливі параметри ліміту.

<div align="center">
  <h3>Happy hacking!</h3>
</div>

### Загальні відомості

- **Content-Type:** `application/json`
- **Авторизація:** `Authorization: Bearer <accessToken>`. Ендпоінти з доступом Anonymous токена не потребують.
- **Токени:** access-token живе 15 хвилин, refresh-token живе 7 днів і змінюється при кожному оновленні.
- **Формати:** ідентифікатори: UUID, дати: ISO 8601 (`2026-10-29T18:08:06+00:00`).
- Успішна відповідь повертається зі статусом `200 OK`.

### Загальний формат відповіді при помилці

```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
  "title": "Validation error",
  "status": 400,
  "detail": "Field 'Identifier': Email or username is required.",
  "instance": "POST /api/identity/login",
  "traceId": "00-c055f07b4facc94fa73330899dc3a9a7-d92195cff61217cf-00",
  "requestId": "0HNPM21UG6MQT:00000001"
}
```
### Endpoints

| Модуль | Документація |
|--|--|
| Identity | [Identity/Endpoints/README.md](https://github.com/no-more-tears/NoMoreTears/blob/main/src/Modules/Identity/HostSide/Endpoints/README.md) |