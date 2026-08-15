import { Component, inject, ViewEncapsulation } from "@angular/core";
import { Router, RouterOutlet } from "@angular/router";
import { LogoutService } from "../../services/logout.service";
import { ErrorList, Result } from "@crossdyne/toolkit";
import { MapErrorsHelper } from "../../helpers/map-errors.helper";

@Component({
    selector: 'app-main-layout',
    templateUrl: './main-layout.component.html',
    styleUrl: './main-layout.component.scss',
    standalone: true,
    encapsulation: ViewEncapsulation.None,
    imports: [RouterOutlet]
})
export class MainLayoutComponent{
    private http = inject(LogoutService);
    private router = inject(Router);

    async logout() {
        const result: Result<void> = await this.http.logout();

        if (result.isFailure){
            console.error('Ошибка загрузки категорий:', MapErrorsHelper.mapErrors(result.errors));
            return;
        }

        this.router.navigate(['/login']);
    }
}