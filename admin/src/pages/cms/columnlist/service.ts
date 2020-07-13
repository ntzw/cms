import request, { AsyncHandleResult } from "@/utils/request";
import { PageParamsType } from "@/components/ListTable";
import { ContentEditType, FieldDefaultType } from "./data";

export function page(params?: { [key: string]: any }) {
    return request('/Api/CMS/Column/Page', {
        method: "POST",
        data: {
            ...params,
        },
    });
}

export function Submit(value?: { [name: string]: any }): AsyncHandleResult {
    return request('/Api/CMS/Column/Edit', {
        method: "POST",
        data: {
            ...value,
        },
    });
}

export function Delete(ids: number[]): AsyncHandleResult {
    return request('/Api/CMS/Column/Delete', {
        method: "POST",
        data: ids,
    });
}

export function columnFieldPage(data: PageParamsType) {
    return request('/Api/CMS/ColumnField/Page', {
        method: "POST",
        data: {
            ...data
        },
    });
}

export function DeleteColumnField(ids: string[]): AsyncHandleResult {
    return request('/Api/CMS/ColumnField/Delete', {
        method: "POST",
        data: ids,
    });
}

export function SubmitModelField(value?: { [name: string]: any }): AsyncHandleResult {
    return request('/Api/CMS/Model/FieldEdit', {
        method: "POST",
        data: {
            ...value,
        },
    });
}

export function MoveModelField(columnNum: string, fieldNums: string[]): AsyncHandleResult {
    return request('/Api/CMS/ColumnField/MoveModelField', {
        method: "POST",
        data: {
            columnNum,
            fieldNums,
        },
    });
}

export function GetColumnContentEdit(itemNum?: string, columnNum?: string): AsyncHandleResult<ContentEditType> {
    return request('/Api/CMS/Content/GetEdit', {
        method: "POST",
        data: {
            columnNum,
            itemNum
        },
    });
}

export function GetColumnFieldEditValue(id: number): AsyncHandleResult<FieldDefaultType> {
    return request('/Api/CMS/ColumnField/GetEditValue', {
        method: "POST",
        data: {
            id,
        },
    });
}

export function GetModelFieldEditValue(id: number): AsyncHandleResult<FieldDefaultType> {
    return request('/Api/CMS/Model/GetFieldEditValue', {
        method: "POST",
        data: {
            id,
        },
    });
}

export function SubmitColumnFieldEdit(value: any): AsyncHandleResult {
    return request('/Api/CMS/ColumnField/Edit', {
        method: "POST",
        data: {
            ...value,
        },
    });
}

