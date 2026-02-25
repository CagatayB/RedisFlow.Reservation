# 🚀 High-Scale Reservation System (.NET 8)

![.NET](https://img.shields.io/badge/.NET-8-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-blue)
![Redis](https://img.shields.io/badge/Redis-Cache-red)
![Docker](https://img.shields.io/badge/Docker-Container-blue)

---

## 📌 Project Overview

This project simulates a **high-concurrency reservation system** designed to handle real-world backend engineering challenges:

- Race condition prevention  
- Double booking elimination  
- Distributed locking  
- Cache strategy & invalidation  
- Idempotent request handling  

Instead of a basic CRUD demo, the focus is on **production-grade backend design decisions**.

---

## 🧠 Problem Statement

Consider a reservation slot:

Capacity = **10**

If **100 users** attempt to reserve simultaneously:

❌ Incorrect systems → Overbooking  
✅ Correct systems → must produce exactly 10 reservations  

This system guarantees **deterministic correctness under concurrency**.

---

## 🏗️ Architecture

The project follows **Clean Architecture** principles:

API → Application → Domain → Infrastructure

---

### ✅ Domain Layer

Contains core business entities:

- `Slot`
- `Reservation`
- `ReservationStatus`

Framework-independent business logic.

---

### ✅ Application Layer

Implements business rules:

- Reservation creation logic  
- Capacity enforcement  
- Concurrency handling  
- Idempotency flow  

---

### ✅ Infrastructure Layer

Handles external dependencies:

- **PostgreSQL** → Source of truth  
- **Redis** → Locking, caching, idempotency  

---

### ✅ API Layer

HTTP entry point:

- Reservation endpoints  
- Header-based idempotency handling  

---

## ⚔️ Concurrency Strategy

### 🔒 Distributed Lock (Redis)

To prevent race conditions:

lock:slot:{slotId}

Ensures only one request enters the critical reservation logic at a time.

---

## 🚀 Cache Strategy

### ⚡ Availability Cache (Redis)

availability:slot:{slotId}

Stores remaining capacity.

---

### ❗ Cache Update vs Invalidation

Instead of mutating cache values:

❌ `remaining--` → Consistency risk  
✅ Cache invalidation → Correctness-safe  

Database remains the **source of truth**.

---

## 🔁 Idempotency Handling

To prevent duplicate reservations caused by retries:

Idempotency-Key (HTTP Header)

Redis store:

idempotency:{key}

Guarantees:

✔ Retry-safe writes  
✔ No duplicate reservations  

---

## 🚀 Concurrency Test Scenario

Test Setup:

- Slot Capacity → 10  
- Concurrent Requests → 100  

Result:

✔ Successful Reservations → **10**  
✔ Double Booking → **Prevented**

---

## 🐳 Running the Project

### 1️⃣ Start Services

```bash
docker-compose up
dotnet ef database update
dotnet run
