import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface CourseListItem {
  id: string;
  title: string;
  category: string;
  level: string;
  price: number;
  instructorId: string;
  status: string;
  totalLessons: number;
  totalDurationMinutes: number;
  publishedAt: string;
}

export interface CourseDetail {
  id: string;
  title: string;
  description: string;
  instructorId: string;
  category: string;
  level: string;
  tags: string[];
  price: number;
  status: string;
  modules: Module[];
  totalLessons: number;
  totalDurationMinutes: number;
}

export interface Module {
  id: string;
  title: string;
  description: string;
  order: number;
  lessons: Lesson[];
}

export interface Lesson {
  id: string;
  title: string;
  description: string;
  videoUrl: string;
  durationMinutes: number;
  order: number;
}

export interface SearchResult {
  items: SearchCourseItem[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface SearchCourseItem {
  id: string;
  title: string;
  description: string;
  category: string;
  level: string;
  tags: string[];
  instructorId: string;
  instructorName: string;
  price: number;
  enrollmentCount: number;
  publishedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private readonly coursesUrl = environment.apiUrls;
  private readonly searchUrl = environment.apiUrls.search;

  constructor(private http: HttpClient) {}

  getCourses(page = 1, pageSize = 10): Observable<any> {
    return this.http.get(`${this.coursesUrl}/api/courses?page=${page}&pageSize=${pageSize}`);
  }

  getCourseById(id: string): Observable<CourseDetail> {
    return this.http.get<CourseDetail>(`${this.coursesUrl}/api/courses/${id}`);
  }

  searchCourses(query?: string, category?: string, page = 1): Observable<SearchResult> {
    let url = `${this.searchUrl}/api/search?page=${page}&pageSize=10`;
    if (query) url += `&q=${query}`;
    if (category) url += `&category=${category}`;
    return this.http.get<SearchResult>(url);
  }
}