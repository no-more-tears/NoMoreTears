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

### 2. Використані архітектурні підходи і паттерни

| Підхід / паттерн | Призначення |
|------------------|-------------|
| Modular Monolith | Єдиний деплой із чіткими межами між модулями. |
| Domain-Driven Design | Моделювання системи таким чином, що вся складність розміщена в єдиному місці. |
| Event-Driven Architecture | Взаємодія модулів через контракти у вигляді подій. |
| Clean Architecture | Seperation of Concerns в горизонтальному вигляді. |
| Outbox/Inbox | Надійна доставка та ідемпотентна обробка повідомлень. |

### 3. Навігація в документації

TODO

### 4. Як запустити систему

TODO

### 5. Ліцензія

Проект ліцензовано під [MIT License](https://github.com/no-more-tears/NoMoreTears/blob/main/LICENSE).