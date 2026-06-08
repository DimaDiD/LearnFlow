import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../services/auth';

interface Certificate {
  id: string;
  certificateNumber: string;
  studentId: string;
  courseId: string;
  courseTitleSnapshot: string;
  completionPercentage: number;
  issuedAt: string;
}

@Component({
  selector: 'app-certificates',
  standalone: true,
  imports: [
    NgFor,
    NgIf,
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './certificates.component.html',
  styleUrl: './certificates.component.scss'
})
export class CertificatesComponent implements OnInit {
  certificates: Certificate[] = [];
  loading = false;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCertificates();
  }

  loadCertificates(): void {
    const user = this.authService.getCurrentUser();
    if (!user) return;

    this.loading = true;
    this.http.get<Certificate[]>(
      `${environment.apiUrls}/api/certificates/my/${user.userId}`
    ).subscribe({
      next: (certs) => {
        this.certificates = certs;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }
}