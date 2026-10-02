import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { Dashboard } from './pages/dashboard/dashboard';
import { Screening } from './pages/screening/screening';
import { Candidates } from './pages/candidates/candidates';
import { CandidateDetail } from './pages/candidate-detail/candidate-detail';
import { Synchronization } from './pages/synchronization/synchronization';
import { Resumes } from './pages/resumes/resumes';
import { AuditLogs } from './pages/audit-logs/audit-logs';
import { AccessDenied } from './pages/access-denied/access-denied';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
  { path: 'screening', component: Screening, canActivate: [authGuard] },
  { path: 'candidates', component: Candidates, canActivate: [authGuard] },
  { path: 'candidates/:id', component: CandidateDetail, canActivate: [authGuard] },
  { path: 'synchronization', component: Synchronization, canActivate: [authGuard] },
  { path: 'resumes', component: Resumes, canActivate: [authGuard] },
  { path: 'audit-logs', component: AuditLogs, canActivate: [authGuard] },
  { path: 'access-denied', component: AccessDenied },
  { path: '**', redirectTo: 'dashboard' },
];
