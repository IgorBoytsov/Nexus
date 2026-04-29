import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ProfileInfoComponent } from "../../features/profile/profile-info/profile-info.component";

@Component({
    selector: 'app-profile-page',
    standalone: true,
    imports: [CommonModule, ProfileInfoComponent],
    templateUrl: './profile.page.html',
})
export class ProfilePage{
    
}