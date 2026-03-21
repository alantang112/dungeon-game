export {};

declare global {
  interface Window {
    Blazor: {
      start: () => Promise<void>;
    };
    DotNet: {
      invokeMethodAsync: <T>(assemblyName: string, methodName: string, ...args: any[]) => Promise<T>;
    };
    IsBlazorStarted?: boolean;
  }
}