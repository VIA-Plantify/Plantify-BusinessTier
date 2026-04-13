# Plantify-BusinessTier

## Dev Container for this project

## IMPORTANT, gRPC Server should be running before WebAPI

### [Dev container setup](https://github.com/VIA-Plantify/Plantify-BusinessTier/blob/development/setup_dev.md)


<h3>From native machine</h3>
```
jdbc:postgresql://localhost:55432/plantify?password=plantifydev&user=dev 
```
<h3>From container</h3>

```
jdbc:postgresql://host.docker.internal:55432/plantify?user=dev&password=plantifydev
```

<h3> In production containers</h3>>

```
jdbc:postgresql://postgres:5432/plantify?password=plantifydev&user=dev
```