import request, { AsyncHandleResult } from "@/utils/request";
import { ColumnField } from "./data";
import { SeoInfo } from "./SeoForm";

export function GetColumnContentFields(columnNum?: string): AsyncHandleResult<{ fields: ColumnField[]; }> {
    return request('/Api/CMS/Content/GetFields', {
        method: "POST",
        data: {
            columnNum,
        },
    });
}

export function SeoAction(url: string, data?: { [key: string]: any }): AsyncHandleResult<SeoInfo> {
    return request(url, {
        method: "POST",
        data,
    });
}

