import { ChangeDetectorRef, Component, inject, OnInit } from "@angular/core";
import { ProfileInfoApi } from "./profile-info.api";
import { FormBuilder } from "@angular/forms";
import { firstValueFrom } from "rxjs";
import { CommonModule } from "@angular/common";

@Component({
    selector: 'app-profile-info',
    standalone: true,
    templateUrl: './profile-info.component.html',
    styleUrls: ['./profile-info.component.scss'],
    imports: [CommonModule]
})
export class ProfileInfoComponent implements OnInit {
    private fb = inject(FormBuilder);
    private profileApi = inject(ProfileInfoApi);
    private cdr = inject(ChangeDetectorRef);

    login: string | null = null;
    email: string | null = null;
    phoneNumber: string | null = null;
    isLoaded = false;

    ngOnInit(): void{
        this.onSubmit();
    }

     onSubmit(): void {
        const info = this.profileApi.getProfileInfo().subscribe({
            next: (info) => {
                this.login = info.login;
                this.email = info.email;
                this.phoneNumber = info.phonNumber;
                
                this.isLoaded = true;
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('Error loading profile:', err);
            }
        });
    }
}