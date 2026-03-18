import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="app-shell">
      <nav class="sidebar">
        <div class="logo">DEGEN</div>
        <a routerLink="/dashboard" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">📊</span>
          <span class="nav-label">Dashboard</span>
        </a>
        <a routerLink="/trading" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">📈</span>
          <span class="nav-label">Trading</span>
        </a>
        <a routerLink="/portfolio" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">💼</span>
          <span class="nav-label">Portfolio</span>
        </a>
        <a routerLink="/signals" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">⚡</span>
          <span class="nav-label">Signals</span>
        </a>
      </nav>
      <main class="content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [
    `
      .app-shell {
        display: flex;
        height: 100vh;
        background: #0a0a1a;
        color: #e0e0e0;
      }

      .sidebar {
        width: 200px;
        background: #0f0f23;
        border-right: 1px solid #1a1a3e;
        display: flex;
        flex-direction: column;
        padding: 0;
        flex-shrink: 0;
      }

      .logo {
        padding: 20px;
        font-size: 20px;
        font-weight: 700;
        letter-spacing: 3px;
        color: #00d4aa;
        border-bottom: 1px solid #1a1a3e;
      }

      .nav-item {
        display: flex;
        align-items: center;
        gap: 10px;
        padding: 14px 20px;
        color: #8888aa;
        text-decoration: none;
        font-size: 14px;
        transition: all 0.15s ease;
        border-left: 3px solid transparent;
      }

      .nav-item:hover {
        color: #c0c0d0;
        background: #12122a;
      }

      .nav-item.active {
        color: #00d4aa;
        background: #0d0d28;
        border-left-color: #00d4aa;
      }

      .nav-icon {
        font-size: 16px;
        width: 20px;
        text-align: center;
      }

      .content {
        flex: 1;
        overflow: auto;
        padding: 0;
      }
    `,
  ],
})
export class AppComponent {}
