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

### Testing end-to-end

Currently there are not automated integration tests but you can test the functionality manually. Here are some useful example commands:

```
curl -v -d '{"Name":"test"}' \
     -H "Content-Type: application/json" \
     -H "X-Tenant: 123abc"\
     -H "X-Async: yes" \
     http://localhost:8000/users/ 
```

```
curl -v -d '{"Setting1": "value1", "Setting2": 2, "Setting3": "value3"}' \
     -H "Content-Type: application/json" \
     -H "X-Tenant: 123abc" \
     -H "X-Async: yes" \
     http://localhost:8000/settings/
```

Omit the `X-Async` header for the request to be processed synchronously.
Omit the `X-Tenant` header to test that it raises an error, similarly for testing the validation of that header's value.

## API

### Special headers

#### `X-Async`

Any value in this header will cause `POST` requests to the API to respond immediately
with `202 Accepted` and put the model on a queue for async processing

#### `X-Tenant` (required)

Required header value, must be a 6 character alphanumeric string and must only be supplied once.

### POST `/users/`

Saves the User Model sent in the body to a Json file named after the `Name` property,
in a directory named after the value of the `X-Tenant` header.

These requests will be handled by the `users.api` microservice.

### POST `/settings/`

Saves the Settings Model sent in the body to a Json file `settings.json`, in a directory named after the `X-Tenant` header.

These requests will be handled by the `settings.api` microservice.


## Follow-up

### Authentication and Authorization

Currently there is no Authentication between the microservices nor on the outward-facing API.
If I were to implement this I would take the following approach:

I would implement OAuth2 with either a dedicated authentication microservice or
using an external provider for this purpose, the authentication service would
provide a bearer token which is capable of being verified independently by the
downstream services without interaction with the authentication service,
allowing for performant authorisation.

In addition, one could also use an internal certifiacate authority, added to the
microservice containers' trust db, to issue X.509 to each of the microservices
for them to both verify one another via tls as well as encrypt their internal
communication, providing an additional layer of security from hostile actors.

### High availability

Since the services are minimal they can be triavially scaled horizontally by
adding more instances of each microservice, and configuring Ocelot to load
balance between downstream entpoints. They can be monitored for health by
utilising Asp.Net healthchecks so that if any instance becomes unresponsive, it
can be removed from the pool at the load balancer, deleted, recreated, and then
added back into the pool.

The RabbitMQ service can be clustered, also, granting it resiliance to sporatic
failure and heavy load conditions.

The gateway service could be scaled horizontally also, so long as there is
another load balancer in front of it, such as AWS Route53, or Nginx.
Alternatively it could be replaced by such a service, granting it resilliance
across availability zones.

If the entire solution were modified to run inside a cloud platform such as AWS,
Google Cloud Platform or Azure then the global scaling could be achieved easily.
This would be trivially achieved as each microservice is fully self-contained.

When scaling horizontally, one could also use Service Discovery such as
HashiCorp's Consul to locate the implementations of each microservice from the
load balancer/router, such that services could be provisioned and deprovisionned
dynamically, advertising themselves for use by other parts of the system through
such a system.

### Logging and monitoring

Further logging and monitoring could be added to the service to allow for
monitoring of performance issues or errors. Performance metrics could be added
using Prometheus and utilised by other sytems for visualisation and alerting.
Alerts could further be configured on certain log events such as errors,
provided that error paths in the code are augmented provide such logging.

Some basic logging has been added to the system already to demonstrate this.

### Other Improvements

Other improvements I would add to this example would be as follows.

#### Continuous Integration and Continuous Release

If this were being used in the real world I would write further Unit Tests, add
Integration Tests and run them as part of a CI pipeline. The CI pipeline would
check for code coverage before accepting the build as passing.

On the integration branch I would deploy (CR) from the pipeline and run
Deployment Verification Tests, before allowing the deployed containers to be
promoted to production. 

It would also be advantageous to write tests to benchmark the performance of the
system and run these as part of the CR pipeline. In the event of major
regression this might fail the build or prevent automatic promotion to
production environments.

#### Refactoring common code

There is code inside the Users and Settings service that is very similar, It
would be possible to refactor the common parts and make greated use of Generics
to remove redundant code. I believe it should be possible to have all of their
code be in a single project, just with build time or runtime customization of
the dependency injection that could be passed to the build or runtime
environment in the docker container, to provide both microservices with the same
project.

#### Generalise the async queue selection

The current AsyncRequestHandler middleware has magic strings to test the
upstream URL and pick which downstream queue to publish to. For a more
extensible and less error-prone implementation I believe it would be possible to
make use of Ocelot to get routing information about the current URL and then use
that information to select the appropriate queue, or to set metadata and publish
to an Exchange instead, binding the queues to the exchanges by filtering on that
metadata.