export class ArrayUtils {
  static reset<T>(target: T[], source: T[]): void {
    target.length = 0;
    target.push(...source);
  }
}