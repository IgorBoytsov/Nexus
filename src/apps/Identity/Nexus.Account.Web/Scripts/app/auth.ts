import { AuthApi } from "../entities/session/api/auth.api.js";

const sessionApi = new AuthApi();

document.addEventListener("DOMContentLoaded", async () => {
    try {
        await sessionApi.refreshSession();
        window.location.href = "/Home/Privacy";
    } catch (error) {
        console.error("Сессия мертва");
    }
});