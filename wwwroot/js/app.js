// app.js

// Funktion zum Anzeigen von Bootstrap-Alerts
function showAlert(message, type = 'success') {
    const alertContainer = document.getElementById('alertContainer');
    if (alertContainer) {
        alertContainer.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;
    }
}

// Login-Formular-Handler
document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    const getDataButton = document.getElementById('getDataButton');
    const logoutButton = document.getElementById('logoutButton');

    if (loginForm) {
        loginForm.addEventListener('submit', async function (event) {
            event.preventDefault(); // Verhindert das Standard-Formularverhalten

            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;

            try {
                const response = await fetch('/api/auth/login', { // Relative URL, da Frontend vom Backend bedient wird
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ username, password })
                });

                if (response.ok) {
                    const data = await response.json();
                    const token = data.Token;

                    // Token sicher speichern (z.B. im localStorage)
                    localStorage.setItem('jwtToken', token);

                    showAlert('Erfolgreich eingeloggt!', 'success');
                    // Weiterleitung zur geschützten Seite
                    setTimeout(() => {
                        window.location.href = 'protected.html';
                    }, 1500);
                } else {
                    const errorData = await response.json();
                    showAlert(`Login fehlgeschlagen: ${errorData.message}`, 'danger');
                }
            } catch (error) {
                console.error('Fehler beim Login:', error);
                showAlert('Ein Fehler ist aufgetreten. Bitte versuche es erneut.', 'danger');
            }
        });
    }

    if (getDataButton) {
        getDataButton.addEventListener('click', async function () {
            const token = localStorage.getItem('jwtToken');
            if (!token) {
                showAlert('Bitte zuerst einloggen.', 'warning');
                return;
            }

            try {
                const response = await fetch('/api/protected/secretdata', { // Relative URL
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

                if (response.ok) {
                    const data = await response.json();
                    document.getElementById('dataDisplay').textContent = JSON.stringify(data, null, 2);
                    showAlert('Daten erfolgreich abgerufen!', 'success');
                } else if (response.status === 401) {
                    showAlert('Nicht autorisiert. Bitte einloggen.', 'danger');
                } else {
                    const errorData = await response.json();
                    showAlert(`Fehler: ${errorData.message}`, 'danger');
                }
            } catch (error) {
                console.error('Fehler beim Abrufen der Daten:', error);
                showAlert('Ein Fehler ist aufgetreten. Bitte versuche es erneut.', 'danger');
            }
        });
    }

    if (logoutButton) {
        logoutButton.addEventListener('click', function () {
            localStorage.removeItem('jwtToken');
            showAlert('Erfolgreich ausgeloggt.', 'info');
            setTimeout(() => {
                window.location.href = 'index.html';
            }, 1000);
        });
    }
});