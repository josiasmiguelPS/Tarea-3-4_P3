const API_EMPLEADO = 'http://localhost:5142/api/Empleado';
const API_PRODUCTO = 'http://localhost:5142/api/Producto';

const VALID_USERS = [
    { user: 'admin', pass: 'admin123' },
    { user: 'josias', pass: '1234' }
];

let modoEdicionProd = false;
let deleteProdId = null;

// ================= LOGIN Y NAVEGACIÓN =================
function doLogin() {
    const u = document.getElementById('username').value.trim();
    const p = document.getElementById('password').value.trim();
    const ok = VALID_USERS.some(v => v.user === u && v.pass === p);

    if (ok) {
        sessionStorage.setItem('loggedIn', '1');
        showApp();
    } else {
        document.getElementById('loginError').style.display = 'block';
    }
}

function logout() {
    sessionStorage.clear();
    location.reload();
}

function showApp() {
    document.getElementById('loginPage').style.display = 'none';
    document.getElementById('appPage').style.display = 'block';
    document.getElementById('navMenu').style.display = 'block';
    document.getElementById('btnLogout').style.display = 'block';
    mostrarModulo('moduloProductos'); // Carga inventario por defecto
}

function mostrarModulo(idModulo) {
    document.getElementById('moduloEmpleados').style.display = 'none';
    document.getElementById('moduloProductos').style.display = 'none';
    document.getElementById(idModulo).style.display = 'block';

    if (idModulo === 'moduloProductos') cargarProductos();
    if (idModulo === 'moduloEmpleados') cargarEmpleados();
}

// ================= MÓDULO PRODUCTOS (INVENTARIO) =================
async function cargarProductos() {
    try {
        const res = await fetch(`${API_PRODUCTO}/Listar`);
        const data = await res.json();
        renderTablaProductos(data);
    } catch {
        console.error("Error al cargar inventario");
    }
}

function renderTablaProductos(productos) {
    const tbody = document.getElementById('tbodyProductos');
    tbody.innerHTML = '';

    productos.forEach(p => {
        // Validación visual de stock bajo
        const stockStyle = p.stockActual <= p.stockMinimo ? 'color: red; font-weight: bold;' : '';
        const tipoVenta = p.seVendePorCaja ? `Caja (x${p.unidadesPorCaja})` : 'Unidad';

        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${p.codigoBarras || 'N/A'}</td>
            <td>${p.nombre}</td>
            <td>$${p.costo.toFixed(2)}</td>
            <td>$${p.precio.toFixed(2)}</td>
            <td style="${stockStyle}">${p.stockActual}</td>
            <td>${tipoVenta}</td>
            <td>
                <div class="tbl-actions">
                    <button class="btn-warning" onclick="cargarParaEditarProd(${p.id})">Editar</button>
                    <button class="btn-danger" onclick="abrirModalProd(${p.id}, '${p.nombre}')">Borrar</button>
                </div>
            </td>`;
        tbody.appendChild(tr);
    });
}

async function guardarProducto() {
    const idVal = document.getElementById('productoId').value;

    // Armamos el DTO exactamente como lo espera el backend de C#
    const modeloDto = {
        codigoBarras: document.getElementById('prodCodigo').value.trim(),
        nombre: document.getElementById('prodNombre').value.trim(),
        categoriaId: parseInt(document.getElementById('prodCategoria').value) || 1, // Por defecto 1 para evitar errores si está vacío
        costo: parseFloat(document.getElementById('prodCosto').value) || 0,
        precio: parseFloat(document.getElementById('prodPrecio').value) || 0,
        stockActual: parseInt(document.getElementById('prodStock').value) || 0,
        stockMinimo: parseInt(document.getElementById('prodMinimo').value) || 5,
        seVendePorCaja: document.getElementById('prodEsCaja').checked,
        unidadesPorCaja: parseInt(document.getElementById('prodUnidadesCaja').value) || 1
    };

    if (!modeloDto.nombre || modeloDto.precio <= 0) {
        mostrarMensaje('El nombre y un precio válido son obligatorios.', false);
        return;
    }

    const isEdit = modoEdicionProd;
    const url = isEdit ? `${API_PRODUCTO}/Modificar/${idVal}` : `${API_PRODUCTO}/Agregar`;
    const method = isEdit ? 'PUT' : 'POST';

    try {
        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(modeloDto)
        });

        if (res.ok) {
            mostrarMensaje(isEdit ? 'Producto actualizado.' : 'Producto agregado.', true);
            limpiarFormProducto();
            cargarProductos();
        } else {
            mostrarMensaje('Error al guardar.', false);
        }
    } catch {
        mostrarMensaje('Error de conexión con la API.', false);
    }
}

async function cargarParaEditarProd(id) {
    const res = await fetch(`${API_PRODUCTO}/Obtener/${id}`);
    const p = await res.json();

    document.getElementById('productoId').value = p.id;
    document.getElementById('prodCodigo').value = p.codigoBarras || '';
    document.getElementById('prodNombre').value = p.nombre;
    document.getElementById('prodCategoria').value = p.categoriaId;
    document.getElementById('prodCosto').value = p.costo;
    document.getElementById('prodPrecio').value = p.precio;
    document.getElementById('prodStock').value = p.stockActual;
    document.getElementById('prodMinimo').value = p.stockMinimo;
    document.getElementById('prodEsCaja').checked = p.seVendePorCaja;
    document.getElementById('prodUnidadesCaja').value = p.unidadesPorCaja;

    modoEdicionProd = true;
    document.getElementById('formTitleProducto').textContent = 'Editar Producto';
    document.getElementById('btnGuardarProd').textContent = 'Actualizar';
    document.getElementById('btnCancelarProd').style.display = 'inline-block';
}

function abrirModalProd(id, nombre) {
    deleteProdId = id;
    document.getElementById('modalMsg').textContent = `¿Seguro que deseas eliminar "${nombre}"?`;
    document.getElementById('modalOverlay').classList.add('open');
}

async function confirmarEliminarProd() {
    document.getElementById('modalOverlay').classList.remove('open');
    try {
        const res = await fetch(`${API_PRODUCTO}/Eliminar?id=${deleteProdId}`, { method: 'DELETE' });

        if (res.ok || res.status === 204) {
            mostrarMensaje('Producto eliminado.', true);
            cargarProductos();
        } else if (res.status === 400) {
            mostrarMensaje('No se puede eliminar porque tiene facturas.', false);
        }
    } catch {
        mostrarMensaje('Error de red.', false);
    }
}

function limpiarFormProducto() {
    ['productoId', 'prodCodigo', 'prodNombre', 'prodCategoria', 'prodCosto', 'prodPrecio', 'prodStock', 'prodMinimo'].forEach(id => document.getElementById(id).value = '');
    document.getElementById('prodEsCaja').checked = false;
    document.getElementById('prodUnidadesCaja').value = '1';

    modoEdicionProd = false;
    document.getElementById('formTitleProducto').textContent = 'Agregar Producto al Inventario';
    document.getElementById('btnGuardarProd').textContent = 'Guardar Producto';
    document.getElementById('btnCancelarProd').style.display = 'none';
}

function mostrarMensaje(msg, esExito) {
    const div = document.getElementById('formFeedbackProd');
    div.textContent = msg;
    div.style.display = 'block';
    div.style.backgroundColor = esExito ? '#d4edda' : '#f8d7da';
    div.style.color = esExito ? '#155724' : '#721c24';
    setTimeout(() => div.style.display = 'none', 3000);
}

// Auto-Login si ya hay sesión
if (sessionStorage.getItem('loggedIn') === '1') showApp();
// ================= MÓDULO PUNTO DE VENTA (POS) =================
let catalogoVentas = [];
let carritoVentas = [];

// Actualizar la función mostrarModulo para cargar el POS
function mostrarModulo(idModulo) {
    document.getElementById('moduloEmpleados').style.display = 'none';
    document.getElementById('moduloProductos').style.display = 'none';
    document.getElementById('moduloVentas').style.display = 'none';
    document.getElementById(idModulo).style.display = 'block';

    if (idModulo === 'moduloProductos') cargarProductos();
    if (idModulo === 'moduloEmpleados') cargarEmpleados(); // Si lo tienes implementado
    if (idModulo === 'moduloVentas') cargarCatalogoPos();
}

async function cargarCatalogoPos() {
    try {
        const res = await fetch(`${API_PRODUCTO}/Listar`);
        catalogoVentas = await res.json();
        renderGridPos(catalogoVentas);
    } catch {
        console.error("Error cargando catálogo para ventas.");
    }
}

function renderGridPos(lista) {
    const grid = document.getElementById('gridProductosPos');
    grid.innerHTML = '';

    lista.forEach(p => {
        // 1. Lógica de Colores por Stock
        let claseEstado = 'estado-disponible'; // Verde por defecto

        if (p.stockActual === 0) {
            claseEstado = 'estado-agotado'; // Gris y sin clic
        } else if (p.stockActual < 5) {
            claseEstado = 'estado-escaso'; // Rojo
        }

        // 2. Lógica de Imagen (Simulada si no existe en BD)
        // Usamos Placehold.co para generar una imagen genérica bonita con el nombre del producto
        const textoImg = encodeURIComponent(p.nombre.substring(0, 10));
        const imagenUrl = p.imagenUrl ? p.imagenUrl : `https://placehold.co/300x200/e8eaf6/1a237e?text=${textoImg}`;

        // 3. Renderizar Tarjeta
        const card = document.createElement('div');
        card.className = `prod-card ${claseEstado}`;
        card.onclick = () => agregarAlCarrito(p);
        card.innerHTML = `
            <img src="${imagenUrl}" alt="${p.nombre}">
            <h4>${p.nombre}</h4>
            <span class="precio">$${p.precio.toFixed(2)}</span>
            <span class="stock">Disponibles: <b>${p.stockActual}</b></span>
        `;
        grid.appendChild(card);
    });
}

function filtrarPos() {
    const texto = document.getElementById('buscadorPos').value.toLowerCase();
    const filtrados = catalogoVentas.filter(p =>
        p.nombre.toLowerCase().includes(texto) ||
        (p.codigoBarras && p.codigoBarras.toLowerCase().includes(texto))
    );
    renderGridPos(filtrados);
}

// ==== Lógica del Carrito ====
function agregarAlCarrito(producto) {
    const itemExistente = carritoVentas.find(item => item.id === producto.id);

    if (itemExistente) {
        if (itemExistente.cantidad + 1 > producto.stockActual) {
            alert('No hay suficiente stock disponible');
            return;
        }
        itemExistente.cantidad++;
    } else {
        carritoVentas.push({ ...producto, cantidad: 1 });
    }
    actualizarCarrito();
}

function cambiarCantidadCarrito(id, nuevaCantidad) {
    const item = carritoVentas.find(i => i.id === id);
    if (!item) return;

    if (nuevaCantidad > item.stockActual) {
        alert('Supera el stock disponible');
        item.cantidad = item.stockActual;
    } else if (nuevaCantidad <= 0) {
        quitarDelCarrito(id);
        return;
    } else {
        item.cantidad = parseInt(nuevaCantidad);
    }
    actualizarCarrito();
}

function quitarDelCarrito(id) {
    carritoVentas = carritoVentas.filter(item => item.id !== id);
    actualizarCarrito();
}

function actualizarCarrito() {
    const lista = document.getElementById('listaCarrito');
    lista.innerHTML = '';
    let total = 0;

    if (carritoVentas.length === 0) {
        lista.innerHTML = '<div style="text-align:center; color:#999; margin-top: 20px;">El carrito está vacío</div>';
    } else {
        carritoVentas.forEach(item => {
            const subtotal = item.precio * item.cantidad;
            total += subtotal;

            lista.innerHTML += `
                <div class="carrito-item">
                    <div class="item-info">
                        <h5>${item.nombre}</h5>
                        <small>$${item.precio.toFixed(2)} c/u</small>
                    </div>
                    <div class="item-qty">
                        <input type="number" min="1" max="${item.stockActual}" value="${item.cantidad}" onchange="cambiarCantidadCarrito(${item.id}, this.value)">
                        <button class="btn-quitar" onclick="quitarDelCarrito(${item.id})">✖</button>
                    </div>
                </div>
            `;
        });
    }

    document.getElementById('subTotalVenta').textContent = `$${total.toFixed(2)}`;
    document.getElementById('totalVenta').textContent = `$${total.toFixed(2)}`;
}

function procesarVenta() {
    if (carritoVentas.length === 0) {
        alert('Agrega artículos al carrito primero.');
        return;
    }

    // Aquí es donde en el futuro llamaremos a la API (FacturaController)
    // para descontar el stock en la base de datos de C#
    alert(`Venta cobrada con éxito por un total de ${document.getElementById('totalVenta').textContent}`);
    carritoVentas = [];
    actualizarCarrito();
}