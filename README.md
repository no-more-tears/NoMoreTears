## NoMoreTears

Backend-система, яка представляє платформу для пошуку, підбору та виклику верифікованих профільних спеціалістів (майстрів) для вирішення побутових і технічних проблем клієнтів: ремонт техніки, сантехніка, електрика, клінінг і т.п. Frontend-система доступна за цим посиланням: [NoMoreTears-frontend](https://github.com/no-more-tears/NoMoreTears-frontend)

### 1. Технічний стек

<div align="center">
  <table>
    <tr>
      <td align="center" valign="middle"><strong>Використані<br>технології</strong></td>
      <td align="center" valign="top"><img src="docs/assets/logos/dotnet.svg" height="42"/><br><sub>.NET 10</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/efcore.svg" height="42"/><br><sub>Entity Framework<br>Core</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/postgres.svg" height="42"/><br><sub>PostgreSQL</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/docker.svg" height="42"/><br><sub>Docker</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/redis.svg" height="42"/><br><sub>Redis</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/rabbitmq.svg" height="42"/><br><sub>RabbitMQ</sub></td>
    </tr>
  </table>
</div>

### 2. Статус репозиторію

| Гілка | CI | Останній коміт | Коміти |
|--|--|--|--|
| main | [![main](https://github.com/no-more-tears/NoMoreTears/actions/workflows/ci.yml/badge.svg?branch=main&event=push)](https://github.com/no-more-tears/NoMoreTears/actions/workflows/ci.yml) | ![last commit](https://img.shields.io/github/last-commit/no-more-tears/NoMoreTears/main) | ![commits](https://img.shields.io/github/commit-activity/t/no-more-tears/NoMoreTears/main) |
| development | [![development](https://github.com/no-more-tears/NoMoreTears/actions/workflows/ci.yml/badge.svg?branch=development&event=push)](https://github.com/no-more-tears/NoMoreTears/actions/workflows/ci.yml) | ![last commit](https://img.shields.io/github/last-commit/no-more-tears/NoMoreTears/development) | ![commits](https://img.shields.io/github/commit-activity/t/no-more-tears/NoMoreTears/development) |

![open PRs](https://img.shields.io/github/issues-pr/no-more-tears/NoMoreTears) ![closed PRs](https://img.shields.io/github/issues-pr-closed/no-more-tears/NoMoreTears)

### 3. Використані архітектурні підходи і паттерни

| Підхід / паттерн | Призначення |
|--|--|
| Modular Monolith | Єдиний деплой із чіткими межами між модулями. |
| Domain-Driven Design | Моделювання системи таким чином, що вся складність розміщена в єдиному місці. |
| Event-Driven Architecture | Взаємодія модулів через контракти у вигляді подій. |
| Clean Architecture | Separation of Concerns в горизонтальному вигляді. |
| Outbox/Inbox | Надійна доставка та ідемпотентна обробка повідомлень. |

### 4. Навігація в документації

| Область | Документація |
|--|--|
| Public API | [docs/public-api.md](https://github.com/no-more-tears/NoMoreTears/blob/main/docs/public-api.md) |
| Domain | [docs/domain-driven-design.md](https://github.com/no-more-tears/NoMoreTears/blob/main/docs/domain-driven-design.md) |
| Architecture | [docs/architecture.md](https://github.com/no-more-tears/NoMoreTears/blob/main/docs/architecture.md) |

### 5. Як запустити систему

TODO

### 6. Ліцензія

Проект ліцензовано під [MIT License](https://github.com/no-more-tears/NoMoreTears/blob/main/LICENSE).