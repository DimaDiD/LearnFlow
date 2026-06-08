import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Progress {
  id: string;
  studentId: string;
  courseId: string;
  status: string;
  completionPercentage: number;
  totalLessons: number;
  completedLessonsCount: number;
  moduleProgresses: ModuleProgress[];
}

export interface ModuleProgress {
  moduleId: string;
  totalLessons: number;
  completedLessonsCount: number;
  isCompleted: boolean;
  lessonProgresses: LessonProgress[];
}

export interface LessonProgress {
  lessonId: string;
  isCompleted: boolean;
  completedAt: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class ProgressService {
  private readonly baseUrl = environment.apiUrls;

  constructor(private http: HttpClient) {}

  getProgress(studentId: string, courseId: string): Observable<Progress> {
    return this.http.get<Progress>(`${this.baseUrl}/api/progress/${studentId}/${courseId}`);
  }

  markLessonComplete(studentId: string, courseId: string, moduleId: string, lessonId: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/api/progress/lessons/complete`, {
      studentId,
      courseId,
      moduleId,
      lessonId
    });
  }
}