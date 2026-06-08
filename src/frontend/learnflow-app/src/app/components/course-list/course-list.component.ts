import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SearchCourseItem, CourseService } from '../../services/course';
import { NgFor, NgIf, SlicePipe } from '@angular/common';

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [
RouterLink,
    FormsModule,
    NgFor,
    NgIf,
    SlicePipe,
    MatCardModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './course-list.component.html',
  styleUrl: './course-list.component.scss'
})
export class CourseListComponent implements OnInit {
  courses: SearchCourseItem[] = [];
  loading = false;
  searchQuery = '';
  selectedCategory = '';

  categories = ['Programming', 'Database', 'DevOps', 'Design'];

  constructor(private courseService: CourseService) {}

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses(): void {
    this.loading = true;
    this.courseService.searchCourses(
      this.searchQuery || undefined,
      this.selectedCategory || undefined
    ).subscribe({
      next: (result: any) => {
        this.courses = result.items;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  onSearch(): void {
    this.loadCourses();
  }

  selectCategory(category: string): void {
    this.selectedCategory = this.selectedCategory === category ? '' : category;
    this.loadCourses();
  }
}