import { ModalBase } from '@/components/ListTable';
import { SiteSelectItem } from 'umi'
import { FormItemType } from '@/components/DynamicForm'

export interface ColumnListProps {
    currentSite?: SiteSelectItem;
}

export interface Column extends ModalBase {
    name: string;
    siteNum: string;
    parentNum: string | string[];
    modelNum: string;
}

export interface ColumnFieldListProps extends ColumnFieldListPropsState {
    onClose: () => void;
}

export interface ColumnFieldListPropsState {
    visible: boolean;
    column?: Column;
}

export interface ColumnField extends FieldDefaultType {
    columnNum: string;
}

export interface FieldDefaultType extends ModalBase {
    name: string;
    explain: string;
    optionType: FormItemType;
    options: string;
}

export interface ModelFieldAddProps extends ModelFieldAddPropsState {
    onClose: () => void;
    onSuccess: () => void;
}

export interface ModelFieldAddPropsState {
    visible: boolean;
    editType: 'model' | 'column'
    modelNum?: string;
    editId?: number;
}

export interface ColumnFormProps {
    columnNum?: string;
    itemNum?: string;
    actionRef?: React.MutableRefObject<ColumnFormAction | undefined> | ((actionRef: ColumnFormAction) => void);
}

export interface ColumnFormAction {
    reload: () => void;
}

export interface ContentItem extends ModalBase {

}

export interface ContentEditType {
    editValue: ContentItem;
    fields: ColumnField[];
}