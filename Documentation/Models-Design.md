# Diseño del Modelo de Datos — PortHub (API Aeropuerto)

## Descripcion:
 Este documento que describe las entidades del dominio, atributos, relaciones y restricciones que se implementarán con Entity Framework Core (Fluent API + Data Annotations).



## Entidades

## Airline
Representa una compañía aérea que opera en el aeropuerto.

- `id`: INT (PK)
- `nombre`: VARCHAR(100)
- `codigo`: INT
- `pais`:  VARCHAR(50)
- `direccion_base`: VARCHAR(200)

## Flight
Representa una operación aérea programada.

- `id`: INT (PK)
- `codigo_vuelo`: VARCHAR(50)
- `aerolinea_id`: INT (FK)
- `fecha_hora_programada`: DATETIME
- `slot_id`: INT (FK)

## Slot
Franja horaria asignada a un vuelo.

- `id`: INT (PK)
- `fecha_hora`: DATETIME
- `pista`: INT
- `gate_id`: INT (FK)
- `estado`: VARCHAR(20)

## Gate
Puerta de embarque.

- `id`: INT (PK)
- `nombre`: VARCHAR(20)
- `ubicacion`: VARCHAR(50)

## Ticket
Identificador del pasajero para un vuelo.

- `id`: INT (PK)
- `vuelo_id`: INT (FK)
- `pasajero_nombre`: VARCHAR(50)
- `validado`: BOOLEAN

## Boarding
Registro de acceso del pasajero.

- `id`: INT (PK)
- `ticket_id`: INT (FK)
- `gate_id`: INT (FK)
- `hora_acceso`: DATETIME

## Relaciones

- Una aerolínea tiene muchos vuelos.
- Un vuelo tiene un slot y muchos tickets.
- Un slot se asigna a un solo vuelo.
- Un gate puede estar en muchos slots y embarques.
- Un ticket puede tener un solo embarque.


## Notas implementacion en EF Core 

- Usar HasIndex(...).IsUnique() para garantizar que no se repitan fecha_hora + pista en Slot.
- Usar HasOne(...).WithMany(...) para relaciones 1:N.
- Usar HasOne(...).WithOne(...) para relaciones 1:1 como Vuelo → Slot.
- Validar integridad referencial con OnDelete(DeleteBehavior.Restrict) donde sea necesario.


