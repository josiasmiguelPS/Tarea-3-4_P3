const API_BASE = 'http://localhost:5142/api/Empleado';

const VALID_USERS = [
    { user: 'admin', pass: 'admin123' },
    { user: 'itla', pass: 'itla2024' },
    { user: 'josias', pass: '1234' }
];

let modoEdicion = false;
let pendingDeleteId = null;

function doLogin() {
    const u = document.getElementById('username').value.trim();
    const p = document.getElementById('password').value.trim();
    const ok = VALID_USERS.some(v => v.user === u && v.pass === p);

    document.getElementById('loginError').style.display = ok ? 'none' : 'block';

    if (ok) {
        sessionStorage.setItem('loggedIn', '1');
        sessionStorage.setItem('currentUser', u);
        showApp();
    }
}

function logout() {
    sessionStorage.clear();
    document.getElementById('appPage').style.display = 'none';
    document.getElementById('loginPage').style.display = 'flex';
    document.getElementById('btnLogout').style.display = 'none';
    document.getElementById('username').value = '';
    document.getElementById('password').value = '';
    document.getElementById('loginError').style.display = 'none';
}

function showApp() {
    document.getElementById('loginPage').style.display = 'none';
    document.getElementById('appPage').style.display = 'block';
    document.getElementById('btnLogout').style.display = 'block';
    cargarEmpleados();
}

async function cargarEmpleados() {
    document.getElementById('spinner').style.display = 'block';
    document.getElementById('noData').style.display = 'none';

    try {
        const res = await fetch(`${API_BASE}/Listar`);
        const data = await res.json();
        renderTabla(data);
    } catch {
        mostrarFeedback('Error de conexion con la API.', false);
    } finally {
        document.getElementById('spinner').style.display = 'none';
    }
}

function renderTabla(empleados) {
    const tbody = document.getElementById('tbodyEmpleados');
    tbody.innerHTML = '';

    if (!empleados.length) {
        document.getElementById('noData').style.display = 'block';
        return;
    }

    empleados.forEach(e => {
        const tr = document.createElement('tr');
        tr.setAttribute('data-id', e.id);
        tr.innerHTML = `
      <td><span class="badge-id">#${e.id}</span></td>
      <td>${e.nombre ?? ''}</td>
      <td>${e.apellido ?? ''}</td>
      <td>${e.edad ?? ''}</td>
      <td>${e.direccion ?? ''}</td>
      <td>${e.numero ?? ''}</td>
      <td>${e.email ?? ''}</td>
      <td>
        <div class="tbl-actions">
          <button class="btn-warning btn-editar" data-id="${e.id}" onclick="cargarParaEditar(${e.id})">Editar</button>
          <button class="btn-danger btn-eliminar" data-id="${e.id}" onclick="pedirConfirmacion(${e.id}, '${e.nombre} ${e.apellido}')">Eliminar</button>
        </div>
      </td>`;
        tbody.appendChild(tr);
    });
}

async function guardarEmpleado() {
    const idVal = document.getElementById('empleadoId').value;

    const modelo = {
        iD: idVal ? parseInt(idVal) : 0,
        nombre: document.getElementById('nombre').value.trim(),
        apellido: document.getElementById('apellido').value.trim(),
        edad: parseInt(document.getElementById('edad').value) || null,
        direccion: document.getElementById('direccion').value.trim(),
        numero: document.getElementById('numero').value.trim(),
        email: document.getElementById('email').value.trim()
    };

    if (!modelo.nombre || !modelo.apellido) {
        mostrarFeedback('Nombre y Apellido son obligatorios.', false);
        return;
    }

    try {
        const isEdit = modoEdicion;
        const url = isEdit ? `${API_BASE}/Modificar` : `${API_BASE}/Agregar`;
        const method = isEdit ? 'PUT' : 'POST';

        const res = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(modelo)
        });

        if (res.ok) {
            mostrarFeedback(isEdit ? 'Empleado actualizado correctamente.' : 'Empleado agregado correctamente.', true);
            limpiarFormulario();
            await cargarEmpleados();
        } else {
            mostrarFeedback('Error al guardar.', false);
        }
    } catch {
        mostrarFeedback('Error de conexion.', false);
    }
}

async function cargarParaEditar(id) {
    try {
        const res = await fetch(`${API_BASE}/Listar`);
        const data = await res.json();
        const emp = data.find(e => e.id === id);
        if (!emp) return;

        document.getElementById('empleadoId').value = emp.id;
        document.getElementById('nombre').value = emp.nombre ?? '';
        document.getElementById('apellido').value = emp.apellido ?? '';
        document.getElementById('edad').value = emp.edad ?? '';
        document.getElementById('direccion').value = emp.direccion ?? '';
        document.getElementById('numero').value = emp.numero ?? '';
        document.getElementById('email').value = emp.email ?? '';

        modoEdicion = true;
        document.getElementById('formTitle').textContent = 'Editar Empleado';
        document.getElementById('btnGuardar').textContent = 'Actualizar';
        document.getElementById('btnCancelar').style.display = 'inline-block';

        window.scrollTo({ top: 0, behavior: 'smooth' });
    } catch { }
}

function pedirConfirmacion(id, nombre) {
    pendingDeleteId = id;
    document.getElementById('modalMsg').textContent = `¿Seguro que deseas eliminar a "${nombre}"?`;
    document.getElementById('modalOverlay').classList.add('open');
}

async function confirmarEliminar() {
    cerrarModal();
    try {
        const res = await fetch(`${API_BASE}/Eliminar?id=${pendingDeleteId}`, {
            method: 'DELETE'
        });

        if (res.ok || res.status === 204) {
            mostrarFeedback('Empleado eliminado.', true);
            await cargarEmpleados();
        } else {
            mostrarFeedback('No se encontro el empleado.', false);
        }
    } catch {
        mostrarFeedback('Error de conexion.', false);
    }
    pendingDeleteId = null;
}

function cerrarModal() {
    document.getElementById('modalOverlay').classList.remove('open');
}

function cancelarEdicion() {
    limpiarFormulario();
}

function limpiarFormulario() {
    ['empleadoId', 'nombre', 'apellido', 'edad', 'direccion', 'numero', 'email']
        .forEach(id => (document.getElementById(id).value = ''));

    modoEdicion = false;
    document.getElementById('formTitle').textContent = 'Agregar Empleado';
    document.getElementById('btnGuardar').textContent = 'Guardar';
    document.getElementById('btnCancelar').style.display = 'none';

    const fb = document.getElementById('formFeedback');
    fb.style.display = 'none';
}

function mostrarFeedback(msg, ok) {
    const fb = document.getElementById('formFeedback');
    fb.textContent = msg;
    fb.className = ok ? 'feedback-ok' : 'feedback-err';
    fb.style.display = 'block';
    setTimeout(() => (fb.style.display = 'none'), 4000);
}

document.addEventListener('keydown', e => {
    const loginVisible = document.getElementById('loginPage').style.display !== 'none';
    if (e.key === 'Enter' && loginVisible) doLogin();
});

if (sessionStorage.getItem('loggedIn') === '1') {
    showApp();
}