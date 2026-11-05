# ReuseMarket

### Buy, Sell and Give Items a Second Life.

ReuseMarket is a web-based second-hand marketplace designed to connect buyers and sellers in Uganda. It aims to make affordable products accessible while encouraging the reuse of items.

The project starts with Kampala, with a long-term vision of expanding across Uganda and East Africa

## Features

- Add and list second-hand products.
- Upload product images with live preview.
- Validate product information and image uploads.
- Browse products stored in a PostgreSQL database.
- Select products from predefined categories.

## Technologies Used

- C#
- ASP.NET Core MVC
- Entity Framework Core
- PostgreSQL
- HTML, CSS and JavaScript
- Bootstrap

## Getting Started

Clone the repository:

```bash
git clone https://github.com/Kevin-nya/ReuseMarket.git
```

Navigate to the project:

```bash
cd ReuseMarket
```

Restore dependencies:

```bash
dotnet restore
```

Configure your PostgreSQL connection string using User Secrets, then apply migrations:

```bash
dotnet ef database update
```

Run the application:

```bash
dotnet run
```

## Planned Features

- User registration and authentication.
- Product search and filtering.
- Seller profiles and verification.
- Shopping cart and order management.
- Mobile Money integration.
- Reviews and ratings.

## Project Status

**Active Development** — Core marketplace functionality is currently being implemented.

## Developer

**Kevin Nyanzi**

[GitHub Repository](https://github.com/Kevin-nya/ReuseMarket)

---

*ReuseMarket — Give items a second life.*
