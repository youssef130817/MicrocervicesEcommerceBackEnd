# Guide de Test E-commerce API

## Configuration de l'Environnement

1. Importer la collection Postman (`Ecommerce.postman_collection.json`)
2. Importer l'environnement Postman (`Ecommerce.postman_environment.json`)
3. Vérifier que Redis est en cours d'exécution pour Cart.API

## Démarrage des Services

```bash
# Gateway API (Port 5188)
cd Gateway.API
dotnet run

# Product API (Port 5143)
cd Product.API
dotnet run

# Order API (Port 5002)
cd Order.API
dotnet run

# Cart API (Port 5079)
cd Cart.API
dotnet run
```

## Scénarios de Test

### 1. Gestion des Produits

#### Test des Catégories

1. Obtenir la liste des catégories

```http
GET http://localhost:5188/api/categories
```

2. Créer une nouvelle catégorie

```http
POST http://localhost:5188/api/categories
Content-Type: application/json

{
    "name": "Electronics",
    "description": "Electronic devices and accessories"
}
```

#### Test des Produits

1. Obtenir la liste des produits avec filtres

```http
GET http://localhost:5188/api/products?page=1&pageSize=10&category=electronics&search=phone&sortBy=price&sortOrder=desc
```

2. Créer un nouveau produit

```http
POST http://localhost:5188/api/products
Content-Type: multipart/form-data

{
    "name": "New Smartphone",
    "description": "Latest smartphone model",
    "price": 999.99,
    "stock": 100,
    "categoryId": "{category-id}",
    "images": [file1, file2]
}
```

### 2. Gestion du Panier

1. Voir le contenu du panier

```http
GET http://localhost:5188/api/cart
```

2. Ajouter un produit au panier

```http
POST http://localhost:5188/api/cart/items
Content-Type: application/json

{
    "productId": "{product-id}",
    "quantity": 2
}
```

3. Vider le panier

```http
POST http://localhost:5188/api/cart/clear
```

### 3. Gestion des Commandes

1. Créer une nouvelle commande

```http
POST http://localhost:5188/api/orders
Content-Type: application/json

{
    "shippingAddress": {
        "street": "123 Main St",
        "city": "Example City",
        "state": "Example State",
        "country": "Example Country",
        "zipCode": "12345",
        "phoneNumber": "+1234567890"
    },
    "paymentMethod": "credit_card"
}
```

2. Obtenir la liste des commandes

```http
GET http://localhost:5188/api/orders?page=1&pageSize=10&status=Pending
```

3. Suivre une commande

```http
GET http://localhost:5188/api/orders/{order-id}/tracking
```

## Flux de Test Complet

1. **Préparation**:

   - Démarrer tous les services
   - Vérifier que Redis est en cours d'exécution

2. **Test des Produits**:

   - Créer une catégorie
   - Créer plusieurs produits dans cette catégorie
   - Tester les filtres et la pagination

3. **Test du Panier**:

   - Ajouter des produits au panier
   - Vérifier le calcul du total
   - Vider le panier

4. **Test des Commandes**:
   - Créer une commande
   - Vérifier le statut
   - Suivre la commande

## Validation des Tests

### Produits

- ✓ La création de produits fonctionne
- ✓ Les images sont correctement uploadées
- ✓ Les filtres et la pagination fonctionnent
- ✓ Les catégories sont correctement liées

### Panier

- ✓ Les produits sont ajoutés correctement
- ✓ Le total est calculé correctement
- ✓ La fonction "vider le panier" fonctionne

### Commandes

- ✓ La création de commande fonctionne
- ✓ Le suivi de commande est disponible
- ✓ La pagination des commandes fonctionne

## Codes HTTP Attendus

- 200: Succès
- 201: Création réussie
- 400: Requête invalide
- 404: Ressource non trouvée
- 500: Erreur serveur

## Dépannage

1. **Problèmes de Base de Données**:

   - Vérifier les fichiers SQLite :
     - `products.db` pour Product.API
     - `orders.db` pour Order.API
   - Vérifier Redis pour le panier :
     ```bash
     redis-cli ping
     redis-cli keys *
     ```

2. **Problèmes d'Images**:

   - Vérifier les limites de taille (max 5MB par image)
   - Formats supportés : .jpg, .jpeg, .png
   - Vérifier le dossier `wwwroot/images` dans Product.API

3. **Problèmes de Communication**:
   - Vérifier que tous les services sont démarrés sur les bons ports :
     - Gateway.API : 5188
     - Product.API : 5143
     - Order.API : 5002
     - Cart.API : 5079
   - Vérifier la configuration Ocelot dans `Gateway.API/ocelot.json`

## Scénarios de Test Avancés

### 1. Test de Gestion des Produits

#### Gestion des Catégories

```http
# Mettre à jour une catégorie
PUT http://localhost:5188/api/categories/{categoryId}
Content-Type: application/json

{
    "name": "Smartphones",
    "description": "Latest mobile devices"
}

# Supprimer une catégorie (impossible si elle contient des produits)
DELETE http://localhost:5188/api/categories/{categoryId}
```

#### Gestion des Produits

```http
# Mettre à jour un produit
PUT http://localhost:5188/api/products/{productId}
Content-Type: multipart/form-data

{
    "name": "Updated Smartphone",
    "price": 899.99,
    "stock": 50,
    "images": [newImage]
}

# Supprimer un produit
DELETE http://localhost:5188/api/products/{productId}
```

### 2. Test Avancé du Panier

```http
# Mettre à jour la quantité d'un article
PUT http://localhost:5188/api/cart/items/{productId}
Content-Type: application/json

{
    "quantity": 5
}

# Supprimer un article spécifique
DELETE http://localhost:5188/api/cart/items/{productId}
```

### 3. Test Avancé des Commandes

```http
# Annuler une commande
PUT http://localhost:5188/api/orders/{orderId}/cancel

# Filtrer les commandes par statut
GET http://localhost:5188/api/orders?status=Delivered

# Obtenir les détails d'une commande spécifique
GET http://localhost:5188/api/orders/{orderId}
```

## Cas de Test Spécifiques

### Test de Validation des Produits

- Prix négatif (doit échouer)
- Stock négatif (doit échouer)
- Nom vide (doit échouer)
- Prix = 0 (doit être accepté)
- Stock = 0 (doit être accepté)

### Test de Validation du Panier

- Ajouter un produit inexistant (doit échouer)
- Ajouter un produit avec quantité = 0 (doit échouer)
- Ajouter un produit avec quantité > stock (doit échouer)

### Test de Validation des Commandes

- Créer une commande avec panier vide (doit échouer)
- Créer une commande avec adresse invalide (doit échouer)
- Annuler une commande déjà expédiée (doit échouer)

## Monitoring et Logs

### Logs des Services

Chaque service écrit ses logs dans la console. Pour rediriger les logs vers un fichier :

```bash
# Gateway API
dotnet run > gateway-logs.txt 2>&1

# Product API
dotnet run > product-logs.txt 2>&1

# Order API
dotnet run > order-logs.txt 2>&1

# Cart API
dotnet run > cart-logs.txt 2>&1
```

### Surveillance Redis

```bash
# Surveiller les opérations Redis en temps réel
redis-cli monitor

# Obtenir des statistiques
redis-cli info
```

## Maintenance

### Nettoyage des Données

```bash
# Supprimer les bases de données SQLite
rm *.db

# Vider Redis
redis-cli flushall

# Supprimer les images uploadées
rm -rf Product.API/wwwroot/images/*
```

### Redémarrage Propre

1. Arrêter tous les services (Ctrl+C)
2. Nettoyer les données si nécessaire
3. Redémarrer Redis
4. Démarrer les services dans l'ordre :
   - Gateway.API
   - Product.API
   - Order.API
   - Cart.API

## Notes Importantes

- Tous les endpoints sont accessibles via la Gateway (port 5188)
- Les images sont stockées localement dans Product.API
- Les données du panier sont temporaires (Redis)
- Les commandes et produits sont persistants (SQLite)
