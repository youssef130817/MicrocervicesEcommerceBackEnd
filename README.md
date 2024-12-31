# MyStore - Application E-commerce

## 🌟 Vue d'ensemble

MyStore est une application e-commerce moderne construite avec une architecture microservices. Elle comprend un frontend React moderne et un backend .NET Core composé de plusieurs microservices.

## 🏗️ Architecture

### Frontend (Port: 5173)

- React 18+ avec TypeScript
- Vite comme build tool
- TailwindCSS pour le styling
- React Query pour la gestion d'état
- Animations fluides avec Framer Motion

### Backend Microservices

- **Gateway API** (Port: 5188)

  - Point d'entrée principal
  - Routage vers les microservices
  - Gestion des requêtes

- **Product API** (Port: 5143)

  - Gestion des produits
  - Gestion des catégories
  - Recherche et filtrage

- **Cart API** (Port: 5079)

  - Gestion du panier
  - Persistance des articles
  - Calcul des totaux

- **Order API** (Port: 5002)
  - Traitement des commandes
  - Historique des commandes
  - Statut des commandes

## 🚀 Démarrage Rapide

### Prérequis

- .NET 7.0+
- Node.js 18+
- SQL Server
- Redis

### Installation

1. **Backend**

```bash
# Cloner le repository
git clone <repository-url>

# Démarrer les services dans l'ordre suivant :
cd Product.API
dotnet run

cd ../Cart.API
dotnet run

cd ../Order.API
dotnet run

cd ../Gateway.API
dotnet run
```

2. **Frontend**

```bash
cd mystore-frontend
npm install
npm run dev
```

## 📡 API Endpoints

### Products

- `GET /api/products` - Liste des produits
- `GET /api/products/{id}` - Détails d'un produit
- `GET /api/categories` - Liste des catégories

### Cart

- `GET /api/cart` - Obtenir le panier
- `POST /api/cart/items` - Ajouter au panier
- `PUT /api/cart/items/{productId}` - Modifier quantité
- `DELETE /api/cart/items/{productId}` - Supprimer du panier
- `POST /api/cart/clear` - Vider le panier

### Orders

- `POST /api/orders` - Créer une commande
- `GET /api/orders` - Liste des commandes
- `GET /api/orders/{id}` - Détails d'une commande

## 💡 Fonctionnalités

### Frontend

- Interface utilisateur moderne et responsive
- Gestion du panier en temps réel
- Animations fluides
- Thème personnalisable
- Formulaires validés
- Notifications toast
- Mode responsive

### Backend

- Architecture microservices
- Communication inter-services
- Mise en cache avec Redis
- Base de données SQL Server
- API RESTful
- Documentation Swagger

## 🎨 Design System

### Couleurs

```css
--primary: #0f172a     # Principal
--primary-light: #1e293b
--secondary: #3b82f6   # Actions
--accent: #f59e0b      # Accent
--surface: #f8fafc     # Background
--error: #ef4444      # Erreurs
--success: #22c55e    # Succès
```

## 📱 Pages Frontend

### 1. Accueil (/)

- Hero section
- Catégories populaires
- Produits vedettes
- Newsletter

### 2. Produits (/products)

- Liste des produits
- Filtres et recherche
- Tri
- Pagination

### 3. Détail Produit (/products/:id)

- Images produit
- Description
- Prix et stock
- Ajout au panier

### 4. Panier (/cart)

- Liste des articles
- Modification quantités
- Total
- Checkout

### 5. Checkout (/checkout)

- Informations livraison
- Résumé commande
- Confirmation

## 🔧 Configuration

### Backend

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Server=localhost;Database=MyStore;Trusted_Connection=True"
	},
	"Redis": {
		"ConnectionString": "localhost:6379"
	}
}
```

### Frontend

```env
VITE_API_URL=http://localhost:5188
```

## 📈 Performance

### Frontend

- Code splitting
- Lazy loading
- Cache optimisé
- Bundle size optimization

### Backend

- Mise en cache Redis
- Optimisation SQL
- Compression des réponses
- Rate limiting

## 🔒 Sécurité

- Validation des entrées
- Protection XSS
- Rate limiting
- Sanitization des données

## 🧪 Tests

### Frontend

- Jest pour les tests unitaires
- React Testing Library
- Cypress pour les E2E

### Backend

- Tests unitaires
- Tests d'intégration
- Tests de charge

## 📝 Conventions

- ESLint + Prettier (Frontend)
- .NET Code Style (Backend)
- Conventional Commits
- Documentation des API

## 🤝 Contribution

1. Fork le projet
2. Créer une branche (`git checkout -b feature/AmazingFeature`)
3. Commit (`git commit -m 'Add some AmazingFeature'`)
4. Push (`git push origin feature/AmazingFeature`)
5. Créer une Pull Request

## 📄 Licence

Ce projet est sous licence MIT.
