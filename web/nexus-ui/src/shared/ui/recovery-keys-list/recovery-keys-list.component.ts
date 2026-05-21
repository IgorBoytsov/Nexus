import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output, signal, ViewEncapsulation } from "@angular/core";

@Component({
    selector: 'recovery-keys-list',
    templateUrl: './recovery-keys-list.component.html',
    styleUrls: ['./recovery-keys-list.component.scss'],
    standalone: true,
    imports: [CommonModule],
})
export class RecoveryKeysListComponent{
    @Input() recoveryKeys: string[] | null = null;
    @Output() confirmed = new EventEmitter<void>();

    copiedIndex = signal<number | null>(null);
    isSaved = signal(false);

    async copyKey(key: string, index: number): Promise<void> {
        try {
            await navigator.clipboard.writeText(key);
            this.copiedIndex.set(index);
            setTimeout(() => this.copiedIndex.set(null), 2000);
        } catch (error) {
            console.error('Не удалось скопировать ключ', error);
        }
    }

    async copyAll(): Promise<void> {
        if (!this.recoveryKeys?.length) 
            return;

         try {
            await navigator.clipboard.writeText(this.recoveryKeys!.join('\n'));
        } catch (error) { 
            console.error('Не удалось скопировать все ключи:', error); 
        }
    }

    onCheckboxChange(event: Event): void {
        const target = event.target as HTMLInputElement;
        this.isSaved.set(target.checked);
    }

    onConfirmSaved(): void {
        this.confirmed.emit();
    }
}