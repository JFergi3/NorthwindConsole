# Northwind Console Application

This is a menu-driven C# console application built with Entity Framework Core using the Northwind database.

View Menu UI Demo by clicking on link below.

https://youtu.be/PGRfiaotCKM

## Features
- Full CRUD operations for Products and Categories
- Input validation with data annotations
- Logging using NLog
- Orphan handling to maintain relational integrity
- Search and reporting feature for Products and Categories

## Architecture
The application is structured using a simple service-based design:
- ProductService – handles product operations
- CategoryService – handles category operations
- MenuService – manages UI and navigation
- SearchReport – provides reporting and search functionality
- ConsoleHelper – handles console formatting

## Purpose
This project demonstrates database interaction, error handling, and clean code organization using C# and Entity Framework Core.
