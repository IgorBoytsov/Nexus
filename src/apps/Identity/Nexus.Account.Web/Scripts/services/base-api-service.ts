export abstract class BaseApiService {
    protected constructor(protected readonly baseUrl: string) { }

    protected async post<T>(endpoint: string, data: any): Promise<T> {
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

        return response.status !== 204 ? await response.json() : (null as any);
    }
}