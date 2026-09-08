.PHONY: start-rabbitmq
start-rabbitmq:
	docker run --detach --hostname rabbitmq --name rabbitmq \
		--env RABBITMQ_DEFAULT_USER=rabbitmq \
		--env RABBITMQ_DEFAULT_PASS=rabbitmq \
		--publish 15672:15672 \
		--publish 5672:5672 \
		-v rabbitmq-data:/var/lib/rabbitmq \
		rabbitmq:3.13-management-alpine

.PHONY: start-db
start-db:
	docker run -d --hostname db --name db \
		-e POSTGRES_DB=db \
		-e POSTGRES_USER=postgres \
		-e POSTGRES_PASSWORD=postgres \
		--publish 5436:5432 \
		-v db-data:/var/lib/postgresql \
		postgres:19beta1

.PHONY: start-services
start-services: start-db start-rabbitmq

.PHONY: stop-db
stop-db:
	docker stop db && docker rm db

.PHONY: stop-rabbitmq
stop-rabbitmq:
	docker stop rabbitmq && docker rm rabbitmq

.PHONY: stop-services
stop-services: stop-db stop-rabbitmq

.PHONY: clean
clean: stop-services
	docker volume rm db-data || true
	docker volume rm rabbitmq-data || true