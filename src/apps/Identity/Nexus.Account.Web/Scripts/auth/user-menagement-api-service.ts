export interface RegisterRequest {
    Login: string,
    UserName: string,
    Password: string,
    ClientSalt: string,
    EncryptedDek: string,
    Email: string,
    Phone: string,
    IdGender: string,
    IdCountry: string
}

export class UserMenegementApiService {
    private readonly baseUrl = "http://localhost:5017/api/users";

    async register(data: RegisterRequest): Promise<void> {
        return await this.post<void>("", data);
    }

    private async post<T>(endpoint: string, data: any): Promise<T> {
        const response = await fetch(`${this.baseUrl}${endpoint}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            let errorMessage = `Ошибка сервера: ${response.status}`;
            try {
                const errorData = await response.json();
                errorMessage = errorData.Detail || errorData.title || errorData.Message || errorMessage;

                if (errorData.Errors && Array.isArray(errorData.Errors)) {
                    errorMessage += ": " + errorData.Errors.map((err: any) => err.msg || err).join(", ");
                }
            } catch {
                const text = await response.text();
                if (text) errorMessage = text;
            }
            throw new Error(errorMessage);
        }

        return response.status !== 204 ? await response.json() : null;
    }
}