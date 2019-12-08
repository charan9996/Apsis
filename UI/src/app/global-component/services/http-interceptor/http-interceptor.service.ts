import {
	HttpEvent,
	HttpEventType,
	HttpHandler,
	HttpHeaders,
	HttpInterceptor,
	HttpRequest,
	HttpErrorResponse
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { throwError } from 'rxjs';
import { Observable } from 'rxjs/Observable';
import { catchError, finalize, map } from 'rxjs/operators';
import { AuthService } from '../../authentication/auth.service';
import { UploadModalService } from 'src/app/ui-component/custom-ui-components/modals/upload-dialog/upload-modal.service';
import { SpinnerService } from 'src/app/ui-component/custom-ui-components/modals/spinner-new/spinner-modal.service';
import { SessionTimeoutDialogService } from 'src/app/ui-component/custom-ui-components/modals/session-timeout-dialog/session-timeout-dialog.service';

@Injectable()
export class HTTPListener implements HttpInterceptor {
	constructor(
		private authService: AuthService,
		private spinnerService: SpinnerService,
		private sessionTimeoutDialogService: SessionTimeoutDialogService
	) {}

	public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		this.spinnerService.openDialog();
		const authToken = this.authService.getAuthTokenFromStorage;
		const prevContentType = req.headers.get('Content-Type');
		let authReq = null;
		if (prevContentType) {
			authReq = req.clone({
				headers: new HttpHeaders({
					Authorization: `Bearer ${authToken}`,
					'Content-Type': prevContentType
				})
			});
		} else {
			authReq = req.clone({
				headers: new HttpHeaders({
					Authorization: `Bearer ${authToken}`
				})
			});
		}

		return next.handle(authReq).pipe(
			map(event => {
				console.log('intercepting' + req.urlWithParams + ' ' + req.method);
				return event;
			}),
			catchError(error => {
				const e = error as HttpErrorResponse;
				if (e.status === 401) {
					this.sessionTimeoutDialogService.openDialog();
					this.authService.login();
				}
				console.log('ERROR intercepting');
				console.log(error);
				this.spinnerService.closeDialog();
				return throwError(error);
			}),
			finalize(() => {
				this.spinnerService.closeDialog();
			})
		);
	}
}
