# fs_2025_assessment_1_74491  
DublinBikes API - V1 (JSON) and V2 (Cosmos DB)

## 1. Project Overview
This project implements a DublinBikes API with two data sources:
- V1: JSON file
- V2: Cosmos DB
Both versions share identical endpoints.

## 2. Technologies Used
- .NET 8
- Minimal APIs
- Cosmos DB Emulator
- xUnit
- Postman

## 3. API Versioning
Both V1 and V2 expose the same endpoints:
GET /api/v1/stations  
GET /api/v1/stations/{number}  
POST /api/v1/stations  
PUT /api/v1/stations/{number}  
GET /api/v1/stations/summary  
GET /api/v2/stations  
GET /api/v2/stations/{number}  
POST /api/v2/stations  
PUT /api/v2/stations/{number}  
GET /api/v2/stations/summary  

## 4. Query Parameters
status, minBikes, search, sortBy, sortDir, page, pageSize.

## 5. Example Response
{
  "number": 42,
  "name": "SMITHFIELD NORTH",
  "status": "OPEN"
}

## 6. Summary Endpoint
Returns totals for number of stations, bike stands, available bikes, and station status counts.

## 7. Random Live Updates
Random simulation of available bikes, timestamps, occupancy, etc.

## 8. Cosmos DB Implementation
Seeding, reading, updating, SQL queries, primary key and URI URL.

## 9. In-Memory Caching
Applied to V1 for list and station lookups.

## 10. Automated Tests
xUnit tests cover filtering, updating, adding, and summary.

## 11. Postman Collection
Contains tests for all endpoints and supports the Postman Test Runner.

## 12. How to Run
1. Start Cosmos DB Emulator
2. Run API from Visual Studio 2022
3. Use Swagger or Postman

## 13. Author
Armando Nunes  
Student ID: 74491  
Module: Full Stack Development  
Dorset College Dublin
