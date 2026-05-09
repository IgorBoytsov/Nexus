import { HttpErrorResponse } from '@angular/common/http';
import { AppError, ErrorCode, Result } from '@crossdyne/toolkit';

export class ResultHttpMapper {

  static fromHttpError<T>(error: HttpErrorResponse): Result<T> {
    if (Array.isArray(error.error)) {
      const errors = error.error
        .filter((item: any) => item?.code?.name && item?.code?.code && item?.message)
        .map((item: any) => AppError.fromJSON(item));
      
      if (errors.length > 0) {
        return Result.failure(errors) as Result<T>;
      }
    }
    
    const fallback = new AppError(
      ErrorCode.Server,
      error.message || `HTTP ${error.status}: ${error.statusText}`
    );
    return Result.failure(fallback) as Result<T>;
  }

  static handleError<T>(): (error: HttpErrorResponse) => Result<T> {
    return (error) => this.fromHttpError<T>(error);
  }
}