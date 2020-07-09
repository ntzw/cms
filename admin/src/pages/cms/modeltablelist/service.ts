import request, { AsyncHandleResult } from "@/utils/request";
import { QuerySymbol } from "@/components/ListTable";

export function page(params?: { [key: string]: any }, sort?: { [key: string]: any }, query?: { [key: string]: QuerySymbol }) {
    return request('/Api/CMS/Model/Page', {
        method: "POST",
        data: {
            params,
            sort,
            query
        },
    });
}

export function Submit(value?: { [name: string]: any }): AsyncHandleResult {
    return request('/Api/CMS/Model/Edit', {
        method: "POST",
        data: {
            ...value,
        },
    });
}

export function Delete(ids: number[]): AsyncHandleResult {
    return request('/Api/CMS/Model/Delete', {
        method: "POST",
        data: ids,
    });
}

export function fieldPage(params?: { [key: string]: any }, sort?: { [key: string]: any }, query?: { [key: string]: QuerySymbol }) {
    return request('/Api/CMS/Model/FieldPage', {
        method: "POST",
        data: {
            params,
            sort,
            query
        },
    });
}
