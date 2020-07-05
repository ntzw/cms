import request, { HandleResult } from "@/utils/request";
import { QuerySymbol } from "@/components/ListTable";

export function page(params?: { [key: string]: any }, sort?: { [key: string]: any }, query?: { [key: string]: QuerySymbol }) {
    return request('/api/Account/Admin/Page', {
        method: "POST",
        data: {
            params,
            sort,
            query
        },
    });
}

export function Submit(value?: { [name: string]: any }): Promise<HandleResult> {
    return request('/api/Account/Admin/Edit', {
        method: "POST",
        data: {
            ...value,
        },
    });
}
