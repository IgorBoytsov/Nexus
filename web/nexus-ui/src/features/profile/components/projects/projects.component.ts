import { Component, signal } from "@angular/core";

@Component({
    selector: 'projects',
    templateUrl: './projects.component.html',
    styleUrls: ['./projects.component.scss'],
    standalone: true
})
export class ProjectsComponent {
    isProjectsExpanded = signal<boolean>(false);

    toggleProjects(): void {
        this.isProjectsExpanded.update(value => !value);
    }
}