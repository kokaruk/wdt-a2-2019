import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpHelperService {
  private readonly baseURL = 'https://staging.kokaruk.com/asdasfdasdygiuyg';

  constructor(private http: HttpClient) {
  }

  get<T>(url: string): Observable<T> {
    return this.http.get<T>(url);
  }

  post<T>(url: string, data: any): Observable<T> {
    return this.http.post<T>(url, data);
  }

  put<T>(url: string, data: any) {
    return this.http.put<T>(url, data);
  }

  delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(url);
  }

  buildURL(endpoint: string, id?: string): string {
    const apiEndpoint = `${this.baseURL}/api/${endpoint}`;
    return id ? `${apiEndpoint}/${id}` : apiEndpoint;
  }

  private furnishHeaders(): any {
    // TODO for tokenization service
  }
}
