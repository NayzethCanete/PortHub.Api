module.exports = (req, res, next) => {
    if (req.method === "POST" && req.path === "/validar-ticket") {
      const { ticketNumber, flightDate } = req.body;
  
      // Simular base de datos (leer desde db.json)
      const tickets = require("./db.json").tickets;
  
      // Buscar el ticket en la "base de datos"
      const ticket = tickets.find(t => t.ticketNumber === ticketNumber);
  
      if (!ticket) {
        return res.status(404).json({
          isValid: false,
          msg: "Ticket no encontrado"
        });
      }
  
      // Verificar si la fecha del ticket coincide
      if (ticket.flightDate !== flightDate) {
        return res.status(400).json({
          isValid: false,
          msg: "La fecha del ticket no coincide"
        });
      }
  
      // Verificar si el ticket está cancelado
      if (ticket.status === "cancelled") {
        return res.status(400).json({
          isValid: false,
          msg: "El ticket está cancelado"
        });
      }
  
      // Si todo es válido
      return res.status(200).json({
        isValid: true,
        msg: "Ticket válido"
      });
    }
  
    // Continuar con el flujo normal de JSON Server
    next();
  };