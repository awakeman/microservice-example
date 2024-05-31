# Microservices Example

## Objective

The objective of this project is to demonstrate a simple microservice architecture using a routing gateway,
a message bus for asynchronous processing of requests and SignalR for notifications between microservices. 

## Running

To run the project simply run the following command in the project root.

```
docker-compose up
```

This will bring up 5 containers:
- `gateway`       - an ocelot gateway microservice that handles routing within
                    the system, listens on `localhost:8000` on the host. 
- `users.api`     - a microservice managing user model objects
- `settings.api`  - a microservice managing settings model objects
- `signalr.srv`   - a microservice running a SignalR Hub, used for notifications within the system
- `rabbit`        - an instance of rabbitmq, the message broker

## Tests

### Unit Tests

Unit tests for the project can be run with:

```
dotnet test
```


## API

### Special headers

#### `X-Async`

Any value in this header will cause `POST` requests to the API to respond immediately
with `202 Accepted` and put the model on a queue for async processing

#### `X-Tenant` (required)

Required header value, must be a 6 character alphanumeric string

### POST `/users/`

Saves the User Model sent in the body to a Json file named after the `Name` property,
in a directory named after the value of the `X-Tenant` header.

These requests will be handled by the `users.api` microservice.

### POST `/settings/`

Saves the Settings Model sent in the body to a Json file `settings.json`, in a directory named after the `X-Tenant` header.

These requests will be handled by the `settings.api` microservice.


## Follow-up

### Authentication

### High availability

### Logging and monitoring

### Other Improvements