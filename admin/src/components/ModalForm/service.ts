import request, { HandleResult } from "@/utils/request";

export function getFields<T>(url: string, params?: { [key: string]: any }): Promise<HandleResult<T> | undefined> {
    return request(url, {
        method: "POST",
        data: {
            ...params,
        },
    });
}

export function getAsyncData(url: string, params?: { [key: string]: any }): Promise<HandleResult<Array<any>> | undefined> {
    return request(url, {
        method: "POST",
        data: {
            ...params,
        },
    });
}
