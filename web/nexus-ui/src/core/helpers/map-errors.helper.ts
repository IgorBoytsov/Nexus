import { ErrorList } from "@crossdyne/toolkit";

export class MapErrorsHelper {
    
    static mapErrors(errors: ErrorList): string{
        return errors.map(e => e.message).join(', ')
    }
} 