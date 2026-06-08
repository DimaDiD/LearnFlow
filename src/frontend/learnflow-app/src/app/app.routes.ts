import { LoginComponent } from './components/login/login.component';
import { Routes } from '@angular/router';
import { authGuard } from './guards/auth-guard';
import { CertificatesComponent } from './components/certificates/certificates.component';
import { CourseDetailComponent } from './components/course-detail/course-detail.component';
import { CourseListComponent } from './components/course-list/course-list.component';
import { MyCoursesComponent } from './components/my-courses/my-courses.component';
import { RegisterComponent } from './components/register/register.component';


export const routes: Routes = [
  { path: '', redirectTo: '/courses', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'courses', component: CourseListComponent },
  { path: 'courses/:id', component: CourseDetailComponent },
  {
    path: 'my-courses',
    component: MyCoursesComponent,
    canActivate: [authGuard]
  },
  {
    path: 'certificates',
    component: CertificatesComponent,
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '/courses' }
];