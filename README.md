# MVC Music Store Modernization

This project is a modernization exercise based on the legacy ASP.NET MVC Music Store application.

The goal was to move the application from:

* ASP.NET MVC / .NET Framework
* SQL Server

to:

* ASP.NET Core MVC / .NET 8
* PostgreSQL
* EF Core with Npgsql

## Completed Work

* Restored and reviewed the legacy application.
* Created a new ASP.NET Core MVC application.
* Added PostgreSQL support using EF Core and Npgsql.
* Migrated catalog data for Albums, Artists, and Genres.
* Implemented Store, Browse, Details, Cart, and Checkout workflows.
* Added Login/Register/Logout using ASP.NET Core Identity.
* Protected admin pages using role-based authorization.

## Main Demo Flow

Home --> Store --> Browse Genre --> Album Details --> Add to Cart --> Checkout --> Order Complete

## Migration Notes

Catalog data was exported from SQL Server and imported into PostgreSQL using scripts included in the `MigrationData` folder.

## Status

The main modernization workflow is complete and ready for review.

## Future Scope

* AWS deployment
* Additional UI refinements
