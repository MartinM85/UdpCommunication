# UdpCommunication
Simple UDP listener and receivers deployed into the Docker containers

## UDP listener
UDP listener is using `UdpClient` to listen for incoming datagram packets on specific local port.

### Publish

Publish builds the project in `Release` configuration for target framework `.net 7` and `linux x64` runtime.

Binaries are published to `C:\Temp\UdpCommunication\UdpListener`. Change the path based on your environment and needs.

### Docker compose

```YML
version: "3"
services:
  udplistener:
    build:
      context: .
      dockerfile: DockerfileUdpListener
    volumes:
      - C:\Temp\UdpCommunication\UdpListener:/opt/mm/udplistener
    networks:
      static-network:
        ipv4_address: 172.20.0.4
networks:
  static-network:
    ipam:
      config:
        - subnet: 172.20.0.0/16
```

Compose file maps the local path `C:\Temp\UdpCommunication\UdpListener` to path `/opt/mm/udplistener` in the docker container.

If you changed the output path for publish you need to change the volume path in compose file.

### Docker file

```YML
FROM debian:latest

WORKDIR /opt/mm/udplistener
#Add missing env property
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENTRYPOINT ["./UdpListener","42123"]
```

Docker file contains only entrypoint with execution of `UdpListener` from mapped folder with one parameter which specifies the local port for listening on upcoming datagrams.

### Docker command

To build docker container and start it run the following commands

```BATCH
docker-compose -f <path>\docker-compose-listener.yml build
docker-compose -f <path>\docker-compose-listener.yml up
```

## UDP sender
UDP sender sends datagrams to the specified endpoint.

### Publish

Publish builds the project in `Release` configuration for target framework `.net 7` and `linux x64` runtime.

Binaries are published to `C:\Temp\UdpCommunication\UdpSender`. Change the path based on your environment and needs.

### Docker compose

```YML
version: "3"
services:
  udpsender1:
    build:
      context: .
      dockerfile: DockerfileUdpSender
    volumes:
      - C:\Temp\UdpCommunication\UdpSender:/opt/mm/udpsender
    networks:
      static-network:
        ipv4_address: 172.20.0.2
  udpsender2:
    build:
      context: .
      dockerfile: DockerfileUdpSender
    volumes:
      - C:\Temp\UdpCommunication\UdpSender:/opt/mm/udpsender
    networks:
      static-network:
        ipv4_address: 172.20.0.3
networks:
  static-network:
    ipam:
      config:
        - subnet: 172.20.0.0/16
 ```

 Compose file maps the local path `C:\Temp\UdpCommunication\UdpSender` to path `/opt/mm/udpsender` in the docker container.

If you changed the output path for publish you need to change the volume path in compose file.

### Docker file

```YML
FROM debian:latest

WORKDIR /opt/mm/udpsender
#Add missing env property
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
#Parameters: ip of udp listener and its port
ENTRYPOINT ["./UdpSender","172.20.0.4","42123"]
```

Docker file contains only entrypoint with execution of `UdpSender` from mapped folder with two parameters. First parameter specifies the remote endpoint IP address and the second one specifies port.

### Docker command

To build docker container and start it run the following commands

```BATCH
docker-compose -f <path>\docker-compose-sender.yml build
docker-compose -f <path>\docker-compose-sender.yml up
```