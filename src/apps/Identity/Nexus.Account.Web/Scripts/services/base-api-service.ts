export abstract class BaseApiService {
    protected constructor(protected readonly baseUrl: string) { }

    protected async post<T>(endpoint: string, data: any): Promise<T> {

        const cleanBase = this.baseUrl.replace(/\/$/, '');
        const cleanEndpoint = endpoint.replace(/^\//, '');
        const url = `${cleanBase}/${cleanEndpoint}`;

        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        return this.handleResponse<T>(response);
    }

    protected async get<T>(endpoint: string, params?: Record<string, any>): Promise<T> {

        const cleanBase = this.baseUrl.replace(/\/$/, '');
        const cleanEndpoint = endpoint.replace(/^\//, '');
        let url = `${cleanBase}/${cleanEndpoint}`;

        if (params) {
            const searchParams = new URLSearchParams();
            Object.entries(params).forEach(([key, value]) => {
                if (value !== undefined && value !== null) {
                    searchParams.append(key, value.toString());
                }
            });
            const queryString = searchParams.toString();
            if (queryString) {
                url += `?${queryString}`;
            }
        }

        const response = await fetch(url, {
            method: 'GET',
            headers: { 'Accept': 'application/json' }
        });

        return this.handleResponse<T>(response);
    }

    private async handleResponse<T>(response: Response): Promise<T> {
        const responseText = await response.text();

        if (!response.ok) {
            let errorMessage = `Ошибка сервера: ${response.status}`;

            try {

                const errorData = JSON.parse(responseText);

                errorMessage = errorData.Detail || errorData.title || errorData.Message || errorMessage;

                if (errorData.Errors && Array.isArray(errorData.Errors)) {
                    errorMessage += ": " + errorData.Errors.map((err: any) => err.msg || err).join(", ");
                }
            } catch {

                if (responseText) errorMessage = responseText;
            }
            throw new Error(errorMessage);
        }


        if (response.status === 204 || !responseText) {
            return null as any;
        }

        return JSON.parse(responseText) as T;
    }
}