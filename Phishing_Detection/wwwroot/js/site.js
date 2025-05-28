"use strict";

document.addEventListener("DOMContentLoaded", function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    let notificationCount = 0;
    let notifications = [];

    function renderNotifications() {
        const notificationList = document.getElementById("notificationList");
        notificationList.innerHTML = "";

        if (notifications.length === 0) {
            notificationList.innerHTML = '<div class="p-3 text-center text-muted">No notifications</div>';
            return;
        }

        notifications.forEach(n => {
            const item = document.createElement("a");
            item.className = "notification-item d-flex align-items-center p-2 border-bottom text-decoration-none";
            item.href ="/Identity/Account/Manage/History"
            item.style.color = "inherit";
            item.style.cursor = "pointer";
            item.onmouseover = function () {
                item.style.backgroundColor = "#f8f9fa";
            };
            item.onmouseout = function () {
                item.style.backgroundColor = "transparent";
            };

            const imgSrc = n.message.includes("to 'Safe'")
                ? "/Images/icons8-correct-24.png"
                : "/Images/icons8-danger-24.png";

            item.innerHTML = `
            <img src="${imgSrc}" alt="Status Icon" class="me-2 rounded-circle" width="40"
                 onerror="this.onerror=null; this.src='/Images/default-icon.png';">
            <div class="flex-grow-1">
                <span>${n.message}</span>
                <br>
                <small class="text-muted">${new Date(n.createdAt).toLocaleString()}</small>
            </div>
        `;

            notificationList.prepend(item);
        });

        document.getElementById("notificationCount").textContent = notificationCount;
        document.getElementById("notificationCount").style.display = notificationCount > 0 ? "inline-block" : "none";
    }

    connection.on("ReceiveNotification", function (message, status) {
        console.log("Received notification:", message, "Status:", status);

        notificationCount++;
        notifications.push({
            message: message,
            status: status,
            createdAt: new Date()
        });

        renderNotifications();
        document.getElementById("notificationDropdown").style.display = "block";
    });

    connection.start()
        .then(() => {
            console.log("SignalR Connected Successfully");
        })
        .catch(err => console.error("SignalR Connection Failed: " + err.toString()));

    document.getElementById("notificationIcon").addEventListener("click", function (e) {
        e.preventDefault();
        const dropdown = document.getElementById("notificationDropdown");
        dropdown.style.display = dropdown.style.display === "block" ? "none" : "block";

        if (dropdown.style.display === "none") {
            notifications = [];
            notificationCount = 0;
            renderNotifications();
        }
    });

    document.getElementById("closeDropdown").addEventListener("click", function () {
        document.getElementById("notificationDropdown").style.display = "none";
        notifications = [];
        notificationCount = 0;
        renderNotifications();
    });
});
