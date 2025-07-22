# dotnet8-opentelemetry

- ASP.NET Core Web API ile basit weather forecast servisi  
- OpenTelemetry ile distributed tracing ve metrik toplama  
- Jaeger, Zipkin trace export, Prometheus metrik export  
- Prometheus metrik endpoint (default `/metrics`)  

## Projeyi Çalıştırma


```
docker run --rm --name jaeger \
  -p 16686:16686 \
  -p 4317:4317 \
  -p 4318:4318 \
  -p 5778:5778 \
  -p 9411:9411 \
  cr.jaegertracing.io/jaegertracing/jaeger:2.8.0

docker run -d --name zipkin \
  -p 9411:9411 \
  openzipkin/zipkin:latest

```
### 1. Projeyi klonla

```
git clone https://github.com/kullaniciadi/dotnet8-opentelemetry.git
cd dotnet8-opentelemetry
```

JAEGER UI: http://localhost:16686

ZIPKIN UI: http://localhost:9411
