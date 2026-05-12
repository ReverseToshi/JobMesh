import { Component, signal } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('JobMesh');
  protected showLogin = false;

  constructor(private router: Router) {
    // set initial state
    this.showLogin = this.router.url.startsWith('/login');

    // update on navigation
    this.router.events.pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd)).subscribe((ev) => {
      this.showLogin = ev.urlAfterRedirects.startsWith('/login');
    });
  }
}
