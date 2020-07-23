import { Dispatch, SiteSelectItem } from 'umi';
import { Column, ColumnField } from '../columnlist/data';
import { ColumnField } from '@/components/Content/data';

export interface ContentManagementProps {
    dispatch: Dispatch;
    columnData: ColumnItem[];
    loadingColumnData?: boolean;
    loadingTableColumns?: boolean;
    currentColumnNum?: string;
    currentTableFields?: ColumnField[];
    currentSite?: SiteSelectItem;
}

export interface ColumnContentItem {
    id: number;
    num: string;
    createDate: string;
    updateDate: string;
    seoTitle: string;
    seoKeyword: string;
    seoDesc: string;
    [key: string]: any;
}

export interface ColumnItem extends Column {

}

export type ColumnTableFieldsType = {
    [key: string]: ColumnField[];
}

export interface ContentModelState {
    columns: ColumnItem[];
    columnTableFields?: ColumnTableFieldsType;
    currentColumnNum?: string;
}

export interface ContentEditProps extends ContentEditState {
    dispatch: Dispatch;
    columnFields: ColumnField[];
    columnNum: string;
    onClose: (isSuccess?: boolean) => void;
}

export interface ContentEditState {
    visible: boolean;
    itemNum?: string;
}