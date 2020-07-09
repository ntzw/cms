import request, { HandleResult } from "@/utils/request";
import { RequestData } from "@/components/ListTable";

type Result = Promise<HandleResult>;

export function page(): Promise<RequestData<any>> {
    return request('/api/Account/Role/Page', {
        method: "POST",
    });
}

export function Submit(value?: { [name: string]: any }): Result {
    return request('/api/Account/Role/Edit', {
        method: "POST",
        data: {
            ...value,
        },
    });
}

export function Delete(ids: number[]): Result {
    return request('/api/Account/Role/Delete', {
        method: "POST",
        data: ids,
    });
}
