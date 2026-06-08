import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Progress, ProgressService } from '../../services/progress';
import { AuthService } from '../../services/auth';
import { Enrollment, EnrollmentService } from '../../services/enrollment';

interface CourseWithProgress {
  enrollment: Enrollment;
  progress: Progress | null;
}

@Component({
  selector: 'app-my-courses',
  standalone: true,
  imports: [
    NgFor,
    NgIf,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './my-courses.component.html',
  styleUrl: './my-courses.component.scss'
})
export class MyCoursesComponent implements OnInit {
  coursesWithProgress: CourseWithProgress[] = [];
  loading = false;

  constructor(
    private enrollmentService: EnrollmentService,
    private progressService: ProgressService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadMyCourses();
  }

  loadMyCourses(): void {
    const user = this.authService.getCurrentUser();
    if (!user) return;

    this.loading = true;
    this.enrollmentService.getMyEnrollments(user.userId).subscribe({
      next: (enrollments) => {
        this.coursesWithProgress = enrollments.map(e => ({ enrollment: e, progress: null }));
        this.loading = false;

        // Load progress for each enrollment
        enrollments.forEach((enrollment, index) => {
          this.progressService.getProgress(user.userId, enrollment.courseId).subscribe({
            next: (progress) => {
              this.coursesWithProgress[index].progress = progress;
            },
            error: () => {} // progress may not exist yet
          });
        });
      },
      error: () => this.loading = false
    });
  }
}