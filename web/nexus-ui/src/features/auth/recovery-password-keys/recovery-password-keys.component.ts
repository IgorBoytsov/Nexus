import { Component, inject, signal } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { RecoveryStateService } from "./services/recovery-password-keys-state.service";

@Component({
    selector: 'app-recovery-password-keys',
    templateUrl: './recovery-password-keys.component.html',
    styleUrls: ['./recovery-password-keys.component.scss'],
    standalone: true,
    // providers: [RecoveryStateService],
    imports: [RouterOutlet]
})
export class RecoveryPasswordKeysComponent {

}