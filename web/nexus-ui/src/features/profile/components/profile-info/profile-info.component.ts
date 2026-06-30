import { Component, input} from "@angular/core";
import { CommonModule } from "@angular/common";

@Component({
    selector: 'profile-info',
    standalone: true,
    templateUrl: './profile-info.component.html',
    styleUrls: ['./profile-info.component.scss'],
    imports: [CommonModule]
})
export class ProfileInfoComponent {
    login = input.required<string>();
    email = input.required<string>();
    dataRegistration = input.required<Date>();
}