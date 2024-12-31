# Spécifications Frontend E-commerce React

## Configuration Technique

- Framework: React.js 18+ avec Vite
- Gestion d'État: Redux Toolkit ou Zustand
- Routage: React Router v6
- UI Framework:
  - Tailwind CSS 3.0+
  - Shadcn/ui (composants modernes préconçus)
  - Framer Motion (pour les animations)
- HTTP Client: Axios
- Validation de formulaire: React Hook Form + Zod

## Configuration API

Base URL Gateway: `http://localhost:5188`

Services individuels (pour référence) :

- Product API: `http://localhost:5143`
- Cart API: `http://localhost:5079`
- Order API: `http://localhost:5002`

## Endpoints API

### 1. Produits API (via Gateway: 5188)

#### Obtenir tous les produits

```http
GET /api/products

Response:
{
    "products": [
        {
            "id": "guid",
            "name": "string",
            "description": "string",
            "price": decimal,
            "stock": integer,
            "categoryId": "guid",
            "category": {
                "id": "guid",
                "name": "string"
            }
        }
    ]
}
```

#### Obtenir un produit

```http
GET /api/products/{id}

Response:
{
    "id": "guid",
    "name": "string",
    "description": "string",
    "price": decimal,
    "stock": integer,
    "categoryId": "guid",
    "category": {
        "id": "guid",
        "name": "string"
    }
}
```

#### Obtenir les catégories

```http
GET /api/categories

Response:
[
    {
        "id": "guid",
        "name": "string",
        "description": "string"
    }
]
```

### 2. Panier API (via Gateway: 5188)

#### Obtenir le panier

```http
GET /api/cart

Response:
{
    "id": "guid",
    "items": [
        {
            "id": "guid",
            "productId": "guid",
            "quantity": integer
        }
    ],
    "createdAt": "datetime",
    "updatedAt": "datetime"
}
```

#### Ajouter au panier

```http
POST /api/cart/items
Body:
{
    "productId": "guid",
    "quantity": integer
}

Response: Cart object
```

#### Mettre à jour la quantité

```http
PUT /api/cart/items/{productId}
Body:
{
    "quantity": integer
}

Response: Cart object
```

#### Supprimer du panier

```http
DELETE /api/cart/items/{productId}

Response: 200 OK
```

#### Vider le panier

```http
POST /api/cart/clear

Response: 200 OK
```

### 3. Commandes API (via Gateway: 5188)

#### Créer une commande

```http
POST /api/orders
Body:
{
    "cartId": "guid",
    "shippingAddress": {
        "street": "string",
        "city": "string",
        "postalCode": "string",
        "country": "string"
    }
}

Response:
{
    "id": "guid",
    "orderDate": "datetime",
    "status": "string",
    "totalAmount": decimal,
    "items": [
        {
            "productId": "guid",
            "quantity": integer,
            "unitPrice": decimal
        }
    ],
    "shippingAddress": {
        "street": "string",
        "city": "string",
        "postalCode": "string",
        "country": "string"
    }
}
```

## Architecture Frontend Proposée

```
src/
├── assets/
│   ├── images/
│   └── styles/
├── components/
│   ├── common/
│   │   ├── Button/
│   │   ├── Input/
│   │   ├── Card/
│   │   └── Loading/
│   ├── layout/
│   │   ├── Header/
│   │   ├── Footer/
│   │   └── Sidebar/
│   ├── products/
│   │   ├── ProductCard/
│   │   ├── ProductGrid/
│   │   └── ProductDetails/
│   ├── cart/
│   │   ├── CartItem/
│   │   └── CartSummary/
│   └── checkout/
│       ├── CheckoutForm/
│       └── OrderSummary/
├── hooks/
│   ├── useCart.ts
│   ├── useProducts.ts
│   └── useOrders.ts
├── lib/
│   ├── axios.ts
│   └── utils.ts
├── pages/
│   ├── Home/
│   ├── Products/
│   ├── ProductDetail/
│   ├── Cart/
│   └── Checkout/
├── store/
│   ├── cart/
│   ├── products/
│   └── orders/
└── types/
    └── index.ts
```

## Design System

### Thème de Couleurs

```css
:root {
	--primary: #0f172a; /* Slate 900 */
	--primary-light: #1e293b; /* Slate 800 */
	--secondary: #3b82f6; /* Blue 500 */
	--accent: #f59e0b; /* Amber 500 */
	--background: #ffffff;
	--surface: #f8fafc; /* Slate 50 */
	--text: #0f172a; /* Slate 900 */
	--text-light: #64748b; /* Slate 500 */
	--error: #ef4444; /* Red 500 */
	--success: #22c55e; /* Green 500 */
}
```

### Composants UI Essentiels

- Shadcn/ui pour:
  - Boutons
  - Inputs
  - Modals
  - Dropdowns
  - Cards
- Lucide Icons pour les icônes
- React Hot Toast pour les notifications
- React Loading Skeleton pour les états de chargement

### Fonctionnalités Principales

1. Page d'accueil (/)

   - Hero section avec promotions
   - Grille de catégories
   - Produits populaires
   - Newsletter signup

2. Liste des Produits (/products)

   - Filtres de catégories
   - Barre de recherche
   - Tri par prix/nom
   - Vue grille responsive
   - Lazy loading des images
   - Skeleton loading

3. Détail Produit (/products/:id)

   - Images du produit
   - Description détaillée
   - Sélecteur de quantité
   - Bouton d'ajout au panier
   - Toast de confirmation
   - Produits similaires

4. Panier (/cart)

   - Liste des articles
   - Contrôles de quantité
   - Calcul en temps réel
   - Bouton de suppression
   - Persistence locale
   - Checkout sécurisé

5. Checkout (/checkout)
   - Formulaire d'adresse
   - Validation des champs
   - Résumé de commande
   - Confirmation

### Responsive Design

```css
/* Breakpoints */
--mobile: 320px;
--tablet: 768px;
--laptop: 1024px;
--desktop: 1280px;
```

### Optimisations

- Code splitting automatique avec React.lazy()
- Prefetching des routes
- Mise en cache des requêtes avec React Query
- Optimistic UI updates
- Service Worker pour le mode hors ligne
- Compression des images avec sharp
- Bundle size optimization avec Rollup

### Sécurité

- Sanitization des inputs
- Protection XSS
- Rate limiting des requêtes API
- Validation des données côté client
- Gestion sécurisée du stockage local

### Tests

- Jest pour les tests unitaires
- React Testing Library
- Cypress pour les E2E
- MSW pour le mocking API
