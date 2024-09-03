# To-Do Web App

# Design

- Users [0-Many]
  - Projects [0-Many]
    - Tasks [0-Many]

## Project Architecture

Using Clean architecture

- Core layer (No dependencies)
    - Business logic services
    - Exception classes
    - Abstraction interfaces
    - Validators
    - Enums
    - Other domain-related classes
- Infrastructure layer (Depends on Core layer)
    - Implementations of abstraction defined in the Core layer
    - Communication with external services ie. databases, message brokers or third-party services
- Presentation layer (Depends on Infrastructure (and Core layer implicitly))
    - Handles interaction with the external world ie. Web API or Web App