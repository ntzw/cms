import request, { HandleResult } from "@/utils/request";

export function exportData(url: string, params?: { [key: string]: any }): Promise<HandleResult<string> | undefined> {
    return request(url, {
        method: "POST",
        data: {
            ...params,
        },
    });
}
