
export interface ApiResponseWithType<T> {
    isSuccess: boolean;
    errorMessage: string;
    result: T;
}
