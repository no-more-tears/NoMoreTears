[![contributors](https://img.shields.io/github/contributors/no-more-tears/NoMoreTears)](https://github.com/no-more-tears/NoMoreTears/graphs/contributors)
[![docs](https://img.shields.io/badge/docs-available-blue)](https://github.com/no-more-tears/NoMoreTears/tree/main/docs)
[![ci](https://github.com/no-more-tears/NoMoreTears/actions/workflows/ci.yml/badge.svg?branch=main&event=push)](https://github.com/no-more-tears/NoMoreTears/actions/workflows/ci.yml)
[![repo size](https://img.shields.io/github/repo-size/no-more-tears/NoMoreTears)](https://github.com/no-more-tears/NoMoreTears)
[![open pull requests](https://img.shields.io/github/issues-pr/no-more-tears/NoMoreTears)](https://github.com/no-more-tears/NoMoreTears/pulls?q=is%3Apr+is%3Aopen)
[![closed pull requests](https://img.shields.io/github/issues-pr-closed/no-more-tears/NoMoreTears)](https://github.com/no-more-tears/NoMoreTears/pulls?q=is%3Apr+is%3Aclosed)
[![merged pull requests](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fapi.github.com%2Fsearch%2Fissues%3Fq%3Drepo%3Ano-more-tears%2FNoMoreTears%2Bis%3Apr%2Bis%3Amerged&query=%24.total_count&label=pull%20requests&suffix=%20merged&color=blueviolet)](https://github.com/no-more-tears/NoMoreTears/pulls?q=is%3Apr+is%3Amerged)

## NoMoreTears

Backend-система, яка представляє платформу для пошуку, підбору та виклику верифікованих профільних спеціалістів (майстрів) для вирішення побутових і технічних проблем клієнтів: ремонт техніки, сантехніка, електрика, клінінг і т.п. Frontend-система доступна за цим посиланням: [NoMoreTears-frontend](https://github.com/no-more-tears/NoMoreTears-frontend)

### 1. Технічний стек

<div align="center">
  <table>
    <tr>
      <td align="center" valign="middle"><strong>Використані<br>технології</strong></td>
      <td align="center" valign="top"><img src="docs/assets/logos/dotnet.svg" height="42" alt=".NET 10"/><br><sub>.NET 10</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/efcore.svg" height="42" alt="Entity Framework Core"/><br><sub>Entity Framework<br>Core</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/mediatr.svg" height="42" alt="MediatR"/><br><sub>MediatR</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/masstransit.svg" height="42" alt="MassTransit"/><br><sub>MassTransit</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/postgres.svg" height="42" alt="PostgreSQL"/><br><sub>PostgreSQL</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/docker.svg" height="42" alt="Docker"/><br><sub>Docker</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/redis.svg" height="42" alt="Redis"/><br><sub>Redis</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/rabbitmq.svg" height="42" alt="RabbitMQ"/><br><sub>RabbitMQ</sub></td>
      <td align="center" valign="top"><img src="docs/assets/logos/minio.svg" height="42" alt="MinIO"/><br><sub>MinIO</sub></td>
    </tr>
  </table>
</div>

### 2. Використані архітектурні підходи і паттерни

| Підхід / паттерн | Призначення |
|--|--|
| Modular Monolith | Єдиний деплой із чіткими межами між модулями. |
| Domain-Driven Design | Моделювання системи таким чином, що вся складність розміщена в єдиному місці. |
| Event-Driven Architecture | Взаємодія модулів через контракти у вигляді подій. |
| Clean Architecture | Separation of Concerns в горизонтальному вигляді. |
| Outbox/Inbox | Надійна доставка та ідемпотентна обробка повідомлень. |

### 3. Навігація в документації

| Область | Документація |
|--|--|
| Public API | [docs/public-api.md](https://github.com/no-more-tears/NoMoreTears/blob/main/docs/public-api.md) |
| Domain | [docs/domain-driven-design.md](https://github.com/no-more-tears/NoMoreTears/blob/main/docs/domain-driven-design.md) |
| Architecture | [docs/architecture.md](https://github.com/no-more-tears/NoMoreTears/blob/main/docs/architecture.md) |

### 4. Як запустити систему

TODO

### 5. Ліцензія

Проект ліцензовано під [MIT License](https://github.com/no-more-tears/NoMoreTears/blob/main/LICENSE).