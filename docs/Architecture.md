# Architecture Ilon

## Vue d'ensemble

Ilon est construit selon une architecture **Modular Monolith** avec **Vertical Slices**, **Event Sourcing** et **CQRS**.

## Diagramme d'architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         Clients                                   │
├─────────────────┬──────────────────┬──────────────────────────────┤
│   Mobile App    │   Backoffice     │      External APIs           │
│   (MAUI)        │   (Blazor)       │      (Stripe, SMS)           │
└────────┬────────┴────────┬─────────┴──────────────────────────────┘
         │                 │
         │                 │
┌────────▼─────────────────▼────────────────────────────────────────┐
│                     Ilon.Api (ASP.NET Core 10)                     │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │                    Middleware Pipeline                        │ │
│  │  HTTPS │ CORS │ Auth │ Logging │ Health Checks               │ │
│  └──────────────────────────────────────────────────────────────┘ │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │                    Modules (Vertical Slices)                  │ │
│  │                                                               │ │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐          │ │
│  │  │  Identity   │  │  Listings   │  │   Search    │          │ │
│  │  │             │  │             │  │             │          │ │
│  │  │ • SendOTP   │  │ • Create    │  │ • Filters   │          │ │
│  │  │ • VerifyOTP │  │ • Update    │  │ • AI Query  │          │ │
│  │  │ • Profile   │  │ • Delete    │  │             │          │ │
│  │  └─────────────┘  └─────────────┘  └─────────────┘          │ │
│  │                                                               │ │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐          │ │
│  │  │  Requests   │  │  Messaging  │  │  Payments   │          │ │
│  │  └─────────────┘  └─────────────┘  └─────────────┘          │ │
│  │                                                               │ │
│  │  ┌─────────────┐                                             │ │
│  │  │ Moderation  │                                             │ │
│  │  └─────────────┘                                             │ │
│  └──────────────────────────────────────────────────────────────┘ │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │                    BuildingBlocks                             │ │
│  │  Primitives │ Security │ Observability │ Extensions           │ │
│  └──────────────────────────────────────────────────────────────┘ │
└────────────────────────────────┬───────────────────────────────────┘
                                 │
         ┌───────────────────────┼───────────────────────┐
         │                       │                       │
┌────────▼──────────┐  ┌────────▼────────┐  ┌──────────▼─────────┐
│   PostgreSQL      │  │   Redis         │  │  External Services │
│                   │  │                 │  │                    │
│ • Event Store     │  │ • OTP Cache     │  │ • SMS Provider     │
│ • Projections     │  │ • Sessions      │  │ • Stripe           │
│ • Outbox          │  │                 │  │ • Storage (S3)     │
└───────────────────┘  └─────────────────┘  └────────────────────┘
```

## Patterns Architecturaux

### 1. Vertical Slices

Chaque fonctionnalité est organisée en slice verticale complète :

```
Identity/Features/SendOtp/
├── SendOtpRequest.cs      # Request DTO
├── SendOtpHandler.cs      # Business logic
├── SendOtpValidator.cs    # Validation
└── SendOtpEndpoint.cs     # API endpoint
```

**Avantages** :
- Cohésion élevée
- Couplage faible
- Facile à tester
- Découverte rapide du code

### 2. Event Sourcing + CQRS

**Write Model** :
- Commandes modifient l'état
- Événements stockés dans l'Event Store
- Agrégats reconstituables depuis événements

**Read Model** :
- Projections optimisées pour les queries
- Dénormalisées si besoin
- Mises à jour par event handlers

```csharp
// Command (Write)
CreateListingCommand → CreateListingHandler → ListingCreatedEvent

// Query (Read)
GetListingsQuery → GetListingsHandler → ListingProjection
```

### 3. Outbox Pattern

Garantit la cohérence événement/données :

1. Transaction commit l'agrégat + événement dans outbox
2. Background worker publie événements
3. Event handlers mettent à jour projections

## Flux de données

### Exemple : Envoi d'OTP

```
┌──────────┐     POST /api/auth/send-otp      ┌────────────────┐
│  Client  │────────────────────────────────>  │   API          │
└──────────┘                                   │   Endpoint     │
                                               └───────┬────────┘
                                                       │
                                               ┌───────▼────────┐
                                               │   Validator    │
                                               │   (validate)   │
                                               └───────┬────────┘
                                                       │
                                               ┌───────▼────────┐
                                               │   Handler      │
                                               │  • Gen OTP     │
                                               │  • Store cache │
                                               │  • Log         │
                                               └───────┬────────┘
                                                       │
                  Response (200 OK)            ┌───────▼────────┐
┌──────────┐  <─────────────────────────────  │   Response     │
│  Client  │                                   └────────────────┘
└──────────┘
```

## Sécurité

### Authentification
- OTP par SMS (6 chiffres, expiration 5min)
- JWT tokens après vérification OTP
- Refresh tokens stockés en base

### Autorisation
- Role-based (User, Professional, Admin, Moderator)
- Claims-based pour permissions granulaires

### Protection
- Rate limiting (100 req/min par défaut)
- HTTPS obligatoire
- CORS configuré
- Input validation stricte
- Headers de sécurité (HSTS, etc.)

## Observabilité

### Logging
- **Serilog** structuré
- Niveaux : Debug, Info, Warning, Error, Fatal
- Enrichissement avec contexte (User, TraceId)
- Sinks : Console + File (+ Application Insights futur)

### Metrics (à venir)
- OpenTelemetry
- Prometheus + Grafana
- Métriques métier : Listings créés, OTP envoyés, etc.

### Tracing (à venir)
- Distributed tracing avec OpenTelemetry
- Jaeger ou Application Insights

### Health Checks
- `/health` : Liveness probe
- `/health/ready` : Readiness probe (+ DB check)

## Base de Données

### Event Store Schema

```sql
-- Event Stream
CREATE TABLE event_streams (
    stream_id UUID PRIMARY KEY,
    aggregate_type VARCHAR(255) NOT NULL,
    aggregate_id VARCHAR(255) NOT NULL,
    version INT NOT NULL
);

-- Events
CREATE TABLE events (
    event_id UUID PRIMARY KEY,
    stream_id UUID REFERENCES event_streams(stream_id),
    event_type VARCHAR(255) NOT NULL,
    event_data JSONB NOT NULL,
    metadata JSONB,
    version INT NOT NULL,
    timestamp TIMESTAMPTZ DEFAULT NOW()
);

-- Outbox
CREATE TABLE outbox (
    id UUID PRIMARY KEY,
    aggregate_id VARCHAR(255),
    event_type VARCHAR(255),
    event_data JSONB,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    processed_at TIMESTAMPTZ NULL
);
```

### Projections

```sql
-- User Profiles (Read Model)
CREATE TABLE user_profiles (
    id UUID PRIMARY KEY,
    phone_number VARCHAR(20) UNIQUE NOT NULL,
    display_name VARCHAR(100),
    email VARCHAR(255),
    role VARCHAR(50),
    created_at TIMESTAMPTZ,
    updated_at TIMESTAMPTZ
);

-- Listings (Read Model)
CREATE TABLE listings (
    id UUID PRIMARY KEY,
    title VARCHAR(255),
    description TEXT,
    price_amount DECIMAL(18, 2),
    price_currency VARCHAR(3),
    location_city VARCHAR(100),
    location_country VARCHAR(100),
    status VARCHAR(50),
    created_at TIMESTAMPTZ,
    updated_at TIMESTAMPTZ
);
```

## Déploiement

### Environnements
- **Dev** : Local + Docker Compose
- **Staging** : Azure App Service / AWS ECS
- **Production** : Azure / AWS avec haute disponibilité

### CI/CD Pipeline
```
┌─────────┐    ┌──────┐    ┌────────┐    ┌────────┐    ┌──────────┐
│  Commit │───>│ Build│───>│ Tests  │───>│ Docker │───>│  Deploy  │
└─────────┘    └──────┘    └────────┘    └────────┘    └──────────┘
                                │
                         ┌──────▼──────┐
                         │  Security   │
                         │  Scan       │
                         └─────────────┘
```

## Scaling Strategy

### Horizontal Scaling
- API : Stateless, peut scaler horizontalement
- Background workers : Un seul actif (leader election)

### Vertical Scaling
- PostgreSQL : Read replicas
- Redis : Cluster mode

### Caching
- **Memory Cache** : OTP (court terme)
- **Redis** : Sessions, résultats recherche
- **CDN** : Assets statiques (images, vidéos)

## Patterns de Résilience

- **Retry** : Tentatives multiples avec backoff exponentiel
- **Circuit Breaker** : Protection contre services défaillants
- **Timeout** : Limites de temps d'exécution
- **Bulkhead** : Isolation des ressources

## Technologies Clés

| Composant | Technologie | Rôle |
|-----------|-------------|------|
| API | ASP.NET Core 10 Minimal API | Backend REST |
| Mobile | .NET MAUI 10 + DevExpress | App multiplateforme |
| Admin | Blazor Server 10 + MudBlazor | Interface admin |
| Database | PostgreSQL 17 | Persistence |
| Cache | Redis (à venir) | Performance |
| Queue | PostgreSQL Outbox | Messaging |
| Logging | Serilog | Observabilité |
| Payments | Stripe | Transactions |

## Évolution Future

- **Microservices** : Si besoin de scaling indépendant de modules
- **Event Bus** : RabbitMQ ou Azure Service Bus
- **API Gateway** : Kong ou Azure API Management
- **Search Engine** : Elasticsearch pour recherche avancée
- **ML/AI** : Azure Cognitive Services pour embeddings
