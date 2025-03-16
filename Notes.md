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
│   │   ├── CasinoWagerEventDTO.cs
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

## Part 2: Database and Entity Framework\*\*

My initial step was designing the database, ensuring it aligned with my entities. This naturally led to a code-first Entity Framework (EF) migration. I began by adding EF Core to my shared project and creating the `CasinoWagersDbContext`. This was a deliberate choice to keep database operations separate, reinforcing the domain-driven design principles I aimed for.

However, I quickly encountered a challenge: where to house my EF migrations. Since migrations require an executable project, my shared library was unsuitable. I debated between the consumer and API projects, ultimately choosing the consumer, despite it being a background service. This unconventional setup required me to explicitly tell EF Core which was my startup project and where the context was located. I used commands like `dotnet ef migrations add InitialCreate --startup-project OT.Assessment.Consumer --context OT.Assessment.Shared.Data.Implementation.CasinoWagersDbContext` and `dotnet ef database update` to get everything running.

Once EF Core was configured, I ran the initial migration, resulting in a database schema that reflected my entities and their indexes. The beauty of code-first workflows! Next, I created repositories to manage database interactions, which I planned to inject into my API and consumer projects. I defined three key methods: `AddWagerAsync`, `GetTopSpendersAsync`, and `GetWagersByPlayerAsync`, each serving specific functionalities for my consumer and API.

The real challenge was designing efficient database interactions. `AddWagerAsync` initially seemed straightforward, but a foreign key constraint forced me to check for player existence. This led to creating a separate `PlayersRepository` and implementing player creation logic. `GetWagersByPlayerAsync` presented a pagination challenge, prompting me to use offset pagination and raising concerns about performance with large datasets. I knew I needed to optimize this query further.

`GetTopSpendersAsync` proved the most complex. After numerous attempts to craft an efficient query, I decided to introduce a `TotalSpend` column in the `Players` table, updated via a SQL trigger upon wager insertion. This database-driven approach aimed to minimize API runtime overhead. However, this introduced a new hurdle: EF Core’s migration system. The trigger, being a database-level construct, needed to be incorporated into the migration itself. This was achieved through manual migration editing, but led to a `DbUpdateException` due to conflicts between the SQL trigger and EF Core's `OUTPUT` clause when auto-generating `created_at` timestamps. Ultimately, I moved the `created_at` generation to the repositories to resolve the conflict, a valuable lesson in understanding EF Core's limitations and adapting my designs accordingly.

## Part 3: Consumer, API, and Bonus Features\*\*

Reflecting on the project's structure, I realized I had initially approached it somewhat backward. However, with the finish line in sight, I focused on moving forward. I also acknowledged a previous mistake: housing player existence checks within the `CasinoWagerRepository`. This violated the single responsibility principle, mixing data access with business logic. To rectify this, I created a service in the consumer project. This led me to identify a second error: keeping publisher and consumer logic in the shared project. Recognizing the workflow complexity this caused, I moved the consumer logic to the consumer project and preemptively fixed similar issues in the API project.

With dedicated API and `CasinoWager` services in place, I proceeded with end-to-end testing. First, I started the API, verifying successful message publication to RabbitMQ. This worked as expected. Then, I ran the consumer, which immediately processed the message. It was time for the test project. However, an initial run resulted in a failure due to a rogue '/' character. After resolving this, I encountered CORS and URL configuration issues. Eventually, I got the tests running, only to face a new problem: duplicate wager IDs. My current setup didn't account for duplicates, leading to repeated attempts to add the same wager and triggering `DbUpdateException`. This highlighted a design oversight. To address this, I decided to treat adding a wager with an existing ID as an update, aligning with the concept of a betting system where bets can be modified.

This update approach led to concurrency conflicts. I encountered a message indicating that the database operation affected zero rows instead of the expected one, suggesting data modification or deletion since entities were loaded. After some research, I learned that I needed to make my database actions more atomic, achievable through transaction handling. I implemented transaction management, which resolved the concurrency issues.

Next, I focused on optimizing NBomber tests. However, I consistently encountered SSL connection errors. After adjusting the requests per second (RPS) to 125, I determined this was the application's maximum sustainable load. While significantly below the required 500 RPS, I believe horizontal scaling could still provide a viable solution. Despite the challenges and learning curves, I found the experience of working with new technologies rewarding and feel it has contributed to my growth as a developer.
