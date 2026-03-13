import { AuthApi } from "../entities/session/api/auth.api.js";

const config = (window as any).AppConfig;

const sessionApi = new AuthApi(config.baseBFF);

document.addEventListener("DOMContentLoaded", async () => {
    try {
        await sessionApi.refreshSession();
        window.location.href = "/Home/Privacy";
    } catch (error) {
        console.error("Сессия мертва");
    }
});