# Kamersoft.Ilon

Application mobile type TikTok pour la location/vente de biens immobiliers en Afrique (focus Cameroun).

## 📱 À Propos

Ilon est une plateforme immobilière moderne qui révolutionne la recherche et la publication de biens en Afrique. Inspirée par l'ergonomie de TikTok, elle offre une expérience utilisateur fluide et engageante basée sur la vidéo verticale.

## ✨ Fonctionnalités

- **Inscription rapide** : Authentification par téléphone + OTP
- **Feed vertical immersif** : Offres et demandes sous forme de vidéos/images
- **Recherche intelligente** : Filtres avancés + embeddings IA
- **Messaging intégré** : Conversations directes + système d'enchères
- **Backoffice de modération** : Interface administrateur complète
- **Paiements sécurisés** : Intégration Stripe

## 🛠️ Stack Technique

### Backend
- **.NET 10** / ASP.NET Core 10 Minimal API
- **PostgreSQL 17** (base de données principale)
- **Event Sourcing + CQRS** (Xpandables.Net)
- **EF Core 10** (ORM)
- **Serilog** (logging structuré)

### Frontend
- **MAUI 10** + DevExpress MAUI (application mobile)
- **Blazor Server 10** + MudBlazor (backoffice)

### Infrastructure
- **Docker & Docker Compose** (conteneurisation)
- **PostgreSQL** (persistence)
- **Redis** (cache - à venir)
- **Stripe** (paiements)

## 🏗️ Architecture

### Modular Monolith avec Vertical Slices
- Event Sourcing (write model)
- Projections (read model)
- Outbox pattern
- CQRS avec handlers dédiés

### Modules applicatifs

| Module | Responsabilité |
|--------|---------------|
| **Identity** | Authentification OTP, gestion profils |
| **Listings** | Offres immobilières des professionnels |
| **Requests** | Demandes des particuliers |
| **Search** | Recherche + filtres + embeddings IA |
| **Messaging** | Conversations et notifications |
| **Payments** | Transactions Stripe |
| **Moderation** | Outils backoffice |

### BuildingBlocks
- **Primitives** : ValueObjects réutilisables (PhoneNumber, Money)
- **Security** : Helpers sécurité (OTP Generator)
- **Observability** : Extensions logging
- **Extensions** : Utilitaires communs

## 🚀 Quick Start

### Prérequis
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/get-started)
- [Git](https://git-scm.com/)

### 1. Cloner le repository
```bash
git clone https://github.com/Francescolis/Kamersoft.Ilon.git
cd Kamersoft.Ilon
```

### 2. Démarrer PostgreSQL avec Docker Compose
```bash
docker-compose up -d
```

Cela démarre :
- **PostgreSQL** sur le port 5432
- **PgAdmin** sur http://localhost:5050

Identifiants PgAdmin :
- Email : `admin@ilon.com`
- Password : `admin`

### 3. Restaurer les packages
```bash
dotnet restore
```

### 4. Build la solution
```bash
dotnet build
```

### 5. Lancer l'API
```bash
cd src/Ilon.Api
dotnet run
```

L'API sera disponible sur :
- **HTTPS** : https://localhost:7001
- **HTTP** : http://localhost:5000
- **Swagger** : https://localhost:7001/swagger

### 6. Tester l'endpoint SendOTP

```bash
curl -X POST https://localhost:7001/api/auth/send-otp \
  -H "Content-Type: application/json" \
  -d '{"phoneNumber": "+237612345678"}'
```

Réponse attendue :
```json
{
  "message": "OTP sent to +237612345678. Valid for 5 minutes.",
  "expiresAt": "2026-01-08T18:45:00Z"
}
```

### 7. Exécuter les tests
```bash
dotnet test
```

## 📂 Structure du Projet

```
Kamersoft.Ilon/
├── src/
│   ├── Ilon.Api/                    # ASP.NET Core Minimal API
│   ├── Ilon.Backoffice/             # Blazor Server Admin
│   ├── Ilon.Mobile/                 # MAUI Mobile App
│   ├── Ilon.BuildingBlocks/         # Shared utilities
│   └── Ilon.Modules.*/              # Feature modules
├── tests/
│   ├── Ilon.Api.Tests/
│   └── Ilon.Modules.Identity.Tests/
├── docs/                             # Documentation
├── docker-compose.yml               # Infrastructure
└── Kamersoft.Ilon.slnx             # Solution file
```

## 🔧 Configuration

### Connection String
Par défaut dans `appsettings.json` :
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ilondb;Username=ilonuser;Password=ilonpassword"
  }
}
```

### OTP Configuration
```json
{
  "Otp": {
    "ExpirationMinutes": 5,
    "Length": 6
  }
}
```

### CORS
Configuré pour les origines :
- `http://localhost:3000`
- `http://localhost:5173`

## 🧪 Tests

Les tests unitaires sont écrits avec **xUnit** :

```bash
# Tous les tests
dotnet test

# Tests d'un projet spécifique
dotnet test tests/Ilon.Modules.Identity.Tests

# Avec détails
dotnet test --logger "console;verbosity=detailed"
```

Couverture actuelle :
- ✅ SendOtpHandler (5 tests)
- ✅ SendOtpValidator (4 tests)

## 📚 Documentation

- [Cahier des Charges](docs/CahierDesCharges.Ilon.md) - Spécifications complètes
- [Architecture](docs/Architecture.md) - Diagrammes et patterns

## 🔐 Sécurité

- **HTTPS** : Redirection automatique
- **OTP** : Codes 6 chiffres expiration 5 min
- **Validation** : Contrôles stricts sur les entrées
- **Secrets** : À externaliser dans Azure Key Vault ou AWS Secrets Manager

## 🌍 Internationalisation

Actuellement :
- Formats téléphone : **Cameroun (+237)** en priorité
- Devise principale : **XAF (Franc CFA)**
- Support : EUR, USD

## 🤝 Contribution

1. Fork le projet
2. Créer une branche feature (`git checkout -b feature/AmazingFeature`)
3. Commit vos changements (`git commit -m 'Add AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

## 📝 License

Ce projet est sous licence propriétaire - voir le fichier [LICENSE.txt](LICENSE.txt)

## 👥 Équipe

Développé par **Kamersoft**
- Contact : contact@kamersoft.com
- GitHub : [@Francescolis](https://github.com/Francescolis)

## 🗺️ Roadmap

### Phase 1 (MVP) - Q1 2026
- [x] Infrastructure de base
- [x] Module Identity (OTP)
- [ ] Module Listings (CRUD offres)
- [ ] Module Search (filtres basiques)
- [ ] Application Mobile (Android)

### Phase 2 - Q2 2026
- [ ] Messaging complet
- [ ] Système d'enchères
- [ ] Intégration Stripe
- [ ] Backoffice modération
- [ ] Support iOS

### Phase 3 - Q3 2026
- [ ] IA embeddings recherche
- [ ] Recommandations personnalisées
- [ ] Analytics avancés
- [ ] Support multi-pays

## ⚡ Performances

Objectifs :
- Temps réponse API : < 200ms
- Disponibilité : 99.9%
- Capacité : 10K utilisateurs concurrents

## 📞 Support

Pour toute question :
- Ouvrir une [Issue](https://github.com/Francescolis/Kamersoft.Ilon/issues)
- Email : support@kamersoft.com
