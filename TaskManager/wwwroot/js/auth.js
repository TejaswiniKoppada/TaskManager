// Function to retrieve JWT token from localStorage
function getAuthToken() {
    return localStorage.getItem("jwtToken");
}

// Function to make authenticated API requests
async function fetchTasks() {
    const token = getAuthToken();

    if (!token) {
        window.location.href = "/Auth/Login"; // Redirect to login if no token
        return;
    }

    try {
        const response = await fetch("http://localhost:5279/api/todo", {
            method: "GET",
            headers: {
                "Authorization": "Bearer " + token, // Attach JWT token
                "Content-Type": "application/json"
            }
        });

        if (response.status === 401) {
            alert("Session expired. Please log in again.");
            logout();
            return;
        }

        const tasks = await response.json();
        console.log(tasks); // Handle the tasks in UI
    } catch (error) {
        console.error("Error fetching tasks:", error);
    }
}

// Logout function
function logout() {
    localStorage.removeItem("jwtToken"); // Remove JWT token
    window.location.href = "/Auth/Login"; // Redirect to login page
}

// Attach logout to button (if exists)
document.addEventListener("DOMContentLoaded", function () {
    const logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", logout);
    }

    fetchTasks(); // Fetch tasks when the page loads
});
