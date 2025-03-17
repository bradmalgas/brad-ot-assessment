# How to run application

1. Ensure you have met all the [prerequisites](https://github.com/bradmalgas/brad-ot-assessment/tree/main#pre-requisites)
2. Run the `OT.Assessment.App` project in a new terminal:

```
cd src/OT.Assessment.App
dotnet run
```

3. In a separate terminal run the `OT.Assessment.Consumer` project:

```
cd src/OT.Assessment.Consumer
dotnet run
```

3. With both projects running, you can begin the benchmark bu running the `OT.Assessment.Tester` project:

```
cd OT.Assessment.Tester
dotnet run
```

You will now see the results of the benchmark in the terminal.

# Key Components and Their Responsibilities

## `OT.Assessment.App` (API Application)

- Handles HTTP requests from clients.
- Validates and maps incoming data using AutoMapper.
- Uses `CasinoWagerApiService` to process business logic and publishing messages.
- Publishes messages to RabbitMQ using `CasinoWagerPublisher`.
- Uses dependency injection to manage dependencies.
- Uses `appsettings.json` for application configuration.

## `OT.Assessment.Consumer` (Consumer Application)

- Connects to RabbitMQ and consumes messages.
- Uses `CasinoWagerConsumer` to consume messages.
- Uses `CasinoWagerService` to process messages and store data in the database.
- Interacts with the database using `CasinoWagerRepository`.
- Uses dependency injection to manage dependencies.
- Uses `appsettings.json` for application configuration.

## `OT.Assessment.Shared` (Shared Library)

- Contains shared models (`CasinoWagerDto`, `CasinoWagerEntity`, etc.).
- Contains RabbitMQ messaging logic (`RabbitMqConnectionManager`, `RabbitMqChannel Factory`, etc.).
- Contains data access logic (`CasinoWagerDbContext`, `CasinoWagerRepository`, `PlayersRepository`).
- Provides a reusable set of components for both the API and the consumer.
- Uses `RabbitMqConfiguration` to manage RabbitMQ connection settings.

# Key Design Principles

- **Layered Architecture:** The API application follows a layered architecture (Controller, Service, Repository).
- **Separation of Concerns:** Each project and class has a specific responsibility.
- **Interface-Based Programming:** Interfaces are used to decouple components and improve testability.
- **Dependency Injection:** Dependency injection is used to manage dependencies and improve testability.
- **DTOs:** DTOs are used to transfer data between layers and applications.
- **RabbitMQ Messaging:** RabbitMQ is used for asynchronous message processing.
- **Configuration:** appsettings.json is used for application configuration.
- **Centralized Database Operations (Data Folder):**
  - Core database operations and the DbContext are encapsulated in the Shared/Data folder.
  - This promotes consistency, reusability, and abstraction from the underlying database.
- **Application-Specific Repositories:**
  - API and consumer applications have their own repositories to handle application-specific database logic.
  - This ensures separation of concerns, testability, maintainability, and encapsulation of business logic.
- **Repository Pattern:**
  - The repository pattern is used to provide an abstraction over the data access layer.
  - This makes the code more testable, maintainable, and flexible.
- **Domain-Driven Design (DDD):**
  - The project structure and data access approach align with DDD principles, separating the domain layer from the application layer.

# Dev Journey

## Part 1: Shared Library and RabbitMQ

The project kicked off smoothly enough. I downloaded the [project zip](https://osiristradingza.github.io/ot-backend-assessment/ot-backend-assessment.zip) and, boom, first commit! Lightning-fast dev speeds, I tell you. But seriously, the project was already well-structured - separate projects for different aspects of the solution, a solid foundation for a microservice architecture. My initial move? Scoping out any sensitive info that could slip into a commit, and removing any unnecessary files. I used a basic `.gitignore` template and appended some project specific files I wanted to exclude.

Next up, breaking down the requirements to identify the necessary classes. The `POST api/player/casinowager` endpoint was initially going to be my first entity class, perfect for database storage. But then I read a crucial line: “Not all the data is required.” Tricky! This meant a request that could handle a buffet of data, not all of which would be consumed (I got puns too). I decided to categorize my data classes into three distinct types:

- **Entities:** To mirror the database structure.
- **DTOs (Data Transfer Objects):** For data transfer between application layers.
- **DTMs (Data Transfer Models):** A new one for me! Specifically for derived data tailored for API endpoints. Always excited to learn and adapt!

Sample requests in hand, I designed these classes. Then, it was RabbitMQ time. Publisher and consumer—the application's backbone. I started with the classic [“Hello World”](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet) tutorial, laying the groundwork for basic publisher and consumer functionality. With that running, I expanded to include our model, complete with serialization/deserialization and the golden rule of interface-based programming. Console logs were my debugging allies, but I knew they’d be replaced with proper logging later.

Message acknowledgment was next. Adding explicit acks or nacks to ensure messages were redelivered if the consumer crashed. Durability was a bit of a hiccup. Turns out, RabbitMQ doesn’t like redefining existing queues with new parameters. The site suggested a new name, but I’m not one to back down. A quick queue deletion, and it was recreated with the right settings. I also made the messages persistent.

Then I learned about ‘exchanges’. These are basically message routers between producers and queues. Seemed like overkill for this assessment, but I saw the value of the concept in large-scale apps. Deciding on which exchange type to use was a puzzle: direct, topic, headers, or fanout? Fanout was a no-go; too much duplication. Headers were out too — very similar to direct, but with non-string routing keys, which we didn’t need. That left topic and direct. Scalability was the deciding factor. Even with one queue now, future expansion meant topic exchanges, with wildcards for flexible routing was a futureproof option.

With the exchange in place, optimization was next. I learned that reusing a single connection with multiple channels beats creating them for every message, especially under heavy load. A connection management class was the first optimization, ensuring a single instance is ever created. Next, a dedicated channel creation class, simplifying the publisher and consumer code. Trying to figure out the asynchronous connection in the constructor was another brain teaser, but calling `Wait()` on the async method did the trick. After refactoring and tweaking, here’s the current layout:

```
OT.Assessment/
├── OT.Assessment.App/             (API Application)
├── OT.Assessment.Consumer/        (Consumer Application)
├── OT.Assessment.Shared/          (Shared Library)
│   ├── DTMs/
│   │   ├── PlayerTopSpenderDTM.cs
│   ├── DTOs/
│   │   ├── CasinoWagerDTO.cs
│   │   ├── CasinoWagerEventDTM.cs
│   ├── Entities/
│   │   ├── CasinoWagerEntity.cs
│   │   ├── PlayerEntity.cs
│   ├── Messaging/
│   |   ├── Implementation
│   │   |   ├── CasinoWagerConsumer.cs
│   │   |   ├── CasinoWagerPublisher.cs
│   │   |   ├── RabbitMqChannelFactory.cs
│   │   |   ├── RabbitMqConnectionManager.cs
│   |   ├── Interfaces
│   │   |   ├── ICasinoWagerConsumer.cs
│   │   |   ├── ICasinoWagerPublisher.cs
│   │   |   ├── IRabbitMqChannelFactory.cs
│   │   |   ├── IRabbitMqConnectionManager.cs
│   │   ├── RabbitMqConfiguration.cs
│   ├── GlobalUsings.cs
│   ├── OT.Assessment.Shared.csproj
├── OT.Assessment.sln               (Solution file)
```

### Resources:

- [https://osiristradingza.github.io/ot-backend-assessment/ot-backend-assessment.zip](https://osiristradingza.github.io/ot-backend-assessment/ot-backend-assessment.zip)
- [https://www.rabbitmq.com/tutorials/tutorial-one-dotnet](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet)
- [​https://www.rabbitmq.com/tutorials/tutorial-two-dotnet#message-acknowledgment](​https://www.rabbitmq.com/tutorials/tutorial-two-dotnet#message-acknowledgment)
- [https://www.rabbitmq.com/tutorials/tutorial-two-dotnet#message-durability](https://www.rabbitmq.com/tutorials/tutorial-two-dotnet#message-durability)
- [https://www.rabbitmq.com/tutorials/tutorial-three-dotnet#exchanges](https://www.rabbitmq.com/tutorials/tutorial-three-dotnet#exchanges)
- [https://www.rabbitmq.com/tutorials/tutorial-three-dotnet#bindings](https://www.rabbitmq.com/tutorials/tutorial-three-dotnet#bindings)
- [https://www.rabbitmq.com/tutorials/tutorial-five-dotnet#putting-it-all-together](https://www.rabbitmq.com/tutorials/tutorial-five-dotnet#putting-it-all-together)

## Part 2: Database and Entity Framework\*\*

My initial step was designing the database, ensuring it aligned with my entities. This naturally led to a code-first Entity Framework (EF) migration. I began by adding EF Core to my shared project and creating the `CasinoWagersDbContext`. This was a deliberate choice to keep database operations separate, reinforcing the domain-driven design principles I aimed for.

However, I quickly encountered a challenge: where to house my EF migrations. Since migrations require an executable project, my shared library was unsuitable. I debated between the consumer and API projects, ultimately choosing the consumer, despite it being a background service. This unconventional setup required me to explicitly tell EF Core which was my startup project and where the context was located. I used commands like `dotnet ef migrations add InitialCreate --startup-project OT.Assessment.Consumer --context OT.Assessment.Shared.Data.Implementation.CasinoWagersDbContext` and `dotnet ef database update` to get everything running.

Once EF Core was configured, I ran the initial migration, resulting in a database schema that reflected my entities and their indexes. The beauty of code-first workflows! Next, I created repositories to manage database interactions, which I planned to inject into my API and consumer projects. I defined three key methods: `AddWagerAsync`, `GetTopSpendersAsync`, and `GetWagersByPlayerAsync`, each serving specific functionalities for my consumer and API.

The real challenge was designing efficient database interactions. `AddWagerAsync` initially seemed straightforward, but a foreign key constraint forced me to check for player existence. This led to creating a separate `PlayersRepository` and implementing player creation logic. `GetWagersByPlayerAsync` presented a pagination challenge, prompting me to use offset pagination and raising concerns about performance with large datasets. I knew I needed to optimize this query further.

`GetTopSpendersAsync` proved the most complex. After numerous attempts to craft an efficient query, I decided to introduce a `TotalSpend` column in the `Players` table, updated via a SQL trigger upon wager insertion. This database-driven approach aimed to minimize API runtime overhead. However, this introduced a new hurdle: EF Core’s migration system. The trigger, being a database-level construct, needed to be incorporated into the migration itself. This was achieved through manual migration editing, but led to a `DbUpdateException` due to conflicts between the SQL trigger and EF Core's `OUTPUT` clause when auto-generating `created_at` timestamps. Ultimately, I moved the `created_at` generation to the repositories to resolve the conflict, a valuable lesson in understanding EF Core's limitations and adapting my designs accordingly.

### Resources:

- [https://learn.microsoft.com/en-us/ef/core/modeling/indexes?tabs=fluent-api](https://learn.microsoft.com/en-us/ef/core/modeling/indexes?tabs=fluent-api)
- [https://learn.microsoft.com/en-us/ef/core/querying/pagination](https://learn.microsoft.com/en-us/ef/core/querying/pagination)
- [https://learn.microsoft.com/en-us/answers/questions/1191128/hasdefaultvaluesql(getutcdate())-valuegeneratedona](<https://learn.microsoft.com/en-us/answers/questions/1191128/hasdefaultvaluesql(getutcdate())-valuegeneratedona>)
- [https://learn.microsoft.com/en-us/ef/core/cli/dotnet\#common-options](https://learn.microsoft.com/en-us/ef/core/cli/dotnet#common-options)
- [https://learn.microsoft.com/en-us/ef/core/querying/pagination\#offset-pagination](https://learn.microsoft.com/en-us/ef/core/querying/pagination#offset-pagination)
- [https://stackoverflow.com/a/78185980](https://stackoverflow.com/a/78185980)
- [https://learn.microsoft.com/en-us/answers/questions/2112283/how-to-resolve-efcore-8-saving-to-table-with-trigg](https://learn.microsoft.com/en-us/answers/questions/2112283/how-to-resolve-efcore-8-saving-to-table-with-trigg)

## Part 3: Consumer, API, and Bonus Features\*\*

Reflecting on the project's structure, I realized I had initially approached it somewhat backward. However, with the finish line in sight, I focused on moving forward. I also acknowledged a previous mistake: housing player existence checks within the `CasinoWagerRepository`. This violated the single responsibility principle, mixing data access with business logic. To rectify this, I created a service in the consumer project. This led me to identify a second error: keeping publisher and consumer logic in the shared project. Recognizing the workflow complexity this caused, I moved the consumer logic to the consumer project and preemptively fixed similar issues in the API project.

With dedicated API and `CasinoWager` services in place, I proceeded with end-to-end testing. First, I started the API, verifying successful message publication to RabbitMQ. This worked as expected. Then, I ran the consumer, which immediately processed the message. It was time for the test project. However, an initial run resulted in a failure due to a rogue '/' character. After resolving this, I encountered CORS and URL configuration issues. Eventually, I got the tests running, only to face a new problem: duplicate wager IDs. My current setup didn't account for duplicates, leading to repeated attempts to add the same wager and triggering `DbUpdateException`. This highlighted a design oversight. To address this, I decided to treat adding a wager with an existing ID as an update, aligning with the concept of a betting system where bets can be modified.

This update approach led to concurrency conflicts. I encountered a message indicating that the database operation affected zero rows instead of the expected one, suggesting data modification or deletion since entities were loaded. After some research, I learned that I needed to make my database actions more atomic, achievable through transaction handling. I implemented transaction management, which resolved the concurrency issues.

Next, I focused on optimizing NBomber tests. However, I consistently encountered SSL connection errors. After adjusting the requests per second (RPS) to 125, I determined this was the application's maximum sustainable load. While significantly below the required 500 RPS, I believe horizontal scaling could still provide a viable solution. Despite the challenges and learning curves, I found the experience of working with new technologies rewarding and feel it has contributed to my growth as a developer.

---

As a final challenge, I decided to refactor the API for efficiency. Recognizing potential overhead in the original controller-based design, I created a new branch and reverted to a boilerplate API. I transitioned to a minimal API and established a performance baseline with empty endpoints. I ran the NBomb test as a benchmark and got the following results:

```
────────────────────────────────────────────────────────────────── scenario stats ───────────────────────────────────────────────────────────────────

scenario: hello_world_scenario
  - ok count: 7000
  - fail count: 0
  - all data: 0 MB
  - duration: 00:00:28

load simulations:
  - iterations_for_inject, rate: 500, interval: 00:00:02, iterations: 7000

┌─────────────────────────┬─────────────────────────────────────────────────────────┐
│                    step │ ok stats                                                │
├─────────────────────────┼─────────────────────────────────────────────────────────┤
│                    name │ global information                                      │
│           request count │ all = 7000, ok = 7000, RPS = 250                        │
│            latency (ms) │ min = 0.45, mean = 65.92, max = 509.92, StdDev = 104.45 │
│ latency percentile (ms) │ p50 = 21.38, p75 = 71.81, p95 = 324.86, p99 = 427.26    │
└─────────────────────────┴─────────────────────────────────────────────────────────┘
```

This was without any changes to the existing code, and all endpoints simply returned an empty string. This served as a good benchmark.

I updated the parameter type of our POST method from object to CasinoWagerEventDTO and then ran another test. I then obtained the following results:

```
────────────────────────────────────────────────── scenario stats ──────────────────────────────────────────────────

scenario: hello_world_scenario
  - ok count: 7000
  - fail count: 0
  - all data: 0 MB
  - duration: 00:00:28

load simulations:
  - iterations_for_inject, rate: 500, interval: 00:00:02, iterations: 7000

┌─────────────────────────┬─────────────────────────────────────────────────────┐
│                    step │ ok stats                                            │
├─────────────────────────┼─────────────────────────────────────────────────────┤
│                    name │ global information                                  │
│           request count │ all = 7000, ok = 7000, RPS = 250                    │
│            latency (ms) │ min = 0.25, mean = 0.91, max = 71.49, StdDev = 1.35 │
│ latency percentile (ms) │ p50 = 0.82, p75 = 0.99, p95 = 1.39, p99 = 2.27      │
└─────────────────────────┴─────────────────────────────────────────────────────┘
```

Surprisingly, my results were actually better. I have to assume the cold start may have impacted the initial results, but nonetheless, I am pleased because it did not get slower.

The next thing I did was introduce a casino wager request to represent the request body structure and reduce the number of properties in the actual CasinoWagerEventDTM that would be sent to the consumer. Instead of sending the entire request, I mapped only required values into the CasinoWagerEventDTM. This could further improve performance as we did not store any unnecessary values. The results of that are shown below:

```
────────────────────────────────────────────────── scenario stats ──────────────────────────────────────────────────

scenario: hello_world_scenario
  - ok count: 7000
  - fail count: 0
  - all data: 0 MB
  - duration: 00:00:28

load simulations:
  - iterations_for_inject, rate: 500, interval: 00:00:02, iterations: 7000

┌─────────────────────────┬───────────────────────────────────────────────────────┐
│                    step │ ok stats                                              │
├─────────────────────────┼───────────────────────────────────────────────────────┤
│                    name │ global information                                    │
│           request count │ all = 7000, ok = 7000, RPS = 250                      │
│            latency (ms) │ min = 2.06, mean = 4.34, max = 249.31, StdDev = 11.42 │
│ latency percentile (ms) │ p50 = 3.24, p75 = 3.97, p95 = 5.17, p99 = 9.27        │
└─────────────────────────┴───────────────────────────────────────────────────────┘
```

Still no failures — this seemed like a good approach. I also confirmed that all 7,000 requests were added to the RabbitMQ queue. Now, at this point, because we were using a minimal API, all the logic was contained in the Program.cs file. We could retain the benefits of a minimal API but reintroduce our service to handle publishing messages, handling mapping, and calling our repositories. This time, however, I used two separate services: one to handle database access via the repository and one to handle publishing to RabbitMQ.

After moving the publish logic into the `CasinoWagerPublishService`, I then ran another test to verify that the changes did not introduce any unnecessary overhead. The results were roughly the same:

```
────────────────────────────────────────────────────────────────────────────── scenario stats ──────────────────────────────────────────────────────────────────────────────

scenario: hello_world_scenario
  - ok count: 7000
  - fail count: 0
  - all data: 0 MB
  - duration: 00:00:28

load simulations:
  - iterations_for_inject, rate: 500, interval: 00:00:02, iterations: 7000

┌─────────────────────────┬───────────────────────────────────────────────────────┐
│                    step │ ok stats                                              │
├─────────────────────────┼───────────────────────────────────────────────────────┤
│                    name │ global information                                    │
│           request count │ all = 7000, ok = 7000, RPS = 250                      │
│            latency (ms) │ min = 2.08, mean = 4.31, max = 264.35, StdDev = 11.64 │
│ latency percentile (ms) │ p50 = 3.23, p75 = 3.93, p95 = 4.91, p99 = 8.25        │
└─────────────────────────┴───────────────────────────────────────────────────────┘
```

Lastly, I needed to add back the logic for the other API endpoints. These were housed in the `CasinoWagerApiService`. This was relatively straightforward as all the logic was in the repository already. One last test with all the pieces together:

```
────────────────────────────────────────────────────────────────────────────── scenario stats ──────────────────────────────────────────────────────────────────────────────

scenario: hello_world_scenario
  - ok count: 7000
  - fail count: 0
  - all data: 0 MB
  - duration: 00:00:28

load simulations:
  - iterations_for_inject, rate: 500, interval: 00:00:02, iterations: 7000

┌─────────────────────────┬─────────────────────────────────────────────────────┐
│                    step │ ok stats                                            │
├─────────────────────────┼─────────────────────────────────────────────────────┤
│                    name │ global information                                  │
│           request count │ all = 7000, ok = 7000, RPS = 250                    │
│            latency (ms) │ min = 2.2, mean = 3.69, max = 126.67, StdDev = 3.74 │
│ latency percentile (ms) │ p50 = 3.35, p75 = 3.98, p95 = 5.08, p99 = 6.84      │
└─────────────────────────┴─────────────────────────────────────────────────────┘
```

I could finally take a sigh of relief, as I finally met all the requirements.

### Resources:

- [https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api\#resolving-concurrency-conflicts](https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api#resolving-concurrency-conflicts)
- [https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.storage.idbcontexttransaction?view=efcore-8.0](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.storage.idbcontexttransaction?view=efcore-8.0)
- [https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.infrastructure.databasefacade?view=efcore-8.0](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.infrastructure.databasefacade?view=efcore-8.0)
- [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-9.0](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-9.0)
- [https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-9.0\&tabs=visual-studio](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-9.0&tabs=visual-studio)
