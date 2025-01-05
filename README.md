# Architecture Microservices E-commerce

## Vue d'ensemble des Microservices

### Cart.API

**Description:**  
Service de gestion du panier d'achats permettant aux utilisateurs de gérer leurs articles temporairement.

**Technologies utilisées:**

- .NET 8
- Redis (pour le stockage des données du panier)

**APIs exposées:**

- `GET /cart/{cartId}` - Récupérer le contenu d'un panier
- `PUT /cart` - Mettre à jour un panier
- `DELETE /cart/{cartId}` - Supprimer un panier

**Caractéristiques:**

- Stockage temporaire des paniers avec une durée de vie de 30 jours
- Gestion des données en mémoire avec Redis

### Order.API

**Description:**  
Service de gestion des commandes pour enregistrer et suivre les commandes des clients.

**Technologies utilisées:**

- .NET 8
- Entity Framework Core (pour la persistance des données)
- SQL Lite (pour la base de données)

**APIs exposées:**

- `POST /api/orders` - Créer une commande
- `GET /api/orders` - Récupérer toutes les commandes
- `GET /api/orders/{orderId}/tracking` - Suivre une commande spécifique

### Product.API

**Description:**  
Service de gestion des produits et des catégories, permet la gestion du catalogue produits et categories avec gestion des images.

**Technologies utilisées:**

- .NET 8
- Entity Framework Core avec SQLite
- AutoMapper

**APIs exposées:**

- `GET /api/products` - Liste des produits avec pagination et filtres
- `GET /api/products/{id}` - Détails d'un produit
- `POST /api/products` - Créer un nouveau produit
- `PUT /api/products/{id}` - Mettre à jour un produit
- `DELETE /api/products/{id}` - Supprimer un produit
- `GET /api/categories` - Liste des catégories
- `GET /api/categories/{id}` - Détails d'une catégorie
- `POST /api/categories` - Créer une catégorie
- `PUT /api/categories/{id}` - Mettre à jour une catégorie
- `DELETE /api/categories/{id}` - Supprimer une catégorie
- `GET /api/images/{imagePath}` - Récupérer une image produit

## Communication entre les Services

Les microservices communiquent entre eux via:

- API REST
