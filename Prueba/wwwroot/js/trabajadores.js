// Declarar token como variable global para que esté disponible en todo el script
let token;

document.addEventListener('DOMContentLoaded', function() {
    const selectDepartamento = document.getElementById('selectDepartamento');
    const selectProvincia = document.getElementById('selectProvincia');
    const selectDistrito = document.getElementById('selectDistrito');

    // Asignar valor a token para que esté disponible en todo el script
    token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Manejar cambio de departamento
    selectDepartamento.addEventListener('change', function() {
        const departamentoId = this.value;

        selectProvincia.innerHTML = '<option value="">-- Seleccione una provincia --</option>';
        selectProvincia.disabled = departamentoId === '';
        selectDistrito.innerHTML = '<option value="">-- Seleccione primero una provincia --</option>';
        selectDistrito.disabled = true;

        if (!departamentoId) return;

        fetch(`?handler=Provincias&departamentoId=${departamentoId}`, {
            headers: {
                'RequestVerificationToken': token
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error en la respuesta del servidor');
            }
            return response.json();
        })
        .then(data => {
            data.forEach(provincia => {
                const option = document.createElement('option');
                option.value = provincia.id;
                option.textContent = provincia.nombreProvincia;
                selectProvincia.appendChild(option);
            });
            selectProvincia.disabled = false;
        })
        .catch(error => console.error('Error al cargar provincias:', error));
    });

    // Manejar cambio de provincia
    selectProvincia.addEventListener('change', function() {
        const provinciaId = this.value;

        selectDistrito.innerHTML = '<option value="">-- Seleccione un distrito --</option>';
        selectDistrito.disabled = provinciaId === '';

        if (!provinciaId) return;

        fetch(`?handler=Distritos&provinciaId=${provinciaId}`, {
            headers: {
                'RequestVerificationToken': token
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error en la respuesta del servidor');
            }
            return response.json();
        })
        .then(data => {
            data.forEach(distrito => {
                const option = document.createElement('option');
                option.value = distrito.id;
                option.textContent = distrito.nombreDistrito;
                selectDistrito.appendChild(option);
            });
            selectDistrito.disabled = false;
        })
        .catch(error => console.error('Error al cargar distritos:', error));
    });

    // Manejar envío del formulario
    document.getElementById('formTrabajador').addEventListener('submit', function(e) {
        e.preventDefault();

        const formData = new FormData(this);
        const formDataObj = {};

        formData.forEach((value, key) => {
            // Manejar campos de ID para ubicación
            if ((key === 'IdDepartamento' || key === 'IdProvincia' || key === 'IdDistrito') && value === '') {
                formDataObj[key] = null;  // Enviar null en lugar de string vacío
            } else {
                formDataObj[key] = value;
            }
        });

        console.log('Datos del formulario:', formDataObj);

        fetch('?handler=AgregarTrabajador', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(formDataObj)
        })
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                mostrarNotificacion('Trabajador guardado exitosamente');
                window.location.reload();
            } else {
                mostrarNotificacion('Error: ' + result.message, 'error');
            }
        })
        .catch(error => {
            console.error('Error al guardar trabajador:', error);
            mostrarNotificacion('Error al guardar trabajador', 'error');
        });
    });

    // Funcionalidad para los selectores en el formulario de edición
    const editSelectDepartamento = document.getElementById('editSelectDepartamento');
    const editSelectProvincia = document.getElementById('editSelectProvincia');
    const editSelectDistrito = document.getElementById('editSelectDistrito');

    // Manejar cambio de departamento en el formulario de edición
    editSelectDepartamento.addEventListener('change', function() {
        const departamentoId = this.value;

        editSelectProvincia.innerHTML = '<option value="">-- Seleccione una provincia --</option>';
        editSelectProvincia.disabled = departamentoId === '';
        editSelectDistrito.innerHTML = '<option value="">-- Seleccione primero una provincia --</option>';
        editSelectDistrito.disabled = true;

        if (!departamentoId) return;

        fetch(`?handler=Provincias&departamentoId=${departamentoId}`, {
            headers: {
                'RequestVerificationToken': token
            }
        })
        .then(response => response.json())
        .then(data => {
            data.forEach(provincia => {
                const option = document.createElement('option');
                option.value = provincia.id;
                option.textContent = provincia.nombreProvincia;
                editSelectProvincia.appendChild(option);
            });
            editSelectProvincia.disabled = false;
        })
        .catch(error => console.error('Error al cargar provincias:', error));
    });

    // Manejar cambio de provincia en el formulario de edición
    editSelectProvincia.addEventListener('change', function() {
        const provinciaId = this.value;

        editSelectDistrito.innerHTML = '<option value="">-- Seleccione un distrito --</option>';
        editSelectDistrito.disabled = provinciaId === '';

        if (!provinciaId) return;

        fetch(`?handler=Distritos&provinciaId=${provinciaId}`, {
            headers: {
                'RequestVerificationToken': token
            }
        })
        .then(response => response.json())
        .then(data => {
            data.forEach(distrito => {
                const option = document.createElement('option');
                option.value = distrito.id;
                option.textContent = distrito.nombreDistrito;
                editSelectDistrito.appendChild(option);
            });
            editSelectDistrito.disabled = false;
        })
        .catch(error => console.error('Error al cargar distritos:', error));
    });

    // Modal de edición
    const modalEditarTrabajador = new bootstrap.Modal(document.getElementById('modalEditarTrabajador'));

    // Manejar botones de edición
    document.querySelectorAll('.btn-editar').forEach(btn => {
        btn.addEventListener('click', function() {
            const id = this.getAttribute('data-id');
            cargarDatosTrabajador(id);
        });
    });

    // Manejar envío del formulario de edición
    document.getElementById('formEditarTrabajador').addEventListener('submit', function(e) {
        e.preventDefault();

        const formData = new FormData(this);
        const formDataObj = {};

        formData.forEach((value, key) => {
            if ((key === 'IdDepartamento' || key === 'IdProvincia' || key === 'IdDistrito') && value === '') {
                formDataObj[key] = null;
            } else if (key === 'Id') {
                formDataObj[key] = parseInt(value);
            } else {
                formDataObj[key] = value;
            }
        });

        fetch('?handler=ActualizarTrabajador', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(formDataObj)
        })
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                mostrarNotificacion('Trabajador actualizado exitosamente');
                modalEditarTrabajador.hide();
                window.location.reload();
            } else {
                mostrarNotificacion('Error: ' + result.message, 'error');
            }
        })
        .catch(error => {
            console.error('Error al actualizar trabajador:', error);
            mostrarNotificacion('Error al actualizar trabajador', 'error');
        });
    });

    // Modal de confirmación de eliminación
    const modalConfirmarEliminar = new bootstrap.Modal(document.getElementById('modalConfirmarEliminar'));

    // Manejar botones de eliminación
    document.querySelectorAll('.btn-eliminar').forEach(btn => {
        btn.addEventListener('click', function() {
            const id = this.getAttribute('data-id');
            document.getElementById('idEliminar').value = id;
            modalConfirmarEliminar.show();
        });
    });

    // Manejar confirmación de eliminación
    document.getElementById('btnConfirmarEliminar').addEventListener('click', function() {
        const id = parseInt(document.getElementById('idEliminar').value);

        fetch('?handler=EliminarTrabajador', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(id)
        })
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                mostrarNotificacion('Trabajador eliminado exitosamente');
                modalConfirmarEliminar.hide();
                window.location.reload();
            } else {
                mostrarNotificacion('Error: ' + result.message, 'error');
            }
        })
        .catch(error => {
            console.error('Error al eliminar trabajador:', error);
            mostrarNotificacion('Error al eliminar trabajador', 'error');
        });
    });
    
    // Mostrar mensaje si existe desde TempData
    if (typeof tempDataMensaje !== 'undefined' && tempDataMensaje) {
        mostrarNotificacion(tempDataMensaje);
    }
});

// Función para cargar datos en el formulario de edición (fuera del DOMContentLoaded)
async function cargarDatosTrabajador(id) {
    try {
        const response = await fetch(`?handler=ObtenerTrabajador&id=${id}`, {
            headers: {
                'RequestVerificationToken': token
            }
        });
        const result = await response.json();

        if (result.success) {
            const trabajador = result.data;

            // Llenar los campos del formulario
            document.getElementById('editId').value = trabajador.id;
            document.getElementById('editTipoDocumento').value = trabajador.tipoDocumento || '';
            document.getElementById('editNumeroDocumento').value = trabajador.numeroDocumento || '';
            document.getElementById('editNombres').value = trabajador.nombres || '';
            document.getElementById('editSexo').value = trabajador.sexo || '';

            // Manejo de ubicación
            document.getElementById('editSelectDepartamento').value = trabajador.idDepartamento || '';

            // Si hay departamento seleccionado, cargar provincias
            if (trabajador.idDepartamento) {
                await cargarProvincias(trabajador.idDepartamento);
                document.getElementById('editSelectProvincia').value = trabajador.idProvincia || '';

                // Si hay provincia seleccionada, cargar distritos
                if (trabajador.idProvincia) {
                    await cargarDistritos(trabajador.idProvincia);
                    document.getElementById('editSelectDistrito').value = trabajador.idDistrito || '';
                }
            }

            const modalEditarTrabajador = new bootstrap.Modal(document.getElementById('modalEditarTrabajador'));
            modalEditarTrabajador.show();
        } else {
            mostrarNotificacion('Error: ' + result.message, 'error');
        }
    } catch (error) {
        console.error('Error al cargar datos del trabajador:', error);
        mostrarNotificacion('Error al cargar datos del trabajador', 'error');
    }
}

// Función auxiliar para cargar provincias (fuera del DOMContentLoaded)
async function cargarProvincias(departamentoId) {
    try {
        const editSelectProvincia = document.getElementById('editSelectProvincia');
        editSelectProvincia.innerHTML = '<option value="">-- Seleccione una provincia --</option>';

        const response = await fetch(`?handler=Provincias&departamentoId=${departamentoId}`, {
            headers: {
                'RequestVerificationToken': token
            }
        });
        const data = await response.json();

        data.forEach(provincia => {
            const option = document.createElement('option');
            option.value = provincia.id;
            option.textContent = provincia.nombreProvincia;
            editSelectProvincia.appendChild(option);
        });

        editSelectProvincia.disabled = false;
        return data;
    } catch (error) {
        console.error('Error al cargar provincias:', error);
        return [];
    }
}

// Función auxiliar para cargar distritos (fuera del DOMContentLoaded)
async function cargarDistritos(provinciaId) {
    try {
        const editSelectDistrito = document.getElementById('editSelectDistrito');
        editSelectDistrito.innerHTML = '<option value="">-- Seleccione un distrito --</option>';

        const response = await fetch(`?handler=Distritos&provinciaId=${provinciaId}`, {
            headers: {
                'RequestVerificationToken': token
            }
        });
        const data = await response.json();

        data.forEach(distrito => {
            const option = document.createElement('option');
            option.value = distrito.id;
            option.textContent = distrito.nombreDistrito;
            editSelectDistrito.appendChild(option);
        });

        editSelectDistrito.disabled = false;
        return data;
    } catch (error) {
        console.error('Error al cargar distritos:', error);
        return [];
    }
}

// Función para mostrar notificaciones toast
function mostrarNotificacion(mensaje, tipo = 'success') {
    const toast = document.getElementById('notificationToast');
    const toastMessage = document.getElementById('toastMessage');
    
    // Eliminar clases de color anteriores
    toast.classList.remove('bg-success', 'bg-danger', 'bg-warning', 'bg-info');
    
    // Establecer el mensaje
    toastMessage.textContent = mensaje;
    
    // Aplicar el estilo según el tipo
    switch (tipo) {
        case 'success':
            toast.classList.add('bg-success');
            break;
        case 'error':
            toast.classList.add('bg-danger');
            break;
        case 'warning':
            toast.classList.add('bg-warning');
            break;
        case 'info':
            toast.classList.add('bg-info');
            break;
    }
    
    // Mostrar la notificación
    const bsToast = new bootstrap.Toast(toast, { 
        autohide: true,
        delay: 3000 
    });
    bsToast.show();
}