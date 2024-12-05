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
    const adminSection = document.querySelector('.admin');
    const employeesTableBody = document.querySelector('#employeesTable tbody');
    const employeesMessage = document.getElementById('employeesMessage');

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

                // Speichere den JWT im LocalStorage
                localStorage.setItem('jwt', token);

                // Speichere die Benutzerrolle
                localStorage.setItem('role', role);

                loginMessage.textContent = `Erfolgreich eingeloggt als ${data.username}!`;
                loginMessage.className = 'success';

                // Zeige Admin-Bereich, wenn der Benutzer ein Admin ist
                if (role === 'Admin') {
                    adminSection.style.display = 'block';
                    fetchEmployees();
                }

                // Öffne ein neues Fenster mit Benutzerinformationen
                const loginWindow = window.open('', 'LoggedIn', 'width=300,height=200');
                loginWindow.document.write(`
                    <h2>Erfolgreich eingeloggt!</h2>
                    <p>Benutzer: ${data.username}</p>
                    <p>Rolle: ${role}</p>
                    <button onclick="window.close()">Schließen</button>
                `);

                // Verstecke das Login-Formular nach dem Login
                loginForm.style.display = 'none';
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

        const jwt = localStorage.getItem('jwt');
        if (!jwt) {
            createOrderMessage.textContent = 'Bitte loggen Sie sich zuerst ein.';
            createOrderMessage.className = 'error';
            return;
        }

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
                    'Authorization': `Bearer ${jwt}`
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
            const response = await fetch(`${apiBaseUrl}/services`, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('jwt') || ''}`
                }
            });
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

    // Funktion zum Abrufen von Mitarbeitern (Admin-Funktion)
    async function fetchEmployees() {
        employeesTableBody.innerHTML = '';
        employeesMessage.textContent = '';

        const jwt = localStorage.getItem('jwt');
        if (!jwt) {
            employeesMessage.textContent = 'Bitte loggen Sie sich zuerst ein.';
            employeesMessage.className = 'error';
            return;
        }

        try {
            const response = await fetch(`${apiBaseUrl}/employees`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${jwt}`
                }
            });

            if (response.ok) {
                const employees = await response.json();
                if (employees.length === 0) {
                    employeesMessage.textContent = 'Keine Mitarbeiter gefunden.';
                    employeesMessage.className = 'error';
                } else {
                    populateEmployeesTable(employees);
                }
            } else {
                const errorData = await response.json();
                employeesMessage.textContent = `Fehler: ${errorData.message || errorData}`;
                employeesMessage.className = 'error';
            }
        } catch (error) {
            console.error('Error:', error);
            employeesMessage.textContent = 'Ein Fehler ist aufgetreten.';
            employeesMessage.className = 'error';
        }
    }

    // Funktion zum Befüllen der Mitarbeiter-Tabelle
    function populateEmployeesTable(employees) {
        employees.forEach(employee => {
            const row = document.createElement('tr');

            row.innerHTML = `
                <td>${employee.employeeID}</td>
                <td>${employee.username}</td>
                <td>${employee.isLocked ? 'Gesperrt' : 'Aktiv'}</td>
                <td>
                    ${employee.isLocked ? `<button onclick="unlockEmployee(${employee.employeeID})">Entsperren</button>` : '—'}
                </td>
            `;

            employeesTableBody.appendChild(row);
        });
    }

    // Funktion zum Entsperren eines Mitarbeiters (Admin-Funktion)
    window.unlockEmployee = async (employeeID) => {
        const jwt = localStorage.getItem('jwt');
        if (!jwt) {
            alert('Bitte loggen Sie sich zuerst ein.');
            return;
        }

        if (!confirm(`Möchten Sie Mitarbeiter ID ${employeeID} entsperren?`)) {
            return;
        }

        try {
            const response = await fetch(`${apiBaseUrl}/employees/unlock`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${jwt}`
                },
                body: JSON.stringify({ employeeID })
            });

            if (response.ok) {
                alert('Mitarbeiter erfolgreich entsperrt.');
                fetchEmployees(); // Aktualisiere die Mitarbeiterliste
            } else {
                const errorData = await response.json();
                alert(`Fehler: ${errorData.message || errorData}`);
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Ein Fehler ist aufgetreten.');
        }
    };
});
