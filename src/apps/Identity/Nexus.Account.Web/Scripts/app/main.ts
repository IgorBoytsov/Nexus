import { AuthApi } from "../entities/session/api/auth.api.js";

const sessionApi = new AuthApi();

document.addEventListener("DOMContentLoaded", async () => {
    try {
        await sessionApi.refreshSession();
        console.log("Приложение готово, сессия обновлена");
    } catch (error) {
        console.error("Сессия мертва");
        window.location.href = "Account/Login";
    }
});