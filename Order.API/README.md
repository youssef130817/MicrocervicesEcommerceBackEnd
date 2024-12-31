# Documentation API des Commandes (Order.API)

## Base URL

```
http://localhost:5188/api/orders
```

## Endpoints

### 1. Créer une commande

```http
POST /api/orders
Content-Type: application/json

{
    "cartItems": [
        {
            "productId": "guid-du-produit",
            "productName": "Nom du produit",
            "unitPrice": 99.99,
            "quantity": 2,
            "imageUrl": "url-de-image"
        }
    ],
    "shippingAddress": {
        "street": "123 Main St",
        "city": "Paris",
        "state": "IDF",
        "zipCode": "75001",
        "phoneNumber": "0123456789"
    }
}
```

**Réponse réussie (201 Created)**

```json
{
	"id": "guid-de-la-commande",
	"status": "Confirmed",
	"totalAmount": 199.98,
	"items": [
		{
			"id": "guid-item",
			"productId": "guid-du-produit",
			"productName": "Nom du produit",
			"unitPrice": 99.99,
			"quantity": 2,
			"imageUrl": "url-de-image"
		}
	],
	"shippingAddress": {
		"street": "123 Main St",
		"city": "Paris",
		"state": "IDF",
		"zipCode": "75001",
		"phoneNumber": "0123456789"
	},
	"createdAt": "2023-12-31T00:00:00Z",
	"updatedAt": "2023-12-31T00:00:00Z"
}
```

### 2. Obtenir toutes les commandes

```http
GET /api/orders?page=1&pageSize=10&status=Confirmed
```

**Paramètres de requête**

- `page` (optionnel, défaut: 1) : Numéro de la page
- `pageSize` (optionnel, défaut: 10) : Nombre d'éléments par page
- `status` (optionnel) : Filtre par statut (Confirmed, Shipped, Delivered, Cancelled)

**Réponse réussie (200 OK)**

```json
{
    "orders": [
        {
            "id": "guid-de-la-commande",
            "status": "Confirmed",
            "totalAmount": 199.98,
            "items": [...],
            "shippingAddress": {...},
            "createdAt": "2023-12-31T00:00:00Z",
            "updatedAt": "2023-12-31T00:00:00Z"
        }
    ],
    "pagination": {
        "currentPage": 1,
        "pageSize": 10,
        "totalPages": 5,
        "totalItems": 50
    }
}
```

### 3. Obtenir une commande spécifique

```http
GET /api/orders/{id}
```

**Réponse réussie (200 OK)**

```json
{
    "id": "guid-de-la-commande",
    "status": "Confirmed",
    "totalAmount": 199.98,
    "items": [...],
    "shippingAddress": {...},
    "createdAt": "2023-12-31T00:00:00Z",
    "updatedAt": "2023-12-31T00:00:00Z"
}
```

### 4. Annuler une commande

```http
PUT /api/orders/{id}/cancel
```

**Réponse réussie (200 OK)**

```json
{
	"message": "Order cancelled successfully"
}
```

### 5. Obtenir le suivi d'une commande

```http
GET /api/orders/{id}/tracking
```

**Réponse réussie (200 OK)**

```json
{
	"id": "guid-de-la-commande",
	"status": "Shipped",
	"trackingNumber": "123456789",
	"updatedAt": "2023-12-31T00:00:00Z"
}
```

### 6. Supprimer toutes les commandes

```http
DELETE /api/orders/all
```

**Réponse réussie (200 OK)**

```json
{
	"message": "Toutes les commandes ont été supprimées avec succès"
}
```

**Réponse d'erreur (500 Internal Server Error)**

```json
{
	"message": "Une erreur est survenue lors de la suppression des commandes",
	"error": "Description de l'erreur"
}
```

## Statuts des commandes

- `Confirmed` : Commande confirmée
- `Shipped` : Commande expédiée
- `Delivered` : Commande livrée
- `Cancelled` : Commande annulée

## Codes d'erreur

- `400 Bad Request` : Requête invalide (ex: panier vide)
- `404 Not Found` : Commande non trouvée
- `500 Internal Server Error` : Erreur serveur

## Notes

- Tous les montants sont en euros avec 2 décimales
- Les dates sont au format ISO 8601
- Les IDs sont au format GUID
- Le total de la commande est calculé automatiquement à partir des articles
