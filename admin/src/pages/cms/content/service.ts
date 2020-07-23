import { PageParamsType } from "@/components/ListTable";
import request, { AsyncHandleResult } from "@/utils/request";
import { ColumnContentItem } from "./data";


export function page({ params, sort, query }: PageParamsType) {
    return request('/Api/CMS/Content/Page', {
        method: "POST",
        data: {
            params,
            sort,
            query
        },
    });
}

export function submit(params: { columnNum: string; itemNum: string;[key: string]: any; }): AsyncHandleResult {
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
