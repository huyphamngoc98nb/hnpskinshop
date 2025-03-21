import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-test-error',
  standalone: true,
  imports: [
    MatButton
  ],
  templateUrl: './test-error.component.html',
  styleUrl: './test-error.component.scss'
})
export class TestErrorComponent {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  validationErrors?: string[];
  get404Error() {
    this.http.get(this.baseUrl + 'bug/notfound').subscribe({
      next: respose => console.log(respose),
      error: error => console.log(error)
    })
  }

  get400Error() {
    this.http.get(this.baseUrl + 'bug/badrequest').subscribe({
      next: respose => console.log(respose),
      error: error => console.log(error)
    })
  }

  get401Error() {
    this.http.get(this.baseUrl + 'bug/unauthorized').subscribe({
      next: respose => console.log(respose),
      error: error => console.log(error)
    })
  }

  get500Error() {
    this.http.get(this.baseUrl + 'bug/internalerror').subscribe({
      next: respose => console.log(respose),
      error: error => console.log(error)
    })
  }

  get400ValidationError() {
    this.http.post(this.baseUrl + 'bug/validationerror', {}).subscribe({
      next: respose => console.log(respose),
      error: error => this.validationErrors = error
    })
  }
}
