import { Dispatch, SiteSelectItem } from 'umi';
import { Column, ColumnField } from '../columnlist/data';
import { ColumnField, ContentFormProps } from '@/components/Content/data';
import { ModalBase } from '@/components/ListTable';

export interface ContentManagementProps {
    dispatch: Dispatch;
    columnData: ColumnItem[];
    loadingColumnData?: boolean;
    loadingTableColumns?: boolean;
    currentColumnNum?: string;
    currentColumn?: ColumnItem;
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
    currentColumn?: ColumnItem;
}

export interface ContentEditProps extends ContentEditState {
    dispatch: Dispatch;
    columnFields: ColumnField[];
    currentColumnNum?: string;
    currentColumn?: ColumnItem;
    actionRef?: ContentFormProps['actionRef'];
    onClose: (isSuccess?: boolean) => void;
}

export interface ContentEditState {
    visible: boolean;
    itemNum?: string;
}

export interface CategoryManagementProps extends CategoryManagementState {
    currentColumnNum?: string;
    currentColumn?: ColumnItem;
    currentSite?: SiteSelectItem;
    onClose: () => void;
}

export interface CategoryManagementState {
    visible: boolean;
}

export interface ContentCategory extends ModalBase {
    columnNum: string;
    siteNum: string;
    parentNum: string | string[];
    name: string;
}

export interface ContentManageProps {
    currentColumnNum?: string;
    currentColumn?: ColumnItem;
    currentTableFields?: ColumnField[];
    currentSite?: SiteSelectItem;
}