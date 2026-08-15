import { HttpErrorResponse } from '@angular/common/http';
import { AppError, ErrorCode, Result } from '@crossdyne/toolkit';

export class ResultHttp {

  static success<T>(value: T): Result<T> {
    return Result.success(value);
  }

  static failure<T>(error: HttpErrorResponse): Result<T> {
    if (Array.isArray(error.error)) {
      const errors = error.error
        .filter((item: any) => item?.code?.name && item?.code?.code && item?.message)
        .map((item: any) => AppError.fromJSON(item));
      
      if (errors.length > 0) {
        return Result.failure(errors);
      }
    }

    const message = error.message || `HTTP ${error.status}: ${error.message}`;
    const errorCode = this.mapStatusToCode(error.status);
    
    return Result.failure(new AppError(errorCode, message));
  }

  private static mapStatusToCode(status: number): ErrorCode {
    switch (status) {
      case 400: return ErrorCode.BadRequest;
      case 401: return ErrorCode.Unauthorized;
      case 404: return ErrorCode.NotFound;
      case 409: return ErrorCode.Conflict;
      case 0:   return ErrorCode.Connection; 
      default:  return status >= 500 ? ErrorCode.Server : ErrorCode.InvalidResponse;
    }
  }
}