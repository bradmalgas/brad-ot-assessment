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

