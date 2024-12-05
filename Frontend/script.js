// script.js

document.addEventListener('DOMContentLoaded', () => {
    const apiBaseUrl = 'http://localhost:5276/api'; // Passe den Port und die URL ggf. an

    // DOM-Elemente
    const loginForm = document.getElementById('loginForm');
    const loginMessage = document.getElementById('loginMessage');
    const createOrderForm = document.getElementById('createOrderForm');
    const serviceIDSelect = document.getElementById('serviceID');
    const ordersTableBody = document.querySelector('#ordersTable tbody');
    const filterPrioritySelect = document.getElementById('filterPriority');
    const filterButton = document.getElementById('filterButton');
    const createOrderMessage = document.getElementById('createOrderMessage');
    const ordersMessage = document.getElementById('ordersMessage');

    // Initiale Funktionen
    fetchServices();
    fetchOrders();

    // Event-Listener für das Login-Formular
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        loginMessage.textContent = '';
        loginMessage.className = '';

        const loginData = {
            username: document.getElementById('username').value.trim(),
            password: document.getElementById('password').value.trim()
        };

        try {
            const response = await fetch(`${apiBaseUrl}/Login`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(loginData)
            });

            if (response.ok) {
                const data = await response.json();
                const token = data.token; // Angenommen, der JWT wird im Feld 'token' zurückgegeben
                const role = data.role;   // Angenommen, die Rolle des Benutzers wird zurückgegeben
                const username = data.username; // Angenommen, der Benutzername wird zurückgegeben

                // Speichere den JWT und die Benutzerrolle im LocalStorage
                localStorage.setItem('jwt', token);
                localStorage.setItem('role', role);
                localStorage.setItem('username', username);

                // Erfolgreicher Login-Meldung
                loginMessage.textContent = `Erfolgreich eingeloggt als ${username}!`;
                loginMessage.className = 'success';

                // Weiterleitung zum Dashboard nach kurzer Verzögerung
                setTimeout(() => {
                    window.location.href = 'dashboard.html';
                }, 1000);
            } else {
                const errorData = await response.json();
                loginMessage.textContent = `Fehler: ${errorData.message}`;
                loginMessage.className = 'error';
            }
        } catch (error) {
            console.error('Error:', error);
            loginMessage.textContent = 'Ein Fehler ist aufgetreten.';
            loginMessage.className = 'error';
        }
    });

    // Event-Listener für das Erstellen eines neuen Auftrags
    createOrderForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        createOrderMessage.textContent = '';
        createOrderMessage.className = '';

        // Optional: Prüfe, ob der Benutzer eingeloggt ist
        const jwt = localStorage.getItem('jwt');

        const orderData = {
            customerName: document.getElementById('customerName').value.trim(),
            email: document.getElementById('email').value.trim(),
            phone: document.getElementById('phone').value.trim(),
            priority: document.getElementById('priority').value,
            serviceID: parseInt(document.getElementById('serviceID').value)
        };

        try {
            const response = await fetch(`${apiBaseUrl}/orders`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    ...(jwt && { 'Authorization': `Bearer ${jwt}` }) // Sende JWT, wenn vorhanden
                },
                body: JSON.stringify(orderData)
            });

            if (response.ok) {
                const createdOrder = await response.json();
                createOrderMessage.textContent = 'Auftrag erfolgreich erstellt!';
                createOrderMessage.className = 'success';
                createOrderForm.reset();
                fetchOrders(); // Aktualisiere die Auftragsliste
            } else {
                const errorData = await response.json();
                createOrderMessage.textContent = `Fehler: ${errorData.message || errorData}`;
                createOrderMessage.className = 'error';
            }
        } catch (error) {
            console.error('Error:', error);
            createOrderMessage.textContent = 'Ein Fehler ist aufgetreten.';
            createOrderMessage.className = 'error';
        }
    });

    // Event-Listener für das Filtern von Aufträgen
    filterButton.addEventListener('click', () => {
        const priority = filterPrioritySelect.value;
        fetchOrders(priority);
    });

    // Funktion zum Abrufen von Dienstleistungen
    async function fetchServices() {
        try {
            const response = await fetch(`${apiBaseUrl}/services`);
            if (response.ok) {
                const services = await response.json();
                populateServicesDropdown(services);
            } else {
                console.error('Fehler beim Abrufen der Dienstleistungen.');
            }
        } catch (error) {
            console.error('Error:', error);
        }
    }

    // Funktion zum Befüllen des Dienstleistungen-Dropdowns
    function populateServicesDropdown(services) {
        services.forEach(service => {
            const option = document.createElement('option');
            option.value = service.serviceID;
            option.textContent = service.serviceName;
            serviceIDSelect.appendChild(option);
        });
    }

    // Funktion zum Abrufen von Aufträgen, optional gefiltert nach Priorität
    async function fetchOrders(priority = '') {
        ordersTableBody.innerHTML = '';
        ordersMessage.textContent = '';

        let url = `${apiBaseUrl}/orders`;
        if (priority) {
            url += `/priority/${encodeURIComponent(priority)}`;
        }

        try {
            const response = await fetch(url, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('jwt') || ''}`
                }
            });
            if (response.ok) {
                const orders = await response.json();
                if (orders.length === 0) {
                    ordersMessage.textContent = 'Keine Aufträge gefunden.';
                    ordersMessage.className = 'error';
                } else {
                    populateOrdersTable(orders);
                }
            } else if (response.status === 404) {
                ordersMessage.textContent = 'Keine Aufträge gefunden.';
                ordersMessage.className = 'error';
            } else {
                const errorData = await response.json();
                ordersMessage.textContent = `Fehler: ${errorData.message || errorData}`;
                ordersMessage.className = 'error';
            }
        } catch (error) {
            console.error('Error:', error);
            ordersMessage.textContent = 'Ein Fehler ist aufgetreten.';
            ordersMessage.className = 'error';
        }
    }

    // Funktion zum Befüllen der Auftrags-Tabelle
    function populateOrdersTable(orders) {
        orders.forEach(order => {
            const row = document.createElement('tr');

            row.innerHTML = `
                <td>${order.orderID}</td>
                <td>${order.customerName}</td>
                <td>${order.email}</td>
                <td>${order.phone}</td>
                <td>${order.priority}</td>
                <td>${order.serviceName}</td>
                <td>${order.status}</td>
                <td>${new Date(order.dateCreated).toLocaleString()}</td>
                <td>${order.dateModified ? new Date(order.dateModified).toLocaleString() : ''}</td>
            `;

            ordersTableBody.appendChild(row);
        });
    }
});