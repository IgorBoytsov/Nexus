import { CommonModule } from "@angular/common";
import { Component, inject, OnInit, signal } from "@angular/core";
import { ProfileInfoComponent } from "../components/profile-info/profile-info.component";
import { ProfileHeaderComponent } from "../components/profile-header/profile-header.component";
import { LogoutService } from "../../../core/services/logout.service";
import { Result } from "@crossdyne/toolkit";
import { MapErrorsHelper } from "../../../core/helpers/map-errors.helper";
import { Router } from "@angular/router";
import { SettingsComponent } from "../components/settings/settings.component";
import { ProfileInfoService } from "../services/profile-info.service";
import { ProfileInfoResponse } from "../models/profile-info.response";
import { ProjectsComponent } from "../components/projects/projects.component";

@Component({
    selector: 'app-profile-page',
    templateUrl: './profile.page.html',
    styleUrls: ['./profile.page.scss'],
    standalone: true,
    imports: [CommonModule, ProfileInfoComponent, ProfileHeaderComponent, SettingsComponent, ProjectsComponent],
})
export class ProfilePage implements OnInit {

    private profileInfoService = inject(ProfileInfoService);
    private logoutService = inject(LogoutService);
    private router = inject(Router);

    activeTab = signal<'profile' | 'settings' | 'projects'>('profile');

    login = signal<string>('');
    userName = signal<string>('');
    email = signal<string>('');
    dateRegistration = signal<Date>(new Date());
    avatarUrl = signal<string>('');

   ngOnInit(): void {
        this.getProfileInfo();
    }

    //#region Выбор активной вкладки

    setActiveTab(tab: 'profile' | 'settings' | 'projects'): void {
        this.activeTab.set(tab);
    }

    //#endregion

    //#region Получение данных
   
    async getProfileInfo() {
        const result: Result<ProfileInfoResponse> = await this.profileInfoService.getProfileInfo();

        result.match(
            info => {
                this.login.set(info.login);
                this.userName.set(info.userName);
                this.email.set(info.email);
                this.dateRegistration.set(new Date(info.dateRegistration)); 
                this.avatarUrl.set(info.avatarUrl);
            },
            errors => console.error('Ошибка получение данных', MapErrorsHelper.mapErrors(errors))
        );
    }

    //#endregion

    //#region Выход из учетной записи

    async logout() {
        const result: Result<void> = await this.logoutService.logout();

        if (result.isFailure){
            console.error("Ошибка при выходе из учетной записи", MapErrorsHelper.mapErrors(result.errors))
            return;
        }

        this.router.navigate(['/login']);
    }

    //#endregion

}