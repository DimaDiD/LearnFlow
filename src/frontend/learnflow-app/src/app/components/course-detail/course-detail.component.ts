import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgFor, NgIf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../services/auth';
import { CourseDetail, CourseService } from '../../services/course';
import { EnrollmentService } from '../../services/enrollment';


@Component({
  selector: 'app-course-detail',
  standalone: true,
  imports: [
    NgFor,
    NgIf,
    MatCardModule,
    MatButtonModule,
    MatExpansionModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './course-detail.component.html',
  styleUrl: './course-detail.component.scss'
})
export class CourseDetailComponent implements OnInit {
  course: CourseDetail | null = null;
  loading = false;
  enrolling = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private courseService: CourseService,
    private enrollmentService: EnrollmentService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadCourse(id);
  }

  loadCourse(id: string): void {
    this.loading = true;
    this.courseService.getCourseById(id).subscribe({
      next: (course) => {
        this.course = course;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  enroll(): void {
    const user = this.authService.getCurrentUser();
    if (!user) {
      this.router.navigate(['/login']);
      return;
    }

    if (!this.course) return;

    this.enrolling = true;
    this.enrollmentService.enroll(user.userId, this.course.id).subscribe({
      next: () => {
        this.snackBar.open('Successfully enrolled!', 'Close', { duration: 3000 });
        this.router.navigate(['/my-courses']);
      },
      error: (err: any) => {
        this.snackBar.open(err.error?.message || 'Enrollment failed', 'Close', { duration: 3000 });
        this.enrolling = false;
      }
    });
  }
}