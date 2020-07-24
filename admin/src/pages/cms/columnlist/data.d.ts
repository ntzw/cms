import { ModalBase } from '@/components/ListTable';
import { SiteSelectItem } from 'umi'


export interface ColumnListProps {
    currentSite?: SiteSelectItem;
}

export interface Column extends ModalBase {
    name: string;
    siteNum: string;
    parentNum: string | string[];
    modelNum: string;
    children: Column[];
}

export interface ColumnFieldListProps extends ColumnFieldListPropsState {
    onClose: () => void;
}

export interface ColumnFieldListPropsState {
    visible: boolean;
    column?: Column | Column[];
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





export interface ContentItem extends ModalBase {

}