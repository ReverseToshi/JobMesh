import { Routes } from '@angular/router';

const dashboardSectionRoutes: Routes = [
	{
		path: 'dashboard/queues',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'Queue monitoring',
				subtitle: 'Track queue depth, latency, and throughput in real time.',
				summary: 'This page should show live queue counts, sorting, filtering, and pause/resume actions for each queue.',
				highlights: [
					'Queue names',
					'Pending task count',
					'Processing count',
					'Dead-letter queue count',
					'Queue latency',
					'Average processing time',
					'Auto-refresh',
				],
				backendApis: [
					'GET /api/queues',
					'POST /api/queues/:name/pause',
					'POST /api/queues/:name/resume',
				],
				mvpPriorities: ['Queue depth', 'Latency', 'Pause/resume controls'],
				employerWins: ['Shows production-grade observability', 'Useful for distributed systems interviews'],
			},
		},
	},
	{
		path: 'dashboard/workers',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'Worker monitoring',
				subtitle: 'Inspect worker health, utilization, and active jobs.',
				summary: 'This page should surface online/offline status, resource usage, current task, and worker lifecycle actions.',
				highlights: ['Worker ID', 'Status', 'CPU usage', 'Memory usage', 'Current task', 'Tasks processed', 'Heartbeat timestamp'],
				backendApis: ['GET /api/workers', 'POST /api/workers/:id/restart', 'POST /api/workers/:id/disable'],
				mvpPriorities: ['Heartbeat tracking', 'Utilization metrics', 'Worker actions'],
				employerWins: ['Proves you can monitor distributed compute nodes', 'Highlights real operational control'],
			},
		},
	},
	{
		path: 'dashboard/submit',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'Task submission',
				subtitle: 'Create jobs, assign priority, and schedule execution.',
				summary: 'This page should contain the job creation form and any upload, cron, or batch task options.',
				highlights: ['Task type', 'Priority', 'Payload/data', 'Retry count', 'Scheduled execution', 'Timeout duration'],
				backendApis: ['POST /api/tasks', 'POST /api/tasks/batch', 'POST /api/tasks/upload'],
				mvpPriorities: ['Manual job creation', 'Priority selection', 'Retry/timeout controls'],
				employerWins: ['Shows full queue producer workflow', 'Connects UI to backend job orchestration'],
			},
		},
	},
	{
		path: 'dashboard/history',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'Task history',
				subtitle: 'Search and review completed, failed, and retrying jobs.',
				summary: 'This page should provide a searchable history table with filters and actions for each task.',
				highlights: ['Task ID', 'Status', 'Assigned worker', 'Created time', 'Start time', 'Completion time', 'Duration', 'Retry attempts'],
				backendApis: ['GET /api/tasks/history', 'GET /api/tasks/:id', 'POST /api/tasks/:id/retry'],
				mvpPriorities: ['Search', 'Status filters', 'Task detail drilldown'],
				employerWins: ['Demonstrates auditability and traceability', 'Useful for operational debugging'],
			},
		},
	},
	{
		path: 'dashboard/activity',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'Real-time activity feed',
				subtitle: 'Stream live events for workers, queues, and tasks.',
				summary: 'This page should show a live feed using SignalR, WebSockets, or Server-Sent Events.',
				highlights: ['Worker connected', 'Task started', 'Task completed', 'Task failed', 'Queue overflow', 'Retry triggered'],
				backendApis: ['GET /api/events/stream', 'GET /api/activity'],
				mvpPriorities: ['Live event stream', 'Event grouping', 'Alert-style updates'],
				employerWins: ['Makes the dashboard feel alive', 'Shows real-time system design thinking'],
			},
		},
	},
	{
		path: 'dashboard/logs',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'Logs and error monitoring',
				subtitle: 'Centralize logs, filter by service, and export failure details.',
				summary: 'This page should surface searchable logs with stack traces and log-level filters.',
				highlights: ['Timestamp', 'Service name', 'Log level', 'Message', 'Stack trace', 'Export logs'],
				backendApis: ['GET /api/logs', 'GET /api/logs/errors', 'POST /api/logs/export'],
				mvpPriorities: ['Error-only view', 'Search', 'Time range filters'],
				employerWins: ['Supports troubleshooting workflows', 'Demonstrates production support tooling'],
			},
		},
	},
	{
		path: 'dashboard/analytics',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'Analytics and charts',
				subtitle: 'Measure throughput, growth, utilization, and failures.',
				summary: 'This page should contain charts for completed tasks, queue growth, worker usage, and retry rate.',
				highlights: ['Tasks completed over time', 'Queue growth', 'Worker utilization', 'Failure rate', 'Average processing time', 'Retry frequency'],
				backendApis: ['GET /api/analytics/throughput', 'GET /api/analytics/failures', 'GET /api/analytics/workers'],
				mvpPriorities: ['Throughput graph', 'Worker utilization chart', 'Failure trend chart'],
				employerWins: ['Makes the project feel product-grade', 'Lets you tell a strong systems story'],
			},
		},
	},
	{
		path: 'dashboard/dlq',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'Dead-letter queue',
				subtitle: 'Review and recover permanently failed tasks.',
				summary: 'This page should show failure payloads, exception messages, retry history, and manual replay actions.',
				highlights: ['Failed payload', 'Failure reason', 'Exception message', 'Retry history', 'Retry manually', 'Delete task'],
				backendApis: ['GET /api/dlq', 'POST /api/dlq/:taskId/retry', 'DELETE /api/dlq/:taskId'],
				mvpPriorities: ['Failure listing', 'Manual retry', 'Delete/export actions'],
				employerWins: ['Shows you can design for failure', 'Adds a real production ops feature'],
			},
		},
	},
	{
		path: 'dashboard/scheduling',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'Scheduling system',
				subtitle: 'Run tasks later or on a recurring schedule.',
				summary: 'This page should handle cron expressions, recurring jobs, and delayed execution settings.',
				highlights: ['Run later', 'Recurring jobs', 'Cron expressions', 'Calendar scheduling'],
				backendApis: ['POST /api/schedules', 'GET /api/schedules', 'DELETE /api/schedules/:id'],
				mvpPriorities: ['Cron parser', 'One-off delayed jobs', 'Recurring jobs'],
				employerWins: ['Expands the system beyond basic queueing', 'Demonstrates scheduling architecture'],
			},
		},
	},
	{
		path: 'dashboard/users',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'User and role management',
				subtitle: 'Control access for admins, operators, and viewers.',
				summary: 'This page should support permissions, audit logs, and access control rules.',
				highlights: ['Admin', 'Operator', 'Viewer', 'User permissions', 'Audit logs', 'Access control'],
				backendApis: ['GET /api/users', 'POST /api/users', 'PUT /api/users/:id/roles'],
				mvpPriorities: ['Role list', 'Permission checks', 'Audit visibility'],
				employerWins: ['Shows secure product thinking', 'Connects UI to governance controls'],
			},
		},
	},
	{
		path: 'dashboard/alerts',
		loadComponent: () => import('./dashboard/section-page.component').then((m) => m.DashboardSectionPageComponent),
		data: {
			page: {
				title: 'Alerting system',
				subtitle: 'Surface worker, queue, and failure alerts before users notice problems.',
				summary: 'This page should configure and display alerts for offline workers, large queues, and high failure rates.',
				highlights: ['Worker offline', 'Queue too large', 'High failure rate', 'Slow processing', 'Email', 'Slack', 'Discord'],
				backendApis: ['GET /api/alerts', 'POST /api/alerts', 'PUT /api/alerts/:id'],
				mvpPriorities: ['Alert list', 'Thresholds', 'Notification delivery'],
				employerWins: ['Shows operational readiness', 'Useful for real-world incident response'],
			},
		},
	},
];

export const routes: Routes = [
	{ path: '', redirectTo: 'login', pathMatch: 'full' },
	{
		path: 'login',
		loadComponent: () => import('./login/login.component').then((m) => m.LoginComponent),
	},
	{
		path: 'register',
		loadComponent: () => import('./register/register.component').then((m) => m.RegisterComponent),
	},
	{
		path: 'dashboard',
		loadComponent: () => import('./dashboard/dashboard.component').then((m) => m.DashboardComponent),
	},
	...dashboardSectionRoutes,
];
