export class ArrayHelper {
  static reset<T>(target: T[], source: T[]): void {
    target.length = 0;
    target.push(...source);
  }
}