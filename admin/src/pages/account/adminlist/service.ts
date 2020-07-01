import request, { HandleResult } from "@/utils/request";

export function page(params?: { [key: string]: any }, sort?: { [key: string]: any }) {
    return request('/api/Account/Admin/Page', {
        method: "POST",
        data: {
            params,
            sort
        },
    });
}

export function Submit(value?: { [name: string]: any }) : Promise<HandleResult> {
    return request('/api/Account/Admin/Edit', {
        method: "POST",
        data: {
            ...value,
        },
    });
}
