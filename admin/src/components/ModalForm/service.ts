import request, { HandleResult } from "@/utils/request";

export async function getFields<T>(url: string, params?: { [key: string]: any }): Promise<HandleResult<T> | undefined> {
    return request(url, {
        method: "POST",
        data: {
            ...params,
        },
    });
}
