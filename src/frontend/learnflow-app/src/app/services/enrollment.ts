import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Enrollment {
  id: string;
  studentId: string;
  courseId: string;
  courseTitleSnapshot: string;
  status: string;
  enrolledAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class EnrollmentService {
  private readonly baseUrl = environment.apiUrls;

  constructor(private http: HttpClient) {}

  enroll(studentId: string, courseId: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/api/enrollments`, { studentId, courseId });
  }

  getMyEnrollments(studentId: string): Observable<Enrollment[]> {
    return this.http.get<Enrollment[]>(`${this.baseUrl}/api/enrollments/my/${studentId}`);
  }
}