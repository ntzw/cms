import request, { AsyncHandleResult } from "@/utils/request";
import { ColumnField } from "./data";

export function GetColumnContentFields(columnNum?: string): AsyncHandleResult<{ fields: ColumnField[]; }> {
    return request('/Api/CMS/Content/GetFields', {
        method: "POST",
        data: {
            columnNum,
        },
    });
}
