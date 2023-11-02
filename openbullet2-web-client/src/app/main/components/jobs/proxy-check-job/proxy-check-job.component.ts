import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faAngleLeft, faCheck, faForward, faPause, faPen, faPlay, faStop, faX } from '@fortawesome/free-solid-svg-icons';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { JobStatus } from 'src/app/main/dtos/job/job-status';
import { ChangeBotsMessage } from 'src/app/main/dtos/job/messages/change-bots.dto';
import { PCJNewResultMessage } from 'src/app/main/dtos/job/messages/proxy-check/new-result.dto';
import { ProxyCheckJobDto } from 'src/app/main/dtos/job/proxy-check-job.dto';
import { StartConditionType } from 'src/app/main/dtos/job/start-condition.dto';
import { ProxyWorkingStatus } from 'src/app/main/enums/proxy-working-status';
import { getMockedProxyCheckJobNewResultMessage } from 'src/app/main/mock/messages.mock';
import { JobService } from 'src/app/main/services/job.service';
import { ProxyCheckJobHubService } from 'src/app/main/services/proxy-check-job.hub.service';

interface LogMessage {
  timestamp: Date;
  message: string;
  color: string;
}

@Component({
  selector: 'app-proxy-check-job',
  templateUrl: './proxy-check-job.component.html',
  styleUrls: ['./proxy-check-job.component.scss']
})
export class ProxyCheckJobComponent implements OnInit, OnDestroy {
  jobId: number | null = null;
  job: ProxyCheckJobDto | null = null;
  faAngleLeft = faAngleLeft;
  faPen = faPen;
  faPause = faPause;
  faPlay = faPlay;
  faStop = faStop;
  faForward = faForward;
  faX = faX;
  faCheck = faCheck;
  Math = Math;
  JobStatus = JobStatus;
  StartConditionType = StartConditionType;

  statusColor: Record<JobStatus, string> = {
    idle: 'secondary',
    waiting: 'accent',
    starting: 'good',
    running: 'good',
    pausing: 'custom',
    paused: 'custom',
    stopping: 'bad',
    resuming: 'good'
  };

  status: JobStatus = JobStatus.IDLE;
  bots: number = 0;
  tested: number = 0;
  working: number = 0;
  notWorking: number = 0;
  cpm: number = 0;
  elapsed: string = '00:00:00';
  remaining: string = '00:00:00';
  progress: number = 0;

  logsBufferSize: number = 10_000;
  logs: LogMessage[] = [];

  isChangingBots: boolean = false;
  desiredBots: number = 1;

  // Subscriptions
  resultSubscription: Subscription | null = null;
  tickSubscription: Subscription | null = null;
  statusSubscription: Subscription | null = null;
  botsSubscription: Subscription | null = null;
  taskErrorSubscription: Subscription | null = null;
  errorSubscription: Subscription | null = null;
  completedSubscription: Subscription | null = null;

  constructor(
    activatedRoute: ActivatedRoute,
    private router: Router,
    private jobService: JobService,
    private messageService: MessageService,
    private proxyCheckJobHubService: ProxyCheckJobHubService
  ) { 
    activatedRoute.url.subscribe(url => {
      this.jobId = parseInt(url[2].path);
    });
  }

  ngOnInit(): void {
    if (this.jobId === null) {
      this.router.navigate(['/jobs']);
      return;
    }

    // Mocked results, to use when debugging
    // setInterval(() => {
    //   this.onNewResult(getMockedProxyCheckJobNewResultMessage())
    // }, 50);

    this.proxyCheckJobHubService.createHubConnection(this.jobId);
    this.resultSubscription = this.proxyCheckJobHubService.result$
    .subscribe(result => {
      if (result !== null) {
        this.onNewResult(result);
      }
    });

    this.tickSubscription = this.proxyCheckJobHubService.tick$
    .subscribe(tick => {
      if (tick !== null) {
        this.tested = tick.tested;
        this.working = tick.working;
        this.notWorking = tick.notWorking;
        this.cpm = tick.cpm;
        this.elapsed = tick.elapsed;
        this.remaining = tick.remaining;
        this.progress = tick.progress;
      }
    });

    this.statusSubscription = this.proxyCheckJobHubService.status$
    .subscribe(status => {
      if (status !== null) {
        this.onStatusChanged(status.newStatus);
      }
    });

    this.botsSubscription = this.proxyCheckJobHubService.bots$
    .subscribe(bots => {
      if (bots !== null) {
        this.onBotsChanged(bots.newValue);
      }
    });

    this.taskErrorSubscription = this.proxyCheckJobHubService.taskError$
    .subscribe(error => {
      if (error !== null) {
        const logMessage = `Task error for proxy ${error.proxyHost}:${error.proxyPort}: ${error.errorMessage}`;

        this.writeLog({
          timestamp: new Date(),
          message: logMessage,
          color: 'var(--fg-bad)'
        });
      }
    });

    this.errorSubscription = this.proxyCheckJobHubService.error$
    .subscribe(error => {
      if (error !== null) {
        this.messageService.add({
          severity: 'error',
          summary: `Error - ${error.type}`,
          detail: error.message
        });
      }
    });

    this.completedSubscription = this.proxyCheckJobHubService.completed$
    .subscribe(completed => {
      if (completed) {
        this.messageService.add({
          severity: 'success',
          summary: 'Completed',
          detail: 'Job completed'
        });
      }
    });

    this.jobService.getProxyCheckJob(this.jobId)
      .subscribe(job => {
        this.status = job.status;
        this.bots = job.bots;
        this.tested = job.tested;
        this.working = job.working;
        this.notWorking = job.notWorking;
        this.cpm = job.cpm;
        this.elapsed = job.elapsed;
        this.remaining = job.remaining;
        this.progress = job.progress;

        this.job = job;
      });
  }

  ngOnDestroy(): void {
    this.proxyCheckJobHubService.stopHubConnection();

    this.resultSubscription?.unsubscribe();
    this.tickSubscription?.unsubscribe();
    this.statusSubscription?.unsubscribe();
    this.botsSubscription?.unsubscribe();
    this.taskErrorSubscription?.unsubscribe();
    this.errorSubscription?.unsubscribe();
    this.completedSubscription?.unsubscribe();
  }

  onNewResult(result: PCJNewResultMessage) {
    const logMessage = result.workingStatus === ProxyWorkingStatus.Working
      ? `Proxy ${result.proxyHost}:${result.proxyPort} is working with ping ${result.ping} ms and country ${result.country}`
      : `Proxy ${result.proxyHost}:${result.proxyPort} is not working`;

    this.writeLog({
      timestamp: new Date(),
      message: logMessage,
      color: result.workingStatus === ProxyWorkingStatus.Working ? 'var(--fg-good)' : 'var(--fg-bad)'
    });
  }

  onStatusChanged(status: JobStatus) {
    this.status = status;

    const logMessage = `Status changed to ${status}`;

    this.writeLog({
      timestamp: new Date(),
      message: logMessage,
      color: 'var(--fg-primary)'
    });
  }

  onBotsChanged(bots: number) {
    this.bots = bots;

    const logMessage = `Bots changed to ${bots}`;

    this.writeLog({
      timestamp: new Date(),
      message: logMessage,
      color: 'var(--fg-primary)'
    });
  }

  canEdit() {
    return this.status === JobStatus.IDLE;
  }

  editSettings() {
    this.router.navigate(
      [`/job/proxy-check/edit`], 
      { queryParams: { jobId: this.jobId } }
    );
  }

  backToJobs() {
    this.router.navigate(['/jobs']);
  }

  canPause() {
    return this.status === JobStatus.RUNNING;
  }

  pause() {
    this.proxyCheckJobHubService.pause();
  }

  canStart() {
    return this.status === JobStatus.IDLE;
  }

  start() {
    this.proxyCheckJobHubService.start();
  }

  canStop() {
    return this.status === JobStatus.RUNNING || this.status === JobStatus.PAUSED;
  }

  stop() {
    this.proxyCheckJobHubService.stop();
  }

  canResume() {
    return this.status === JobStatus.PAUSED;
  }

  resume() {
    this.proxyCheckJobHubService.resume();
  }

  canAbort() {
    return this.status === JobStatus.RUNNING ||
      this.status === JobStatus.PAUSED ||
      this.status === JobStatus.PAUSING ||
      this.status === JobStatus.STOPPING;
  }

  abort() {
    this.proxyCheckJobHubService.abort();
  }

  canSkipWait() {
    return this.status === JobStatus.WAITING;
  }

  skipWait() {
    this.proxyCheckJobHubService.skipWait();
  }

  showEditBotsInput() {
    this.desiredBots = this.bots;
    this.isChangingBots = true;
  }

  changeBots(bots: number) {
    this.proxyCheckJobHubService.changeBots(
      <ChangeBotsMessage>{ desired: bots }
    );

    const logMessage = `Requested to change bots to ${bots}`;

    this.writeLog({
      timestamp: new Date(),
      message: logMessage,
      color: 'var(--fg-primary)'
    });

    // If we decrease the bots while the job is running, it
    // might take some time
    const slow = this.bots > bots && this.status === JobStatus.RUNNING;

    this.messageService.add({
      severity: 'info',
      summary: 'Requested',
      detail: `Requested to change bots to ${bots}`
        + (slow ? '. This might take some time' : '')
    });

    this.isChangingBots = false;
    this.bots = bots;
  }

  writeLog(message: LogMessage) {
    // If there are more than N logs, shift the array
    // to always keep up to N logs
    if (this.logs.length > this.logsBufferSize) {
      this.logs.pop();
    }

    this.logs.unshift(message);
  }
}
