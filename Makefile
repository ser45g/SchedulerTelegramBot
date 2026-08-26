start-rabbitmq:
	docker run --detach --hostname rabbitmq --name rabbitmq\
    --env RABBITMQ_DEFAULT_USER=rabbitmq \
    --env RABBITMQ_DEFAULT_PASS=rabbitmq \
    --publish 15672:15672 \
    --publish 5672:5672 \
     rabbitmq:3.13-management-alpine
