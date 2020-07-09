import { ModalBase } from '@/components/ListTable';

export interface ModelTableListProps {

}

export interface ModelTable extends ModalBase {
    tableName: string;
    explain: string;
}

export interface ModelFieldListProps extends ModelFieldListPropsState {
    onClose: () => void;
}

export interface ModelFieldListPropsState {
    modelItem?: ModelTable;
    visible: boolean;
}

export interface ModelField extends ModalBase {

}