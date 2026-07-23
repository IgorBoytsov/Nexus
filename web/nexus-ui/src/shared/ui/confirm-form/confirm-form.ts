import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, inject, signal } from '@angular/core';
import { ConfirmFormResult } from './model/confirm-form.result';
import { ConfirmFormData } from './model/confirm-form.data';

@Component({
  selector: 'confirm-form',
  imports: [],
  templateUrl: './confirm-form.html',
  styleUrl: './confirm-form.scss',
})
export class ConfirmFormComponent {
    private dialogRef = inject(DialogRef<ConfirmFormResult>);
    data = inject(DIALOG_DATA) as ConfirmFormData;

    title = signal<string>(this.data.title);
    message = signal<string>(this.data.body);

    confirm(status: 'ok' | 'no') {
      const result: ConfirmFormResult = {
        status: status
      } 

      this.dialogRef.close(result);
    }
}
