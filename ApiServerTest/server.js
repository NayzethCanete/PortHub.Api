const jsonServer = require('json-server');
const path = require('path');
const middleware = require('./middleware.js'); // Tu middleware personalizado

const server = jsonServer.create();
const router = jsonServer.router(path.join(__dirname, 'db.json'));
const middlewares = jsonServer.defaults();

// Usar middlewares predeterminados de json-server
server.use(middlewares);

// Usar tu middleware personalizado
server.use(middleware);

// Usar el enrutador de json-server
server.use(router);

// Iniciar el servidor
const PORT = 3000;
server.listen(PORT, () => {
  console.log(`JSON Server está corriendo en http://localhost:${PORT}`);
});