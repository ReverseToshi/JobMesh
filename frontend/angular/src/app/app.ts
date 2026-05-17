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
  protected showAuthPages = false;

  constructor(private router: Router) {
    // set initial state
    this.showAuthPages =
      this.router.url.startsWith('/login') || this.router.url.startsWith('/register');

    // update on navigation
    this.router.events.pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd)).subscribe((ev) => {
      this.showAuthPages =
        ev.urlAfterRedirects.startsWith('/login') || ev.urlAfterRedirects.startsWith('/register');
    });
  }
}
