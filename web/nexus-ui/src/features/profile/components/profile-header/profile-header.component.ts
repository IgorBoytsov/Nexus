import { Component, computed, inject, input, signal } from "@angular/core";
import { ProfileInfoService } from "../../services/profile-info.service";
import { Result } from "@crossdyne/toolkit";
import { MapErrorsHelper } from "../../../../core/helpers/map-errors.helper";

@Component({
    selector: 'profile-header',
    templateUrl: './profile-header.component.html',
    styleUrls: ['./profile-header.component.scss'],
    standalone: true,
})
export class ProfileHeaderComponent {
    private profileInfoService = inject(ProfileInfoService);

    selectedFile = signal<File | null>(null);
    private objectUrl = signal<string | null>(null);

    nickName = input.required<string>();
    dateRegistration = input.required<Date>();
    avatarUrlInput = input<string | null>(null);

    avatarUrl = computed(() => {
        if (this.objectUrl()) {
            return this.objectUrl()!;
        }
        
        return this.avatarUrlInput() || '/assets/images/default-avatar.png';
    });

    registrationDuration = computed(() => {
        const regDate = this.dateRegistration();

        if (!regDate)
            return '0 лет 0 месяцев';

        const now = new Date();

        let years = now.getFullYear() - regDate.getFullYear();
        let months = now.getMonth() - regDate.getMonth();
        let days = now.getDate() - regDate.getDate();

        if (days < 0){
            months--;

            const prevMount = new Date(now.getFullYear(), now.getMonth(), 0);
            days += prevMount.getDate();
        }

        if (months < 0) {
            years--;
            months += 12;
        }

        if (years < 0) {
            years = 0;
            months = 0;
        }

        const yearWord = pluralize(years, ['год', 'года', 'лет']);
        const monthWord = pluralize(months, ['месяц', 'месяца', 'месяцев']);

        return `${years} ${yearWord} ${months} ${monthWord}`;
    });

    async onFileSelected(event: Event): Promise<void> {
        const input = event.target as HTMLInputElement;

        if (input.files && input.files.length > 0) {
            const file = input.files[0];
            this.selectedFile.set(file);

            this.revokeObjectUrl();

            const url = URL.createObjectURL(file);
            this.objectUrl.set(url);

            const result: Result<void> = await this.profileInfoService.changeAvatar(file);

            result.match(
                () => console.log('Аватарка успешно изменена'),
                errors => {
                    console.error(MapErrorsHelper.mapErrors(errors));

                    this.revokeObjectUrl();
                    this.selectedFile.set(null);
                }
            );
        }
    }

    onImageError(event: Event): void {
        const img = event.target as HTMLImageElement;
        const fallbackImage = '/assets/images/default-avatar.png';

        if (!img.src.includes('default-avatar.png')) {
            img.src = fallbackImage;
        }
    }

    private revokeObjectUrl(): void {
        const currentUrl = this.objectUrl();
        if (currentUrl) {
            URL.revokeObjectURL(currentUrl);
            this.objectUrl.set(null);
        }
    }

    ngOnDestroy(): void {
        this.revokeObjectUrl();
    }
}

function pluralize(count: number, forms: [string, string, string]): string {
    const absCount = Math.abs(count);
    const lastDigit = absCount % 10;
    const lastTwoDigits = absCount % 100;

    if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
        return forms[2];

    if (lastDigit === 1)
        return forms[0];

    if (lastDigit >= 2 && lastDigit <= 4)
        return forms[1];
    
    return forms[2];
}