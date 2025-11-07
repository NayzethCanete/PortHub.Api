# Diseño del Modelo de Datos — PortHub

## Descripción
Este documento describe el modelo de datos actual implementado en la API PortHub, incluyendo entidades, atributos clave, relaciones y consideraciones de implementación con Entity Framework Core.

---

## Entidades

### Airline
Representa una aerolínea registrada que opera en el aeropuerto.
*Referencia: `PortHub.Api/Models/Airline.cs`*

- `Id`: INT (PK, Identity)
- `Name`: STRING (Requerido)
- `Code`: STRING (Requerido, Único)
- `Country`: STRING (Opcional)
- `BaseAddress`: STRING (Opcional)
- `ApiUrl`: STRING (Opcional) - *Base URL para integración saliente (validación de tickets).*
- `ApiKey`: STRING (Opcional) - *Clave para autenticarse contra la API de la aerolínea.*

### Slot
Representa una franja horaria y pista asignada a una operación de vuelo (aterrizaje/despegue).
*Referencia: `PortHub.Api/Models/Slot.cs`*

- `Id`: INT (PK, Identity)
- `ScheduleTime`: DATETIME (Requerido) - *Horario programado de la operación.*
- `Runway`: STRING (Requerido) - *Identificador de la pista asignada (ej. "Pista 1").*
- `Status`: STRING (Requerido, Default: "Reservado") - *Estados: Reservado, Confirmado, Libre.*
- `FlightCode`: STRING (Opcional) - *Código del vuelo asociado (ej. AR1234).*
- `GateId`: INT? (FK, Opcional) - *Referencia a la puerta de embarque asignada.*
- `ReservationExpiresAt`: DATETIME? (Opcional) - *Marca de tiempo para expiración automática de reservas temporales.*
- *(Recomendado)* `AirlineId`: INT? (FK) - *Vínculo con la aerolínea propietaria de la operación.*

### Gate
Representa una puerta de embarque física en la terminal.
*Referencia: `PortHub.Api/Models/Gate.cs`*

- `Id`: INT (PK, Identity)
- `Name`: STRING (Opcional) - *Nombre visible (ej. "Puerta 5A").*
- `Location`: STRING (Opcional) - *Ubicación en la terminal (ej. "Terminal B, Ala Norte").*

### Boarding
Registro individual del embarque de un pasajero (trazabilidad de seguridad).
*Referencia: `PortHub.Api/Models/Boarding.cs`*

- `Id`: INT (PK, Identity)
- `TicketNumber`: STRING (Requerido) - *Identificador único del ticket del pasajero.*
- `FlightCode`: STRING (Requerido) - *Vuelo al que abordó.*
- `BoardingTime`: DATETIME (Requerido) - *Momento exacto del cruce por puerta.*
- `GateId`: INT (FK, Requerido) - *Puerta física utilizada para el embarque.*
- `SlotId`: INT? (FK, Opcional) - *Vínculo con la operación aérea específica.*

---

## Relaciones del Dominio

| Entidad Origen | Relación | Entidad Destino | Descripción |
| :--- | :---: | :--- | :--- |
| **Slot** | N:1 | **Gate** | Un slot puede tener asignada una única puerta (opcional). |
| **Slot** | 1:N | **Boarding** | Una operación de slot tiene múltiples registros de pasajeros embarcados. |
| **Gate** | 1:N | **Boarding** | Una puerta registra históricamente muchos embarques. |
| **Gate** | 1:N | **Slot** | Una puerta es reutilizada por diferentes slots en distintos horarios. |

---

## Notas de Implementación (EF Core)

1. **Integración de Entidades:** Se han simplificado las entidades teóricas `Flight` y `Ticket`, integrando sus identificadores clave (`FlightCode` y `TicketNumber`) directamente en las tablas transaccionales (`Slot` y `Boarding`) para agilizar la operación y reducir la complejidad del esquema en esta fase.

2. **Validación de Unicidad (Business Logic):**
   - La regla de negocio *"No puede existir más de un Slot activo para la misma combinación de `ScheduleTime` + `Runway`"* es garantizada actualmente por la lógica de servicio (`SlotService.ReserveSlot`).