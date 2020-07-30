import { PageParamsType } from "@/components/ListTable";
import request, { AsyncHandleResult } from "@/utils/request";
import { ColumnContentItem } from "./data";


export function ContentPage({ params, sort, query }: PageParamsType) {
    return request('/Api/CMS/Content/Page', {
        method: "POST",
        data: {
            params,
            sort,
            query
        },
    });
}

export function ContentSubmit(params: { columnNum: string; itemNum: string;[key: string]: any; }): AsyncHandleResult {
    return request('/Api/CMS/Content/Submit', {
        method: "POST",
        data: {
            ...params,
        },
    });
}

export function GetEditValue(itemNum: string, columnNum: string): AsyncHandleResult<ColumnContentItem> {
    return request('/Api/CMS/Content/GetEdit', {
        method: "POST",
        data: {
            itemNum,
            columnNum
        },
    });
}

export function categoryPage(columnNum: string) {
    return request('/Api/CMS/Category/Page', {
        method: "POST",
        data: {
            columnNum,
        },
    });
}

export function SubmitCategory(values: any): AsyncHandleResult {
    return request('/Api/CMS/Category/Submit', {
        method: "POST",
        data: {
            ...values,
        },
    });
}

export function DeleteCategory(ids: number[], columnNum: string): AsyncHandleResult {
    return request('/Api/CMS/Category/Delete', {
        method: "POST",
        data: {
            ids: ids.join(','),
            columnNum
        },
    });
}
