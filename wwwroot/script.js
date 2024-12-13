const BASE_URL = "https://localhost:7226"; // An Ihre API anpassen

async function login(username, password) {
    try {
        const response = await fetch(`${BASE_URL}/api/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ Username: username, Password: password })
        });

        if (!response.ok) {
            return null;
        }

        const data = await response.json();
        return data.token;
    } catch (error) {
        console.error("Error during login:", error);
        return null;
    }
}

async function getUserInfo(jwtToken) {
    try {
        const response = await fetch(`${BASE_URL}/api/auth/me`, {
            headers: {
                'Authorization': `Bearer ${jwtToken}`
            }
        });
        if (response.ok) {
            return await response.json();
        } else {
            console.error("Fehler beim Laden der Benutzerinformationen");
            return null;
        }
    } catch (error) {
        console.error("Error loading user info:", error);
        return null;
    }
}

async function getEmployees(jwtToken) {
    try {
        const response = await fetch(`${BASE_URL}/api/auth/employees`, {
            headers: {
                'Authorization': `Bearer ${jwtToken}`
            }
        });
        if (response.ok) {
            return await response.json();
        } else {
            console.error("Fehler beim Laden der Mitarbeiterliste");
            return null;
        }
    } catch (error) {
        console.error("Error loading employees:", error);
        return null;
    }
}

async function unlockEmployeeRequest(jwtToken, employeeId) {
    try {
        const response = await fetch(`${BASE_URL}/api/auth/unlock/${employeeId}`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${jwtToken}`
            }
        });
        if (!response.ok) {
            const msg = await response.text();
            console.error("Fehler beim Entsperren des Mitarbeiters: ", msg);
            return false;
        }
        return true;
    } catch (error) {
        console.error("Error unlocking employee:", error);
        return false;
    }
}

// Im dashboard.html Skript-Teil oder script.js
window.unlockEmployee = async function(employeeId) {
    const success = await unlockEmployeeRequest(jwtToken, employeeId);
    if (success) {
        alert("Mitarbeiter erfolgreich entsperrt!");
        // Liste neu laden, um aktualisierten Status anzuzeigen
        loadEmployees();
    } else {
        alert("Fehler beim Entsperren!");
    }
}

async function getOrders(jwtToken, priority = "") {
    let url = `${BASE_URL}/api/orders`;
    if (priority) {
        url = `${BASE_URL}/api/orders/priority/${priority}`;
    }

    try {
        const headers = {};
        if (jwtToken) {
            headers['Authorization'] = `Bearer ${jwtToken}`;
        }

        const response = await fetch(url, { headers });
        if (!response.ok) {
            console.error("Fehler beim Laden der Orders");
            return null;
        }
        return await response.json();
    } catch (error) {
        console.error("Error loading orders:", error);
        return null;
    }
}

async function createOrder(jwtToken, {customerName, email, phone, priority, serviceId}) {
    try {
        const headers = {
            'Content-Type': 'application/json'
        };
        if (jwtToken) {
            headers['Authorization'] = `Bearer ${jwtToken}`;
        }

        const response = await fetch(`${BASE_URL}/api/orders`, {
            method: 'POST',
            headers,
            body: JSON.stringify({ CustomerName: customerName, Email: email, Phone: phone, Priority: priority, ServiceID: serviceId })
        });

        return response.ok;
    } catch (error) {
        console.error("Error creating order:", error);
        return false;
    }
}

async function updateOrderStatus(jwtToken, orderId, newStatus) {
    try {
        const response = await fetch(`${BASE_URL}/api/orders/${orderId}/status`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwtToken}`
            },
            body: JSON.stringify({ Status: newStatus })
        });
        return response.ok;
    } catch (error) {
        console.error("Error updating order status:", error);
        return false;
    }
}

async function deleteOrder(jwtToken, orderId) {
    try {
        const response = await fetch(`${BASE_URL}/api/orders/${orderId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${jwtToken}`
            }
        });
        return response.ok;
    } catch (error) {
        console.error("Error deleting order:", error);
        return false;
    }
}

// JWT dekodieren (einfach, ohne Validierung durch den Client, nur um Claims anzuzeigen)
function parseJwt (token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    } catch (e) {
        return {};
    }
}
